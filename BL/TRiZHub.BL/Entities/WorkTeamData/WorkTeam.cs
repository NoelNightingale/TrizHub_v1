#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.TeamData;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    /// <summary>
    /// One team type inside a work group. The label is the group name plus the team type name.
    /// A group can have many teams, and only one of each team type.
    /// </summary>
    [Table("WorkTeam")]
    public class WorkTeam : DbEntity
    {
        [Index("UIDX_WorkTeamGroupType", IsUnique = true, Order = 0)]
        public virtual Guid WorkGroupId { get; set; }

        [ForeignKey("WorkGroupId")]
        public virtual WorkGroup WorkGroup { get; set; }

        [Index("UIDX_WorkTeamGroupType", IsUnique = true, Order = 1)]
        public virtual Guid TeamTypeId { get; set; }

        [ForeignKey("TeamTypeId")]
        public virtual Team TeamType { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual ICollection<WorkTeamMember> Members { get; set; }
    }
}
