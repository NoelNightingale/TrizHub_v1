#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TRiZHub.BL.Provider.BillingRatesData
{
    public class ProjectTeamRateRow
    {
        public Guid UserAccountId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AccountName { get; set; }
        public decimal? ProjectRate { get; set; }
        public decimal? ClientRate { get; set; }
        public decimal? DefaultRate { get; set; }
        public decimal? EffectiveRate { get; set; }
        public string EffectiveScope { get; set; }
    }

    public class ProjectTeamRatesResult
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime AsOfDate { get; set; }
        public List<ProjectTeamRateRow> Team { get; set; }
    }

    public class UserRatesForProjectContextResult
    {
        public Guid UserAccountId { get; set; }
        public string UserName { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public List<Entities.BillingRatesData.BillingRates> ProjectRates { get; set; }
        public List<Entities.BillingRatesData.BillingRates> ClientRates { get; set; }
        public List<Entities.BillingRatesData.BillingRates> DefaultRates { get; set; }
    }
}
