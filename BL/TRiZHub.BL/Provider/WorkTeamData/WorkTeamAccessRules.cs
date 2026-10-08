#region Usings

using System;
using System.Linq;
using System.Linq.Expressions;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.WorkTeamData;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    /// <summary>
    /// Team authorization rules expressed as composable queries, so the same logic runs against
    /// EF (SQL) and against in-memory lists in unit tests.
    /// </summary>
    public static class WorkTeamAccessRules
    {
        public static Expression<Func<WorkTeamMember, bool>> GrantsCapability(WorkTeamCapability cap)
        {
            switch (cap)
            {
                case WorkTeamCapability.Timesheets:
                    return m => (m.Role == WorkTeamRoleType.Manager || m.Role == WorkTeamRoleType.Lead)
                                && m.WorkTeam.AllowTeamTimesheets;
                case WorkTeamCapability.Scorecards:
                    return m => (m.Role == WorkTeamRoleType.Manager || m.Role == WorkTeamRoleType.Lead)
                                && m.WorkTeam.AllowTeamScorecards;
                case WorkTeamCapability.TeamAllocations:
                    return m => m.Role == WorkTeamRoleType.Manager
                                || (m.Role == WorkTeamRoleType.Lead && m.WorkTeam.LeadsManageAllocations);
                case WorkTeamCapability.Rates:
                    return m => m.Role == WorkTeamRoleType.Manager
                                || (m.Role == WorkTeamRoleType.Lead && m.WorkTeam.LeadsManageRates);
                case WorkTeamCapability.ManageTeam:
                    return m => m.Role == WorkTeamRoleType.Manager;
                default:
                    return m => false;
            }
        }

        /// <summary>The actor's role rows (in active teams, current on <paramref name="today"/>) that grant <paramref name="cap"/>.</summary>
        public static IQueryable<WorkTeamMember> QualifyingActorRows(IQueryable<WorkTeamMember> members,
            Guid actorId, WorkTeamCapability cap, DateTime today)
        {
            var day = today.Date;
            return members
                .Where(m => m.UserAccountId == actorId
                            && m.WorkTeam.IsActive
                            && m.StartDate <= day
                            && (m.EndDate == null || m.EndDate >= day))
                .Where(GrantsCapability(cap));
        }

        /// <summary>
        /// Role rows of people the actor can reach for <paramref name="cap"/> whose own period overlaps
        /// [<paramref name="from"/>, <paramref name="to"/>]. Managers reach Members and Leads, Leads reach
        /// Members, and nobody reaches themselves.
        /// </summary>
        public static IQueryable<WorkTeamMember> ReachableRows(IQueryable<WorkTeamMember> members,
            Guid actorId, WorkTeamCapability cap, DateTime today, DateTime from, DateTime to)
        {
            var fromDay = from.Date;
            var toDay = to.Date;
            var actorRows = QualifyingActorRows(members, actorId, cap, today);

            return from a in actorRows
                   join t in members on a.WorkTeamId equals t.WorkTeamId
                   where t.UserAccountId != actorId
                         && (t.Role == WorkTeamRoleType.Member
                             || (a.Role == WorkTeamRoleType.Manager && t.Role == WorkTeamRoleType.Lead))
                         && t.StartDate <= toDay
                         && (t.EndDate == null || t.EndDate >= fromDay)
                   select t;
        }

        public static IQueryable<Guid> ReachableUserIds(IQueryable<WorkTeamMember> members,
            Guid actorId, WorkTeamCapability cap, DateTime today, DateTime from, DateTime to)
        {
            return ReachableRows(members, actorId, cap, today, from, to).Select(t => t.UserAccountId).Distinct();
        }

        /// <summary>Teams through which the actor reaches <paramref name="targetUserId"/> for <paramref name="cap"/>.</summary>
        public static IQueryable<Guid> ReachingTeamIds(IQueryable<WorkTeamMember> members,
            Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime today, DateTime from, DateTime to)
        {
            return ReachableRows(members, actorId, cap, today, from, to)
                .Where(t => t.UserAccountId == targetUserId)
                .Select(t => t.WorkTeamId)
                .Distinct();
        }

        /// <summary>Any-role rows of <paramref name="userId"/> in active teams overlapping [from, to]; these drive inherited allocations.</summary>
        public static IQueryable<WorkTeamMember> OwnRowsOverlapping(IQueryable<WorkTeamMember> members,
            Guid userId, DateTime from, DateTime to)
        {
            var fromDay = from.Date;
            var toDay = to.Date;
            return members.Where(m => m.UserAccountId == userId
                                      && m.WorkTeam.IsActive
                                      && m.StartDate <= toDay
                                      && (m.EndDate == null || m.EndDate >= fromDay));
        }

        public static bool Overlaps(DateTime start, DateTime? end, DateTime from, DateTime to)
        {
            return start.Date <= to.Date && (end == null || end.Value.Date >= from.Date);
        }
    }
}
