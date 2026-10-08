#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
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
            return WorkTeamAccessRules.ReachableUserIds(DataContext.WorkTeamMemberSet, actorId, cap, DateTime.Today, from, to);
        }

        public bool CanActOn(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return ManagedUserIds(actorId, cap, from, to).Any(id => id == targetUserId);
        }

        public List<WorkTeamTypePeriod> ReachablePeriods(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return WorkTeamAccessRules.ReachableRows(DataContext.WorkTeamMemberSet, actorId, cap, DateTime.Today, from, to)
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
                .ReachingTeamIds(DataContext.WorkTeamMemberSet, actorId, targetUserId, cap, DateTime.Today, from, to)
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
            return WorkTeamAccessRules.QualifyingActorRows(DataContext.WorkTeamMemberSet, actorId, cap, DateTime.Today)
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
            var rows = DataContext.WorkTeamMemberSet
                .Where(m => m.UserAccountId == actorId
                            && m.WorkTeam.IsActive
                            && m.StartDate <= today
                            && (m.EndDate == null || m.EndDate >= today)
                            && (m.Role == WorkTeamRoleType.Manager || m.Role == WorkTeamRoleType.Lead))
                .Select(m => new
                {
                    m.Role,
                    m.WorkTeam.AllowTeamTimesheets,
                    m.WorkTeam.AllowTeamScorecards,
                    m.WorkTeam.LeadsManageAllocations,
                    m.WorkTeam.LeadsManageRates
                })
                .ToList();

            return new WorkTeamCapabilitySummary
            {
                IsTeamManager = rows.Any(r => r.Role == WorkTeamRoleType.Manager),
                IsTeamLead = rows.Any(r => r.Role == WorkTeamRoleType.Lead),
                Timesheets = rows.Any(r => r.AllowTeamTimesheets),
                Scorecards = rows.Any(r => r.AllowTeamScorecards),
                TeamAllocations = rows.Any(r => r.Role == WorkTeamRoleType.Manager || r.LeadsManageAllocations),
                Rates = rows.Any(r => r.Role == WorkTeamRoleType.Manager || r.LeadsManageRates),
                ManageTeam = rows.Any(r => r.Role == WorkTeamRoleType.Manager)
            };
        }
    }
}
