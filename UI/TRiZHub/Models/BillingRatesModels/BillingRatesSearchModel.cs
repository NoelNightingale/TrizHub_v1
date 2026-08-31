#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TRiZHub.Models.BillingRatesModels
{
    public class BillingRatesSearchModel : GridModel
    {
        public Guid? UserAccountId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// Optional multi-select (standalone maintenance). When non-empty, takes precedence over singular UserAccountId.
        /// </summary>
        public List<Guid> UserAccountIds { get; set; }

        /// <summary>
        /// Optional multi-select (standalone maintenance). When non-empty, takes precedence over singular ClientId.
        /// </summary>
        public List<Guid> ClientIds { get; set; }

        /// <summary>
        /// Optional multi-select (standalone maintenance). When non-empty, takes precedence over singular ProjectId.
        /// </summary>
        public List<Guid> ProjectIds { get; set; }

        /// <summary>
        /// Optional scope filter: null/empty = all, or "Default" / "Client" / "Project".
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// When set, only rates whose period covers this date (StartDate ≤ ActiveOn ≤ EndDate).
        /// </summary>
        public DateTime? ActiveOn { get; set; }
    }

    public class BillingRatesFilterOptionsRequest
    {
        public List<Guid> UserAccountIds { get; set; }
        public List<Guid> ClientIds { get; set; }
        public List<Guid> ProjectIds { get; set; }
    }

    public class BillingRatesFilterOptionModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class BillingRatesFilterOptionsModel
    {
        public List<BillingRatesFilterOptionModel> Users { get; set; }
        public List<BillingRatesFilterOptionModel> Clients { get; set; }
        public List<BillingRatesFilterOptionModel> Projects { get; set; }
    }
}
