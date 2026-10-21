#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.WorkTeamData;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    /// <summary>
    /// Pure lookups answering "may this actor act on that person through a team". Callers combine the
    /// answer with global privileges and the system-admin bypass.
    /// </summary>
    public class WorkTeamAccessProvider : TRiZHubProvider, IWorkTeamAccessProvider
    {
        #region Constructor

        public WorkTeamAccessProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        public IQueryable<Guid> ManagedUserIds(Guid actorId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return WorkTeamAccessRules.ReachableUserIds(DataContext.WorkTeamMemberSet,
                DataContext.WorkTeamRolePrivilegeSet, DataContext.WorkTeamMemberPrivilegeSet,
                actorId, cap, DateTime.Today, from, to);
        }

        public bool CanActOn(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return ManagedUserIds(actorId, cap, from, to).Any(id => id == targetUserId);
        }

        public List<WorkTeamTypePeriod> ReachablePeriods(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return WorkTeamAccessRules.ReachableRows(DataContext.WorkTeamMemberSet,
                DataContext.WorkTeamRolePrivilegeSet, DataContext.WorkTeamMemberPrivilegeSet,
                actorId, cap, DateTime.Today, from, to)
                .Where(t => t.UserAccountId == targetUserId)
                .Select(t => new WorkTeamTypePeriod
                {
                    WorkTeamId = t.WorkTeamId,
                    TeamTypeId = t.WorkTeam.TeamTypeId,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                })
                .ToList();
        }

        public WorkTeamScope TeamScopeFor(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            var teamIds = WorkTeamAccessRules
                .ReachingTeamIds(DataContext.WorkTeamMemberSet,
                    DataContext.WorkTeamRolePrivilegeSet, DataContext.WorkTeamMemberPrivilegeSet,
                    actorId, targetUserId, cap, DateTime.Today, from, to)
                .ToList();
            return TeamScope(teamIds);
        }

        public WorkTeamScope TeamScope(IEnumerable<Guid> workTeamIds)
        {
            var ids = workTeamIds.Distinct().ToList();
            var scope = new WorkTeamScope();
            if (ids.Count == 0)
                return scope;

            foreach (var clientId in DataContext.WorkTeamClientSet.Where(c => ids.Contains(c.WorkTeamId)).Select(c => c.ClientId).Distinct())
                scope.ClientIds.Add(clientId);

            foreach (var row in DataContext.WorkTeamProjectSet.Where(p => ids.Contains(p.WorkTeamId))
                         .Select(p => new { p.ProjectId, p.SubProjectId }).ToList())
            {
                scope.ProjectIds.Add(row.ProjectId);
                if (row.SubProjectId.HasValue)
                    scope.SubProjectIds.Add(row.SubProjectId.Value);
            }

            return scope;
        }

        public IQueryable<Guid> EditableTeamIds(Guid actorId, WorkTeamCapability cap)
        {
            return WorkTeamAccessRules.QualifyingActorRows(DataContext.WorkTeamMemberSet,
                DataContext.WorkTeamRolePrivilegeSet, DataContext.WorkTeamMemberPrivilegeSet,
                actorId, cap, DateTime.Today)
                .Select(m => m.WorkTeamId)
                .Distinct();
        }

        public List<WorkTeamTypePeriod> TeamTypePeriodsFor(Guid userId, DateTime from, DateTime to)
        {
            return WorkTeamAccessRules.OwnRowsOverlapping(DataContext.WorkTeamMemberSet, userId, from, to)
                .Select(m => new WorkTeamTypePeriod
                {
                    WorkTeamId = m.WorkTeamId,
                    TeamTypeId = m.WorkTeam.TeamTypeId,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate
                })
                .ToList();
        }

        public WorkTeamCapabilitySummary CapabilitySummary(Guid actorId)
        {
            var today = DateTime.Today;
            var current = DataContext.WorkTeamMemberSet
                .Where(m => m.UserAccountId == actorId
                            && m.WorkTeam.IsActive
                            && m.StartDate <= today
                            && (m.EndDate == null || m.EndDate >= today));

            return new WorkTeamCapabilitySummary
            {
                IsTeamManager = current.Any(m => m.Role == WorkTeamRoleType.Manager),
                IsTeamLead = current.Any(m => m.Role == WorkTeamRoleType.Lead),
                Timesheets = HasPrivilege(current, WorkTeamPrivilege.TeamTimesheets),
                Scorecards = HasPrivilege(current, WorkTeamPrivilege.TeamScorecards),
                TeamAllocations = HasPrivilege(current, WorkTeamPrivilege.ManageMembers),
                Rates = HasPrivilege(current, WorkTeamPrivilege.ManageRates),
                ManageTeam = HasPrivilege(current, WorkTeamPrivilege.ManageMembers),
                MaintainClient = HasPrivilege(current, WorkTeamPrivilege.MaintainClient)
            };
        }

        /// <summary>Clients on teams where the actor currently has Maintain client. Whole-client allocations only.</summary>
        public IQueryable<Guid> MaintainedClientIds(Guid actorId)
        {
            var today = DateTime.Today;
            var current = DataContext.WorkTeamMemberSet
                .Where(m => m.UserAccountId == actorId
                            && m.WorkTeam.IsActive
                            && m.StartDate <= today
                            && (m.EndDate == null || m.EndDate >= today));
            var teamIds = WithPrivilege(current, WorkTeamPrivilege.MaintainClient).Select(m => m.WorkTeamId);
            return DataContext.WorkTeamClientSet
                .Where(c => teamIds.Contains(c.WorkTeamId))
                .Select(c => c.ClientId)
                .Distinct();
        }

        public bool CanMaintainClient(Guid actorId, Guid clientId)
        {
            return MaintainedClientIds(actorId).Any(id => id == clientId);
        }

        private bool HasPrivilege(IQueryable<WorkTeamMember> current, WorkTeamPrivilege privilege)
        {
            return WithPrivilege(current, privilege).Any();
        }

        private IQueryable<WorkTeamMember> WithPrivilege(IQueryable<WorkTeamMember> members, WorkTeamPrivilege privilege)
        {
            return WorkTeamAccessRules.WithPrivilege(members,
                DataContext.WorkTeamRolePrivilegeSet, DataContext.WorkTeamMemberPrivilegeSet, privilege);
        }
    }
}
