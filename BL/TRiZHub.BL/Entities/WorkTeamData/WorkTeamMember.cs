#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    /// <summary>A lead or a member of one work team. A person holds one role on a team.</summary>
    [Table("WorkTeamMember")]
    public class WorkTeamMember : DbEntity
    {
        [Index("UIDX_WorkTeamMember", IsUnique = true, Order = 0)]
        public virtual Guid WorkTeamId { get; set; }

        [ForeignKey("WorkTeamId")]
        public virtual WorkTeam WorkTeam { get; set; }

        [Index("UIDX_WorkTeamMember", IsUnique = true, Order = 1)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        public virtual WorkTeamRoleType Role { get; set; }

        /// <summary>Metadata only for now. No uniqueness across teams.</summary>
        public virtual bool IsHomeTeam { get; set; }

        public virtual ICollection<WorkTeamMemberCapability> Capabilities { get; set; }
    }
}
