#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.BillingCycleData
{
    [Table("BillingCycleEntry")]
    public class BillingCycleEntry : DbEntity
    {
        [Required]
        public virtual short Cycle { get; set; }

        [Required]
        public virtual short Year { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual short Weekdays { get; set; }

        public virtual short PublicHolidays { get; set; }

        public virtual short WorkDays { get; set; }

        public virtual Guid CreatedByAccountId { get; set; }

        [ForeignKey("CreatedByAccountId")]
        public virtual UserAccount CreatedByAccount { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual bool IsClosed { get; set; }

        public virtual bool IsActive { get; set; }
    }
}