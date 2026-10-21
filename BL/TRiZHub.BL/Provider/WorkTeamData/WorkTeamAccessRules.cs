#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.WorkTeamData;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    /// <summary>
    /// Team authorization rules expressed as composable queries, so the same logic runs against
    /// EF (SQL) and against in-memory lists in unit tests.
    /// A privilege is on when the membership has an override row that grants it, or when it has no
    /// override and the role default grants it.
    /// </summary>
    public static class WorkTeamAccessRules
    {
        public static WorkTeamPrivilege PrivilegeFor(WorkTeamCapability cap)
        {
            switch (cap)
            {
                case WorkTeamCapability.Timesheets:
                    return WorkTeamPrivilege.TeamTimesheets;
                case WorkTeamCapability.Scorecards:
                    return WorkTeamPrivilege.TeamScorecards;
                case WorkTeamCapability.Rates:
                    return WorkTeamPrivilege.ManageRates;
                case WorkTeamCapability.TeamAllocations:
                case WorkTeamCapability.ManageTeam:
                    return WorkTeamPrivilege.ManageMembers;
                default:
                    throw new ArgumentOutOfRangeException("cap");
            }
        }

        /// <summary>Memberships whose effective <paramref name="privilege"/> is granted.</summary>
        public static IQueryable<WorkTeamMember> WithPrivilege(IQueryable<WorkTeamMember> members,
            IQueryable<WorkTeamRolePrivilege> defaults, IQueryable<WorkTeamMemberPrivilege> overrides,
            WorkTeamPrivilege privilege)
        {
            return members.Where(m =>
                overrides.Any(o => o.WorkTeamMemberId == m.Id && o.Privilege == privilege && o.IsGranted)
                || (!overrides.Any(o => o.WorkTeamMemberId == m.Id && o.Privilege == privilege)
                    && defaults.Any(d => d.Role == m.Role && d.Privilege == privilege && d.IsGranted)));
        }

        /// <summary>The actor's role rows (in active teams, current on <paramref name="today"/>) that grant <paramref name="cap"/>.</summary>
        public static IQueryable<WorkTeamMember> QualifyingActorRows(IQueryable<WorkTeamMember> members,
            IQueryable<WorkTeamRolePrivilege> defaults, IQueryable<WorkTeamMemberPrivilege> overrides,
            Guid actorId, WorkTeamCapability cap, DateTime today)
        {
            var day = today.Date;
            var current = members.Where(m => m.UserAccountId == actorId
                                             && m.WorkTeam.IsActive
                                             && m.StartDate <= day
                                             && (m.EndDate == null || m.EndDate >= day));
            return WithPrivilege(current, defaults, overrides, PrivilegeFor(cap));
        }

        /// <summary>
        /// Role rows of people the actor can reach for <paramref name="cap"/> whose own period overlaps
        /// [<paramref name="from"/>, <paramref name="to"/>]. A Manager reaches Members and Leads. A Lead or a
        /// Member who holds the privilege reaches Members. Nobody reaches themselves.
        /// </summary>
        public static IQueryable<WorkTeamMember> ReachableRows(IQueryable<WorkTeamMember> members,
            IQueryable<WorkTeamRolePrivilege> defaults, IQueryable<WorkTeamMemberPrivilege> overrides,
            Guid actorId, WorkTeamCapability cap, DateTime today, DateTime from, DateTime to)
        {
            var fromDay = from.Date;
            var toDay = to.Date;
            var actorRows = QualifyingActorRows(members, defaults, overrides, actorId, cap, today);

            return from a in actorRows
                   join t in members on a.WorkTeamId equals t.WorkTeamId
                   where t.UserAccountId != actorId
                         && ((a.Role == WorkTeamRoleType.Manager
                              && (t.Role == WorkTeamRoleType.Member || t.Role == WorkTeamRoleType.Lead))
                             || (a.Role != WorkTeamRoleType.Manager && t.Role == WorkTeamRoleType.Member))
                         && t.StartDate <= toDay
                         && (t.EndDate == null || t.EndDate >= fromDay)
                   select t;
        }

        public static IQueryable<Guid> ReachableUserIds(IQueryable<WorkTeamMember> members,
            IQueryable<WorkTeamRolePrivilege> defaults, IQueryable<WorkTeamMemberPrivilege> overrides,
            Guid actorId, WorkTeamCapability cap, DateTime today, DateTime from, DateTime to)
        {
            return ReachableRows(members, defaults, overrides, actorId, cap, today, from, to)
                .Select(t => t.UserAccountId)
                .Distinct();
        }

        /// <summary>Teams through which the actor reaches <paramref name="targetUserId"/> for <paramref name="cap"/>.</summary>
        public static IQueryable<Guid> ReachingTeamIds(IQueryable<WorkTeamMember> members,
            IQueryable<WorkTeamRolePrivilege> defaults, IQueryable<WorkTeamMemberPrivilege> overrides,
            Guid actorId, Guid targetUserId, WorkTeamCapability cap, DateTime today, DateTime from, DateTime to)
        {
            return ReachableRows(members, defaults, overrides, actorId, cap, today, from, to)
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
