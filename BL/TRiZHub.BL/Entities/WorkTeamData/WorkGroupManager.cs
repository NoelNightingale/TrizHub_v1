#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    [Table("WorkGroupManager")]
    public class WorkGroupManager : DbEntity
    {
        [Index("UIDX_WorkGroupManager", IsUnique = true, Order = 0)]
        public virtual Guid WorkGroupId { get; set; }

        [ForeignKey("WorkGroupId")]
        public virtual WorkGroup WorkGroup { get; set; }

        [Index("UIDX_WorkGroupManager", IsUnique = true, Order = 1)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        public virtual ICollection<WorkGroupManagerCapability> Capabilities { get; set; }
    }
}
