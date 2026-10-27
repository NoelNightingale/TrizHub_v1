#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    /// <summary>
    /// Data lookup only: callers decide who may see whose allocations.
    /// </summary>
    public class EffectiveAllocationProvider : TRiZHubProvider, IEffectiveAllocationProvider
    {
        #region Constructor

        public EffectiveAllocationProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        public EffectiveAllocations For(Guid userId, DateTime from, DateTime to)
        {
            EffectiveAllocations result;
            return ForUsers(new[] { userId }, from, to).TryGetValue(userId, out result) ? result : new EffectiveAllocations();
        }

        public Dictionary<Guid, EffectiveAllocations> ForUsers(IEnumerable<Guid> userIds, DateTime from, DateTime to)
        {
            var ids = userIds.Distinct().ToList();
            var result = ids.ToDictionary(id => id, id => new EffectiveAllocations());
            if (ids.Count == 0)
                return result;

            foreach (var row in DataContext.UserIdentityClientSet
                         .Where(c => ids.Contains(c.UserAccountId) && c.ClientId != null)
                         .Select(c => new { c.UserAccountId, c.ClientId })
                         .ToList())
            {
                result[row.UserAccountId].DirectClientIds.Add(row.ClientId.Value);
            }

            foreach (var row in DataContext.UserIdentityProjectSet
                         .Where(p => ids.Contains(p.UserAccountId) && p.ProjectId != null)
                         .Select(p => new { p.UserAccountId, p.ProjectId, p.SubProjectId })
                         .ToList())
            {
                if (row.SubProjectId.HasValue)
                    result[row.UserAccountId].DirectSubProjectIds.Add(row.SubProjectId.Value);
                else
                    result[row.UserAccountId].DirectProjectIds.Add(row.ProjectId.Value);
            }

            // Team allocations are out of scope until the next specification. Direct rows above still apply.
            return result;
        }

        public IQueryable<Guid> TeamUserIdsForClients(IEnumerable<Guid> clientIds, DateTime from, DateTime to)
        {
            return None();
        }

        public IQueryable<Guid> TeamUserIdsForProjects(IEnumerable<Guid> projectIds, DateTime from, DateTime to)
        {
            return None();
        }

        public IQueryable<Guid> TeamClientIdsForUsers(IEnumerable<Guid> userIds, DateTime from, DateTime to)
        {
            return None();
        }

        public IQueryable<Guid> TeamProjectIdsForUsers(IEnumerable<Guid> userIds, DateTime from, DateTime to)
        {
            return None();
        }

        private IQueryable<Guid> None()
        {
            return DataContext.UserAccountSet.Where(u => false).Select(u => u.Id);
        }
    }
}
