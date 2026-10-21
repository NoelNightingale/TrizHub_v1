#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    /// <summary>The privilege a role has on every team, unless a membership overrides it.</summary>
    [Table("WorkTeamRolePrivilege")]
    public class WorkTeamRolePrivilege : DbEntity
    {
        [Index("UIDX_WorkTeamRolePrivilege", IsUnique = true, Order = 0)]
        public virtual WorkTeamRoleType Role { get; set; }

        [Index("UIDX_WorkTeamRolePrivilege", IsUnique = true, Order = 1)]
        public virtual WorkTeamPrivilege Privilege { get; set; }

        public virtual bool IsGranted { get; set; }
    }
}
