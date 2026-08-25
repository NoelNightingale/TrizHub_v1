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

    public class ClientTeamRateRowModel
    {
        public Guid UserAccountId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AccountName { get; set; }
        public string UserName { get; set; }
        public decimal? ClientRate { get; set; }
        public decimal? DefaultRate { get; set; }
        public decimal? EffectiveRate { get; set; }
        public string EffectiveScope { get; set; }
        public int ProjectOverrideCount { get; set; }
    }

    public class ClientTeamRatesModel
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime AsOfDate { get; set; }
        public List<ClientTeamRateRowModel> Team { get; set; }
    }

    public class ClientTeamRatesRequest
    {
        public Guid ClientId { get; set; }
        public DateTime AsOfDate { get; set; }
    }

    public class ClientProjectRateGroupModel
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<BillingRatesGridModel> Rates { get; set; }
    }

    public class ClientProjectOptionModel
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
    }

    public class UserRatesForClientContextModel
    {
        public Guid UserAccountId { get; set; }
        public string UserName { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public List<BillingRatesGridModel> ClientRates { get; set; }
        public List<BillingRatesGridModel> DefaultRates { get; set; }
        public List<ClientProjectRateGroupModel> ProjectRateGroups { get; set; }
        public List<ClientProjectOptionModel> ClientProjects { get; set; }
    }

    public class UserRatesAsOfRequest
    {
        public Guid UserAccountId { get; set; }
        public DateTime AsOfDate { get; set; }
    }

    public class UserRatesAsOfProjectRowModel
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal? ProjectRate { get; set; }
        public Guid? ProjectRateId { get; set; }
        public decimal? EffectiveRate { get; set; }
        public string EffectiveScope { get; set; }
    }

    public class UserRatesAsOfClientRowModel
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public decimal? ClientRate { get; set; }
        public Guid? ClientRateId { get; set; }
        public decimal? EffectiveRate { get; set; }
        public string EffectiveScope { get; set; }
        public List<UserRatesAsOfProjectRowModel> Projects { get; set; }
    }

    public class UserRatesAsOfModel
    {
        public Guid UserAccountId { get; set; }
        public string UserName { get; set; }
        public DateTime AsOfDate { get; set; }
        public decimal? DefaultRate { get; set; }
        public Guid? DefaultRateId { get; set; }
        public List<UserRatesAsOfClientRowModel> Clients { get; set; }
    }
}
