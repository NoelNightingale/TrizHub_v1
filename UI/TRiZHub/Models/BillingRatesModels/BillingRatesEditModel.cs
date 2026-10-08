#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.BillingRatesModels
{
    public class BillingRatesEditModel
    {
        public Guid? Id { get; set; }

        public Guid UserAccountId { get; set; }

        public Guid? ClientId { get; set; }

        public Guid? ProjectId { get; set; }

        [Required]
        public decimal Rate { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // Read-only display fields; ignored on save.
        public string ClientName { get; set; }
        public string ProjectName { get; set; }
        public Guid? ProjectClientId { get; set; }
        public string ProjectClientName { get; set; }
        public bool IsLocked { get; set; }
        public string LockReason { get; set; }
    }
}
