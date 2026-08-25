#region Usings

using System;

#endregion

namespace TRiZHub.Models.BillingRatesModels
{
    public class BillingRatesSearchModel : GridModel
    {
        public Guid? UserAccountId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
