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

    public class ClientTeamRateRow
    {
        public Guid UserAccountId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AccountName { get; set; }
        public decimal? ClientRate { get; set; }
        public decimal? DefaultRate { get; set; }
        public decimal? EffectiveRate { get; set; }
        public string EffectiveScope { get; set; }
        public int ProjectOverrideCount { get; set; }
    }

    public class ClientTeamRatesResult
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime AsOfDate { get; set; }
        public List<ClientTeamRateRow> Team { get; set; }
    }

    public class ClientProjectRateGroup
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<Entities.BillingRatesData.BillingRates> Rates { get; set; }
    }

    public class ClientProjectOption
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
    }

    public class UserRatesForClientContextResult
    {
        public Guid UserAccountId { get; set; }
        public string UserName { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public List<Entities.BillingRatesData.BillingRates> ClientRates { get; set; }
        public List<Entities.BillingRatesData.BillingRates> DefaultRates { get; set; }
        public List<ClientProjectRateGroup> ProjectRateGroups { get; set; }
        public List<ClientProjectOption> ClientProjects { get; set; }
    }

    public class UserRatesAsOfProjectRow
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal? ProjectRate { get; set; }
        public Guid? ProjectRateId { get; set; }
        public decimal? EffectiveRate { get; set; }
        public string EffectiveScope { get; set; }
        public bool IsInactive { get; set; }
        public bool IsLocked { get; set; }
    }

    public class UserRatesAsOfClientRow
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public decimal? ClientRate { get; set; }
        public Guid? ClientRateId { get; set; }
        public decimal? EffectiveRate { get; set; }
        public string EffectiveScope { get; set; }
        public bool IsInactive { get; set; }
        public List<UserRatesAsOfProjectRow> Projects { get; set; }
    }

    public class UserRatesAsOfResult
    {
        public Guid UserAccountId { get; set; }
        public string UserName { get; set; }
        public DateTime AsOfDate { get; set; }
        public decimal? DefaultRate { get; set; }
        public Guid? DefaultRateId { get; set; }
        public List<UserRatesAsOfClientRow> Clients { get; set; }
    }

    public class BillingRatesFilterOption
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class BillingRatesFilterOptionsResult
    {
        public List<BillingRatesFilterOption> Users { get; set; }
        public List<BillingRatesFilterOption> Clients { get; set; }
        public List<BillingRatesFilterOption> Projects { get; set; }
    }

    /// <summary>
    /// One effective-rate row for the standalone Billing Rates workbench (as of Active On).
    /// </summary>
    public class BillingRatesEffectiveRow
    {
        public Guid UserAccountId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AccountName { get; set; }
        public string UserName { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; }
        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal? EffectiveRate { get; set; }
        public string EffectiveScope { get; set; }
        public Guid? RateId { get; set; }
    }

    /// <summary>One person's Client and Project rates within a work team's allocations.</summary>
    public class WorkTeamMemberRatesResult
    {
        public Guid WorkTeamId { get; set; }
        public string WorkTeamName { get; set; }
        public Guid UserAccountId { get; set; }
        public string UserName { get; set; }
        public List<WorkTeamMemberRateRow> Rates { get; set; }

        /// <summary>Clients a Client-scope rate may be added for.</summary>
        public List<BillingRatesFilterOption> Clients { get; set; }

        /// <summary>Projects a Project-scope rate may be added for.</summary>
        public List<WorkTeamRateProjectOption> Projects { get; set; }
    }

    public class WorkTeamMemberRateRow
    {
        public Guid Id { get; set; }
        public string Scope { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; }
        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal Rate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsLocked { get; set; }
        public bool CanEdit { get; set; }
    }

    public class WorkTeamRateProjectOption
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
