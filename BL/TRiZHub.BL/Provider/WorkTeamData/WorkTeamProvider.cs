#region Usings

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TCR.Lib.BL;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.WorkTeamData;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public class WorkTeamProvider : TRiZHubProvider, IWorkTeamProvider
    {
        #region Constructor

        public WorkTeamProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        #region Helpers

        private bool IsAdmin
        {
            get { return UserIsAllowed(PrivilegeType.TeamMaintenance); }
        }

        private Guid CurrentUserId
        {
            get
            {
                if (CurrentUser == null)
                    throw new GenericSecurityException("Not Allowed!");
                return CurrentUser.Id;
            }
        }

        private List<WorkTeamRoleType> CurrentRolesIn(Guid workTeamId)
        {
            var today = DateTime.Today;
            var userId = CurrentUserId;
            return DataContext.WorkTeamMemberSet
                .Where(m => m.WorkTeamId == workTeamId
                            && m.UserAccountId == userId
                            && m.StartDate <= today
                            && (m.EndDate == null || m.EndDate >= today))
                .Select(m => m.Role)
                .ToList();
        }

        private WorkTeam LoadTeam(Guid workTeamId)
        {
            var team = DataContext.WorkTeamSet.Include(t => t.TeamType).FirstOrDefault(t => t.Id == workTeamId);
            if (team == null)
                throw new WorkTeamException("The team could not be found.");
            return team;
        }

        #endregion

        #region Team

        public IQueryable<WorkTeam> WorkTeamList()
        {
            if (IsAdmin)
                return DataContext.WorkTeamSet;

            var today = DateTime.Today;
            var userId = CurrentUserId;
            var teamIds = DataContext.WorkTeamMemberSet
                .Where(m => m.UserAccountId == userId
                            && (m.Role == WorkTeamRoleType.Manager || m.Role == WorkTeamRoleType.Lead)
                            && m.StartDate <= today
                            && (m.EndDate == null || m.EndDate >= today))
                .Select(m => m.WorkTeamId);

            return DataContext.WorkTeamSet.Where(t => teamIds.Contains(t.Id));
        }

        public WorkTeam GetWorkTeam(Guid id)
        {
            if (!PermissionsFor(id).CanView)
                throw new GenericSecurityException("Not Allowed!");
            return LoadTeam(id);
        }

        public WorkTeamPermissions PermissionsFor(Guid workTeamId)
        {
            var isAdmin = IsAdmin;
            var team = DataContext.WorkTeamSet.FirstOrDefault(t => t.Id == workTeamId);
            var roles = team == null ? new List<WorkTeamRoleType>() : CurrentRolesIn(workTeamId);
            var active = team != null && team.IsActive;
            var isManager = active && roles.Contains(WorkTeamRoleType.Manager);
            var isLead = active && roles.Contains(WorkTeamRoleType.Lead);
            var managerOrLead = isManager || isLead;

            return new WorkTeamPermissions
            {
                IsAdmin = isAdmin,
                IsManager = isManager,
                IsLead = isLead,
                CanView = isAdmin || managerOrLead,
                CanEditHeader = isAdmin,
                CanEditAdminFlags = isAdmin,
                CanEditLeadFlags = isAdmin || isManager,
                CanManageManagers = isAdmin,
                CanManagePeople = isAdmin || isManager,
                CanEditAllocations = isAdmin || isManager || (isLead && team.LeadsManageAllocations),
                CanTimesheets = UserIsAllowed(PrivilegeType.TimesheetCaptureForOtherAccounts)
                                || (managerOrLead && team.AllowTeamTimesheets),
                CanScorecards = UserIsAllowed(PrivilegeType.PerformanceManagementCreateScoreCards)
                                || (managerOrLead && team.AllowTeamScorecards),
                CanRates = UserIsAllowed(PrivilegeType.UserBillingRatesMaintenance)
                           || isManager || (isLead && team.LeadsManageRates)
            };
        }

        public WorkTeam SaveWorkTeam(Guid? id, string name, string description, bool isActive, Guid teamTypeId,
            bool allowTeamTimesheets, bool allowTeamScorecards, bool leadsManageAllocations, bool leadsManageRates)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new WorkTeamException("A team name is required.");

            name = name.Trim();
            var record = id.HasValue ? DataContext.WorkTeamSet.FirstOrDefault(t => t.Id == id.Value) : null;

            if (record == null)
            {
                Authenticate(PrivilegeType.TeamMaintenance);
                record = new WorkTeam();
                DataContext.WorkTeamSet.Add(record);
            }
            else
            {
                var permissions = PermissionsFor(record.Id);
                if (!permissions.CanEditLeadFlags)
                    throw new GenericSecurityException("Not Allowed!");

                if (!permissions.CanEditHeader
                    && (record.Name != name || (record.Description ?? "") != (description ?? "") || record.IsActive != isActive
                        || record.TeamTypeId != teamTypeId))
                    throw new WorkTeamException("Only administrators can change the team name, description, type or active status.");

                if (!permissions.CanEditAdminFlags
                    && (record.AllowTeamTimesheets != allowTeamTimesheets || record.AllowTeamScorecards != allowTeamScorecards))
                    throw new WorkTeamException("Only administrators can change the team timesheet and scorecard access.");
            }

            if (DataContext.WorkTeamSet.Any(t => t.Name == name && t.Id != record.Id))
                throw new WorkTeamException("A team with the name: " + name + " already exists.");

            if (!DataContext.TeamSet.Any(t => t.Id == teamTypeId))
                throw new WorkTeamException("Select a valid team type.");

            record.Name = name;
            record.Description = description;
            record.IsActive = isActive;
            record.TeamTypeId = teamTypeId;
            record.AllowTeamTimesheets = allowTeamTimesheets;
            record.AllowTeamScorecards = allowTeamScorecards;
            record.LeadsManageAllocations = leadsManageAllocations;
            record.LeadsManageRates = leadsManageRates;

            DataContextSaveChanges();
            return record;
        }

        #endregion

        #region People

        public IQueryable<WorkTeamMember> MemberList(Guid workTeamId)
        {
            if (!PermissionsFor(workTeamId).CanView)
                throw new GenericSecurityException("Not Allowed!");

            return DataContext.WorkTeamMemberSet.Include(m => m.UserAccount).Where(m => m.WorkTeamId == workTeamId);
        }

        private void EnsureCanEditRole(WorkTeamPermissions permissions, WorkTeamRoleType role)
        {
            var allowed = role == WorkTeamRoleType.Manager ? permissions.CanManageManagers : permissions.CanManagePeople;
            if (!allowed)
                throw new WorkTeamException(role == WorkTeamRoleType.Manager
                    ? "Only administrators can add or change team managers."
                    : "You are not allowed to change the people in this team.");
        }

        public WorkTeamMember SaveMember(Guid? id, Guid workTeamId, Guid userAccountId, WorkTeamRoleType role,
            DateTime startDate, DateTime? endDate)
        {
            LoadTeam(workTeamId);
            var permissions = PermissionsFor(workTeamId);
            EnsureCanEditRole(permissions, role);

            var start = startDate.Date;
            var end = endDate.HasValue ? endDate.Value.Date : (DateTime?)null;
            if (end.HasValue && end.Value < start)
                throw new WorkTeamException("The end date cannot be before the start date.");

            if (!DataContext.UserAccountSet.Any(u => u.Id == userAccountId))
                throw new WorkTeamException("Select a valid user.");

            var record = id.HasValue ? DataContext.WorkTeamMemberSet.FirstOrDefault(m => m.Id == id.Value) : null;
            if (record != null)
            {
                if (record.WorkTeamId != workTeamId)
                    throw new WorkTeamException("The membership does not belong to this team.");
                EnsureCanEditRole(permissions, record.Role);
            }

            var recordId = record == null ? Guid.Empty : record.Id;
            var sameRole = DataContext.WorkTeamMemberSet.Where(m => m.WorkTeamId == workTeamId
                                                                   && m.UserAccountId == userAccountId
                                                                   && m.Role == role
                                                                   && m.Id != recordId
                                                                   && (m.EndDate == null || m.EndDate >= start));
            if (end.HasValue)
            {
                var endDay = end.Value;
                sameRole = sameRole.Where(m => m.StartDate <= endDay);
            }

            if (sameRole.Any())
                throw new WorkTeamException("This person already has the " + role + " role in this team for an overlapping period.");

            if (record == null)
            {
                record = new WorkTeamMember { WorkTeamId = workTeamId };
                DataContext.WorkTeamMemberSet.Add(record);
            }

            record.UserAccountId = userAccountId;
            record.Role = role;
            record.StartDate = start;
            record.EndDate = end;

            DataContextSaveChanges();
            return record;
        }

        public void DeleteMember(Guid id)
        {
            var record = DataContext.WorkTeamMemberSet.FirstOrDefault(m => m.Id == id);
            if (record == null)
                throw new WorkTeamException("The membership could not be found.");

            EnsureCanEditRole(PermissionsFor(record.WorkTeamId), record.Role);

            DataContext.WorkTeamMemberSet.Remove(record);
            DataContextSaveChanges();
        }

        #endregion

        #region Allocations

        public List<WorkTeamClient> ClientAllocations(Guid workTeamId)
        {
            if (!PermissionsFor(workTeamId).CanView)
                throw new GenericSecurityException("Not Allowed!");
            return DataContext.WorkTeamClientSet.Where(c => c.WorkTeamId == workTeamId).ToList();
        }

        public List<WorkTeamProject> ProjectAllocations(Guid workTeamId)
        {
            if (!PermissionsFor(workTeamId).CanView)
                throw new GenericSecurityException("Not Allowed!");
            return DataContext.WorkTeamProjectSet.Where(p => p.WorkTeamId == workTeamId).ToList();
        }

        public void SaveAllocations(Guid workTeamId, IEnumerable<Guid> clientIds,
            IEnumerable<KeyValuePair<Guid, Guid?>> projectAndSubProjectIds)
        {
            LoadTeam(workTeamId);
            if (!PermissionsFor(workTeamId).CanEditAllocations)
                throw new WorkTeamException("You are not allowed to change this team's allocations.");

            var wholeClients = new HashSet<Guid>(clientIds.Where(c => c != Guid.Empty));
            var selected = projectAndSubProjectIds.Where(p => p.Key != Guid.Empty).Distinct().ToList();
            var projectIds = selected.Select(p => p.Key).Distinct().ToList();
            var projectClients = DataContext.ProjectSet.Where(p => projectIds.Contains(p.Id))
                .Select(p => new { p.Id, p.ClientId }).ToList()
                .ToDictionary(p => p.Id, p => p.ClientId);

            // Whole clients swallow their projects; whole projects swallow their subprojects.
            selected = selected.Where(p => projectClients.ContainsKey(p.Key) && !wholeClients.Contains(projectClients[p.Key])).ToList();
            var wholeProjects = new HashSet<Guid>(selected.Where(p => !p.Value.HasValue || p.Value == Guid.Empty).Select(p => p.Key));
            var subProjects = selected.Where(p => p.Value.HasValue && p.Value != Guid.Empty && !wholeProjects.Contains(p.Key)).ToList();

            var wanted = wholeProjects.Select(p => new KeyValuePair<Guid, Guid?>(p, null))
                .Concat(subProjects)
                .ToList();

            var existingClients = DataContext.WorkTeamClientSet.Where(c => c.WorkTeamId == workTeamId).ToList();
            DataContext.WorkTeamClientSet.RemoveRange(existingClients.Where(c => !wholeClients.Contains(c.ClientId)).ToList());
            foreach (var clientId in wholeClients.Where(c => existingClients.All(e => e.ClientId != c)))
                DataContext.WorkTeamClientSet.Add(new WorkTeamClient { WorkTeamId = workTeamId, ClientId = clientId });

            var existingProjects = DataContext.WorkTeamProjectSet.Where(p => p.WorkTeamId == workTeamId).ToList();
            DataContext.WorkTeamProjectSet.RemoveRange(existingProjects
                .Where(e => !wanted.Any(w => w.Key == e.ProjectId && w.Value == e.SubProjectId)).ToList());
            foreach (var w in wanted.Where(w => !existingProjects.Any(e => e.ProjectId == w.Key && e.SubProjectId == w.Value)))
                DataContext.WorkTeamProjectSet.Add(new WorkTeamProject { WorkTeamId = workTeamId, ProjectId = w.Key, SubProjectId = w.Value });

            DataContextSaveChanges();
        }

        public IQueryable<ClientEntity> ClientCatalog()
        {
            return DataContext.ClientEntitySet.Where(c => !c.IsDeleted);
        }

        public IQueryable<Project> ProjectCatalog()
        {
            return DataContext.ProjectSet.Where(p => !p.IsDeleted);
        }

        public IQueryable<SubProject> SubProjectCatalog()
        {
            return DataContext.SubProjectSet.Where(s => !s.IsDeleted);
        }

        #endregion
    }
}
