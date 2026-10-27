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
    /// Interim lookups while timesheet, scorecard, and rate reach are specified separately.
    /// Team-derived reach is empty. The capability summary still reports who manages a group or leads a team,
    /// because the Work Groups menu uses that.
    /// </summary>
    public class WorkTeamAccessProvider : TRiZHubProvider, IWorkTeamAccessProvider
    {
        public WorkTeamAccessProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        public IQueryable<Guid> ManagedUserIds(Guid actorId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return NoUsers();
        }

        public bool CanActOn(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return false;
        }

        public List<WorkTeamTypePeriod> ReachablePeriods(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return new List<WorkTeamTypePeriod>();
        }

        public WorkTeamScope TeamScopeFor(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to)
        {
            return new WorkTeamScope();
        }

        public WorkTeamScope TeamScope(IEnumerable<Guid> workTeamIds)
        {
            return new WorkTeamScope();
        }

        public IQueryable<Guid> EditableTeamIds(Guid actorId, WorkTeamCapability cap)
        {
            return DataContext.WorkTeamSet.Where(t => false).Select(t => t.Id);
        }

        public List<WorkTeamTypePeriod> TeamTypePeriodsFor(Guid userId, DateTime from, DateTime to)
        {
            return new List<WorkTeamTypePeriod>();
        }

        public WorkTeamCapabilitySummary CapabilitySummary(Guid actorId)
        {
            return new WorkTeamCapabilitySummary
            {
                IsTeamManager = DataContext.WorkGroupManagerSet.Any(m => m.UserAccountId == actorId),
                IsTeamLead = DataContext.WorkTeamMemberSet.Any(m => m.UserAccountId == actorId && m.Role == WorkTeamRoleType.Lead),
                IsTeamMember = DataContext.WorkTeamMemberSet.Any(m => m.UserAccountId == actorId && m.Role == WorkTeamRoleType.Member)
            };
        }

        public IQueryable<Guid> MaintainedClientIds(Guid actorId)
        {
            return NoUsers();
        }

        public bool CanMaintainClient(Guid actorId, Guid clientId)
        {
            return false;
        }

        private IQueryable<Guid> NoUsers()
        {
            return DataContext.UserAccountSet.Where(u => false).Select(u => u.Id);
        }
    }
}
