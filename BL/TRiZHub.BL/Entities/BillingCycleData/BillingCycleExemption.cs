#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.BillingCycleData
{
    /// <summary>
    /// A user who may still capture time in a billing cycle while it is closed.
    /// </summary>
    [Table("BillingCycleExemption")]
    public class BillingCycleExemption : DbEntity
    {
        [Index("UIDX_BillingCycleExemption", IsUnique = true, Order = 0)]
        public virtual Guid BillingCycleEntryId { get; set; }

        [ForeignKey("BillingCycleEntryId")]
        public virtual BillingCycleEntry BillingCycleEntry { get; set; }

        [Index("UIDX_BillingCycleExemption", IsUnique = true, Order = 1)]
        [Index("IDX_BillingCycleExemptionUser")]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        public virtual Guid CreatedByAccountId { get; set; }

        [ForeignKey("CreatedByAccountId")]
        public virtual UserAccount CreatedByAccount { get; set; }

        public virtual DateTime DateCreated { get; set; }
    }
}
