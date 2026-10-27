#region Usings

using System;

#endregion

namespace TRiZHub.BL.Entities.Types
{
    /// <summary>Role on a work team. Group managers live on WorkGroupManager, not here.</summary>
    [Serializable]
    public enum WorkTeamRoleType
    {
        Lead = 0,
        Member = 1
    }
}
