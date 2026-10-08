#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ClientEntityData;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    [Table("WorkTeamClient")]
    public class WorkTeamClient : DbEntity
    {
        [Index("UIDX_WorkTeamClient", IsUnique = true, Order = 0)]
        public virtual Guid WorkTeamId { get; set; }

        [ForeignKey("WorkTeamId")]
        public virtual WorkTeam WorkTeam { get; set; }

        [Index("UIDX_WorkTeamClient", IsUnique = true, Order = 1)]
        public virtual Guid ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual ClientEntity Client { get; set; }
    }
}
