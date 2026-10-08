#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.WorkTeamData;
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
            var fromDay = from.Date;
            var toDay = to.Date;
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

            var memberships = DataContext.WorkTeamMemberSet
                .Where(m => ids.Contains(m.UserAccountId)
                            && m.WorkTeam.IsActive
                            && m.StartDate <= toDay
                            && (m.EndDate == null || m.EndDate >= fromDay))
                .Select(m => new { m.UserAccountId, m.WorkTeamId, m.WorkTeam.Name, m.StartDate, m.EndDate })
                .ToList();
            if (memberships.Count == 0)
                return result;

            var teamIds = memberships.Select(m => m.WorkTeamId).Distinct().ToList();
            var teamClients = DataContext.WorkTeamClientSet
                .Where(c => teamIds.Contains(c.WorkTeamId))
                .Select(c => new { c.WorkTeamId, c.ClientId })
                .ToList()
                .ToLookup(c => c.WorkTeamId);
            var teamProjects = DataContext.WorkTeamProjectSet
                .Where(p => teamIds.Contains(p.WorkTeamId))
                .Select(p => new { p.WorkTeamId, p.ProjectId, p.SubProjectId })
                .ToList()
                .ToLookup(p => p.WorkTeamId);

            foreach (var m in memberships)
            {
                var grants = result[m.UserAccountId].TeamGrants;
                foreach (var c in teamClients[m.WorkTeamId])
                {
                    grants.Add(new TeamAllocationGrant
                    {
                        WorkTeamId = m.WorkTeamId,
                        WorkTeamName = m.Name,
                        ClientId = c.ClientId,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate
                    });
                }

                foreach (var p in teamProjects[m.WorkTeamId])
                {
                    grants.Add(new TeamAllocationGrant
                    {
                        WorkTeamId = m.WorkTeamId,
                        WorkTeamName = m.Name,
                        ProjectId = p.ProjectId,
                        SubProjectId = p.SubProjectId,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate
                    });
                }
            }

            return result;
        }

        #region Team part as queries (to union with the direct UserIdentityClient/UserIdentityProject queries)

        private IQueryable<WorkTeamMember> MembershipsOverlapping(DateTime from, DateTime to)
        {
            var fromDay = from.Date;
            var toDay = to.Date;
            return DataContext.WorkTeamMemberSet
                .Where(m => m.WorkTeam.IsActive
                            && m.StartDate <= toDay
                            && (m.EndDate == null || m.EndDate >= fromDay));
        }

        public IQueryable<Guid> TeamUserIdsForClients(IEnumerable<Guid> clientIds, DateTime from, DateTime to)
        {
            var ids = clientIds.Distinct().ToList();
            var teamIds = DataContext.WorkTeamClientSet
                .Where(c => ids.Contains(c.ClientId))
                .Select(c => c.WorkTeamId);
            return MembershipsOverlapping(from, to)
                .Where(m => teamIds.Contains(m.WorkTeamId))
                .Select(m => m.UserAccountId)
                .Distinct();
        }

        public IQueryable<Guid> TeamUserIdsForProjects(IEnumerable<Guid> projectIds, DateTime from, DateTime to)
        {
            var ids = projectIds.Distinct().ToList();
            var teamIds = DataContext.WorkTeamProjectSet
                .Where(p => ids.Contains(p.ProjectId))
                .Select(p => p.WorkTeamId);
            return MembershipsOverlapping(from, to)
                .Where(m => teamIds.Contains(m.WorkTeamId))
                .Select(m => m.UserAccountId)
                .Distinct();
        }

        public IQueryable<Guid> TeamClientIdsForUsers(IEnumerable<Guid> userIds, DateTime from, DateTime to)
        {
            var ids = userIds.Distinct().ToList();
            var teamIds = MembershipsOverlapping(from, to)
                .Where(m => ids.Contains(m.UserAccountId))
                .Select(m => m.WorkTeamId);
            return DataContext.WorkTeamClientSet
                .Where(c => teamIds.Contains(c.WorkTeamId))
                .Select(c => c.ClientId)
                .Distinct();
        }

        public IQueryable<Guid> TeamProjectIdsForUsers(IEnumerable<Guid> userIds, DateTime from, DateTime to)
        {
            var ids = userIds.Distinct().ToList();
            var teamIds = MembershipsOverlapping(from, to)
                .Where(m => ids.Contains(m.UserAccountId))
                .Select(m => m.WorkTeamId);
            return DataContext.WorkTeamProjectSet
                .Where(p => teamIds.Contains(p.WorkTeamId))
                .Select(p => p.ProjectId)
                .Distinct();
        }

        #endregion
    }
}
