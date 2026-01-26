#region Usings

using System;

#endregion

namespace TRiZHub.Models.BillingCycleModels
{
    public class BillingCycleModel
    {

       
        public Guid? Id { get; set; }

        public short Cycle { get; set; }
        public short Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public short Weekdays { get; set; }
        public short PublicHolidays { get; set; }
        public virtual short WorkDays { get; set; }
        public Guid CreatedByAccountId { get; set; }
        public DateTime DateCreated { get ; set; }

        public bool IsClosed { get; set; }
        public bool IsActive { get; set; }
    }
}