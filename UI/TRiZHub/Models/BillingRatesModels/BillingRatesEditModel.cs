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

        [Required]
        public decimal Rate { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}