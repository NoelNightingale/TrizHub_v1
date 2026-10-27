#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    [Table("WorkGroupManagerCapability")]
    public class WorkGroupManagerCapability : DbEntity
    {
        [Index("UIDX_WorkGroupManagerCapability", IsUnique = true, Order = 0)]
        public virtual Guid WorkGroupManagerId { get; set; }

        [ForeignKey("WorkGroupManagerId")]
        public virtual WorkGroupManager WorkGroupManager { get; set; }

        [Index("UIDX_WorkGroupManagerCapability", IsUnique = true, Order = 1)]
        public virtual WorkAssignmentCapability Capability { get; set; }

        public virtual bool IsAllowed { get; set; }
    }
}
