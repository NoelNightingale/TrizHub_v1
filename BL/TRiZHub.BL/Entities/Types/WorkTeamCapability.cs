#region Usings

using System;

#endregion

namespace TRiZHub.BL.Entities.Types
{
    [Serializable]
    public enum WorkTeamCapability
    {
        Timesheets = 0,
        Scorecards = 1,
        TeamAllocations = 2,
        Rates = 3,
        ManageTeam = 4
    }
}
