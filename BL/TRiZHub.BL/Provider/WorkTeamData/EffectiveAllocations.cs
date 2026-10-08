#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    /// <summary>One client or project allocation a user receives through a team, valid while the membership is.</summary>
    public class TeamAllocationGrant
    {
        public Guid WorkTeamId { get; set; }
        public string WorkTeamName { get; set; }

        /// <summary>Set for a whole-client allocation.</summary>
        public Guid? ClientId { get; set; }

        /// <summary>Set for a project or subproject allocation.</summary>
        public Guid? ProjectId { get; set; }

        /// <summary>Set for a subproject allocation; null with a ProjectId means the whole project.</summary>
        public Guid? SubProjectId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool CoversDate(DateTime date)
        {
            var day = date.Date;
            return StartDate <= day && (EndDate == null || EndDate.Value >= day);
        }
    }

    /// <summary>
    /// A user's allocations for a date range: direct UserIdentityClient/UserIdentityProject rows (undated) plus team
    /// allocations bounded by the user's membership periods. Pass a date to test one day; pass null for "any day in the
    /// loaded range".
    /// </summary>
    public class EffectiveAllocations
    {
        public EffectiveAllocations()
        {
            DirectClientIds = new HashSet<Guid>();
            DirectProjectIds = new HashSet<Guid>();
            DirectSubProjectIds = new HashSet<Guid>();
            TeamGrants = new List<TeamAllocationGrant>();
        }

        public HashSet<Guid> DirectClientIds { get; private set; }

        /// <summary>Whole-project direct allocations.</summary>
        public HashSet<Guid> DirectProjectIds { get; private set; }

        public HashSet<Guid> DirectSubProjectIds { get; private set; }

        public List<TeamAllocationGrant> TeamGrants { get; private set; }

        private IEnumerable<TeamAllocationGrant> GrantsOn(DateTime? date)
        {
            return date.HasValue ? TeamGrants.Where(g => g.CoversDate(date.Value)) : TeamGrants;
        }

        public bool ClientCovered(Guid clientId, DateTime? date = null)
        {
            return DirectClientIds.Contains(clientId)
                   || GrantsOn(date).Any(g => g.ClientId == clientId);
        }

        /// <summary>The whole project is allocated (directly, or through a team), not only some subprojects.</summary>
        public bool WholeProjectCovered(Guid projectId, DateTime? date = null)
        {
            return DirectProjectIds.Contains(projectId)
                   || GrantsOn(date).Any(g => g.ProjectId == projectId && g.SubProjectId == null);
        }

        public bool SubProjectCovered(Guid subProjectId, DateTime? date = null)
        {
            return DirectSubProjectIds.Contains(subProjectId)
                   || GrantsOn(date).Any(g => g.SubProjectId == subProjectId);
        }

        /// <summary>
        /// May the user log time against this project line on the date? A project line without a subproject needs the
        /// client or the whole project; a subproject line is also allowed by the subproject itself.
        /// </summary>
        public bool Allows(Guid clientId, Guid projectId, Guid? subProjectId, DateTime? date)
        {
            if (ClientCovered(clientId, date) || WholeProjectCovered(projectId, date))
                return true;
            return subProjectId.HasValue && SubProjectCovered(subProjectId.Value, date);
        }

        public bool HasDirect(Guid clientId, Guid projectId, Guid? subProjectId)
        {
            return DirectClientIds.Contains(clientId)
                   || DirectProjectIds.Contains(projectId)
                   || (subProjectId.HasValue && DirectSubProjectIds.Contains(subProjectId.Value));
        }

        /// <summary>Names of the teams that allocate the item in the loaded range, for "via team" display.</summary>
        public List<string> TeamNamesFor(Guid? clientId, Guid? projectId, Guid? subProjectId)
        {
            return TeamGrants
                .Where(g => (clientId.HasValue && g.ClientId == clientId)
                            || (projectId.HasValue && g.ProjectId == projectId && g.SubProjectId == subProjectId))
                .Select(g => g.WorkTeamName)
                .Distinct()
                .OrderBy(n => n)
                .ToList();
        }
    }
}
