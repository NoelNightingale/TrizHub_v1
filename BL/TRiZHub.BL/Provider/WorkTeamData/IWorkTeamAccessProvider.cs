#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public interface IWorkTeamAccessProvider : ITRiZHubProvider
    {
        IQueryable<Guid> ManagedUserIds(Guid actorId, WorkTeamCapability cap, DateTime from, DateTime to);

        bool CanActOn(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to);

        /// <summary>The target's Member/Lead periods, overlapping [from, to], through which the actor reaches them.</summary>
        List<WorkTeamTypePeriod> ReachablePeriods(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to);

        WorkTeamScope TeamScopeFor(Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime from, DateTime to);

        WorkTeamScope TeamScope(IEnumerable<Guid> workTeamIds);

        IQueryable<Guid> EditableTeamIds(Guid actorId, WorkTeamCapability cap);

        List<WorkTeamTypePeriod> TeamTypePeriodsFor(Guid userId, DateTime from, DateTime to);

        WorkTeamCapabilitySummary CapabilitySummary(Guid actorId);
    }
}
