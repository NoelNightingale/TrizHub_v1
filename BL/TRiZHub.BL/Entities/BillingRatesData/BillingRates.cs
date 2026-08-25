#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.BillingRatesData
{
    [Table("BillingRates")]
    public class BillingRates : DbEntity
    {
        [Index("IDX_BillingRatesUserAccount", Order = 0)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        [Index("IDX_BillingRatesClient", Order = 0)]
        public virtual Guid? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual ClientEntity Client { get; set; }

        [Index("IDX_BillingRatesProject", Order = 0)]
        public virtual Guid? ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [Required]
        public virtual decimal Rate { get; set; }

        [Required]
        public virtual DateTime StartDate { get; set; }

        [Required]
        public virtual DateTime EndDate { get; set; }

    }
}
