#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ClientEntityData;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    [Table("WorkGroupClient")]
    public class WorkGroupClient : DbEntity
    {
        [Index("UIDX_WorkGroupClient", IsUnique = true, Order = 0)]
        public virtual Guid WorkGroupId { get; set; }

        [ForeignKey("WorkGroupId")]
        public virtual WorkGroup WorkGroup { get; set; }

        [Index("UIDX_WorkGroupClient", IsUnique = true, Order = 1)]
        public virtual Guid ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual ClientEntity Client { get; set; }
    }
}
