#region Usings

using System;

#endregion

namespace TRiZHub.Models.BillingRatesModels
{
    public class BillingRatesGridModel
    {
        public Guid Id { get; set; }
        public Guid UserAccountId { get; set; }
        public string Account { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; }
        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Scope { get; set; }
        public decimal Rate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
