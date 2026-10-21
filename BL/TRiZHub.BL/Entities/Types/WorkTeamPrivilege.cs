#region Usings

using System;

#endregion

namespace TRiZHub.BL.Entities.Types
{
    /// <summary>
    /// Privileges a work-team membership can hold. Role defaults live in WorkTeamRolePrivilege;
    /// a WorkTeamMemberPrivilege row overrides the default for one membership.
    /// </summary>
    [Serializable]
    public enum WorkTeamPrivilege
    {
        CaptureTime = 0,
        TeamTimesheets = 1,
        TeamScorecards = 2,
        MaintainClient = 3,
        ManageRates = 4,
        ManageMembers = 5
    }
}
