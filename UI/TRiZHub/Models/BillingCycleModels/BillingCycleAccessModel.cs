#region Usings

using System;

#endregion

namespace TRiZHub.Models.BillingCycleModels
{
    /// <summary>Whether a user's timesheet may be edited within a billing cycle.</summary>
    public class BillingCycleAccessModel
    {
        public Guid BillingCycleId { get; set; }
        public Guid UserAccountId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsClosed { get; set; }
        public bool IsExempt { get; set; }
        public bool IsReadOnly { get; set; }
        public string Message { get; set; }
    }
}
