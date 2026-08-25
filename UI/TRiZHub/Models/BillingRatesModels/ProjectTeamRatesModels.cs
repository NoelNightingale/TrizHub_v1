#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TRiZHub.Models.BillingRatesModels
{
    public class ProjectTeamRateRowModel
    {
        public Guid UserAccountId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AccountName { get; set; }
        public string UserName { get; set; }
        public decimal? ProjectRate { get; set; }
        public decimal? ClientRate { get; set; }
        public decimal? DefaultRate { get; set; }
        public decimal? EffectiveRate { get; set; }
        public string EffectiveScope { get; set; }
    }

    public class ProjectTeamRatesModel
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime AsOfDate { get; set; }
        public List<ProjectTeamRateRowModel> Team { get; set; }
    }

    public class ProjectTeamRatesRequest
    {
        public Guid ProjectId { get; set; }
        public DateTime AsOfDate { get; set; }
    }

    public class UserRatesForProjectContextModel
    {
        public Guid UserAccountId { get; set; }
        public string UserName { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public List<BillingRatesGridModel> ProjectRates { get; set; }
        public List<BillingRatesGridModel> ClientRates { get; set; }
        public List<BillingRatesGridModel> DefaultRates { get; set; }
    }
}
