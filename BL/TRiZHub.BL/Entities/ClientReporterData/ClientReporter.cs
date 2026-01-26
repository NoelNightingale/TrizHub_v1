using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.SecurityData;


namespace TRiZHub.BL.Entities.ClientReporterData
{
    [Table("ClientReporter")]
    public class ClientReporter : DbEntity
    {
        public virtual Guid ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual ClientEntity Client { get; set; }

        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserIdentity UserIdentity { get; set; }
    }
}
