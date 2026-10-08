#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    /// <summary>Clients and projects covered by a set of teams' allocations.</summary>
    public class WorkTeamScope
    {
        public WorkTeamScope()
        {
            ClientIds = new HashSet<Guid>();
            ProjectIds = new HashSet<Guid>();
            SubProjectIds = new HashSet<Guid>();
        }

        /// <summary>Whole-client allocations.</summary>
        public HashSet<Guid> ClientIds { get; private set; }

        /// <summary>Projects allocated directly, as a whole or through one of their subprojects.</summary>
        public HashSet<Guid> ProjectIds { get; private set; }

        public HashSet<Guid> SubProjectIds { get; private set; }

        public bool IsEmpty
        {
            get { return ClientIds.Count == 0 && ProjectIds.Count == 0; }
        }
    }

    public class WorkTeamCapabilitySummary
    {
        public bool IsTeamManager { get; set; }
        public bool IsTeamLead { get; set; }
        public bool Timesheets { get; set; }
        public bool Scorecards { get; set; }
        public bool TeamAllocations { get; set; }
        public bool Rates { get; set; }
        public bool ManageTeam { get; set; }
    }

    /// <summary>What the current user may do on one team's maintenance screen.</summary>
    public class WorkTeamPermissions
    {
        public bool CanView { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; }
        public bool IsLead { get; set; }
        public bool CanEditHeader { get; set; }
        public bool CanEditAdminFlags { get; set; }
        public bool CanEditLeadFlags { get; set; }
        public bool CanManageManagers { get; set; }
        public bool CanManagePeople { get; set; }
        public bool CanEditAllocations { get; set; }
        public bool CanTimesheets { get; set; }
        public bool CanScorecards { get; set; }
        public bool CanRates { get; set; }
    }

    public class WorkTeamTypePeriod
    {
        public Guid WorkTeamId { get; set; }
        public Guid TeamTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
