#region Usings

using System;

#endregion

namespace TRiZHub.Models.BillingCycleModels
{
    public class BillingCycleExemptUserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}
