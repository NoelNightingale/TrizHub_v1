#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public interface IEffectiveAllocationProvider
    {
        /// <summary>Direct allocations plus team allocations whose membership overlaps [from, to].</summary>
        EffectiveAllocations For(Guid userId, DateTime from, DateTime to);

        /// <summary>The same, for several users at once (rosters and reports).</summary>
        Dictionary<Guid, EffectiveAllocations> ForUsers(IEnumerable<Guid> userIds, DateTime from, DateTime to);

        /// <summary>People (any role) in active teams allocating one of the clients, membership overlapping [from, to].</summary>
        IQueryable<Guid> TeamUserIdsForClients(IEnumerable<Guid> clientIds, DateTime from, DateTime to);

        /// <summary>People in active teams allocating one of the projects (whole or a subproject).</summary>
        IQueryable<Guid> TeamUserIdsForProjects(IEnumerable<Guid> projectIds, DateTime from, DateTime to);

        /// <summary>Whole-client team allocations of the users, membership overlapping [from, to].</summary>
        IQueryable<Guid> TeamClientIdsForUsers(IEnumerable<Guid> userIds, DateTime from, DateTime to);

        /// <summary>Projects the users' teams allocate (whole or a subproject), membership overlapping [from, to].</summary>
        IQueryable<Guid> TeamProjectIdsForUsers(IEnumerable<Guid> userIds, DateTime from, DateTime to);
    }
}
