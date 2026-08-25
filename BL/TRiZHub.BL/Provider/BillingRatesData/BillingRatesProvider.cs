#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.BillingRatesData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Extensions;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.BillingRatesData
{
    public class BillingRatesProvider : TRiZHubProvider, IBillingRatesProvider
    {
        #region Constructor

        public BillingRatesProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        #region Billing Rates

        public IQueryable<BillingRates> BillingRatesFilterList(Guid? userAccountId, Guid? clientId, Guid? projectId)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var query = DataContext.BillingRatesSet.AsQueryable();

            if (userAccountId.HasValue && userAccountId.Value != Guid.Empty)
                query = query.Where(a => a.UserAccountId == userAccountId.Value);

            if (clientId.HasValue && clientId.Value != Guid.Empty)
                query = query.Where(a => a.ClientId == clientId.Value && a.ProjectId == null);

            if (projectId.HasValue && projectId.Value != Guid.Empty)
                query = query.Where(a => a.ProjectId == projectId.Value && a.ClientId == null);

            return query;
        }

        public void DeleteBillingRatesEntry(Guid id)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var record = GetBillingRates(id);

            if (record != null)
            {
                DataContext.BillingRatesSet.Remove(record);
                DataContext.SaveChanges();
            }
        }

        public BillingRates GetBillingRates(Guid id)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);
            return DataContext.BillingRatesSet.FirstOrDefault(a => a.Id == id);
        }

        public BillingRates SaveBillingRates(Guid? id, Guid userAccountId, decimal rate, DateTime startDate,
            DateTime endDate, Guid? clientId, Guid? projectId)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            if (id == Guid.Empty)
                id = null;

            if (userAccountId == Guid.Empty)
                throw new BillingRatesException("User is required!");

            if (clientId == Guid.Empty)
                clientId = null;

            if (projectId == Guid.Empty)
                projectId = null;

            if (clientId.HasValue && projectId.HasValue)
                throw new BillingRatesException("A billing rate cannot be scoped to both a Client and a Project!");

            if (clientId.HasValue)
            {
                var clientExists = DataContext.ClientEntitySet.Any(c => c.Id == clientId.Value);
                if (!clientExists)
                    throw new BillingRatesException("Selected Client was not found!");
            }

            if (projectId.HasValue)
            {
                var projectExists = DataContext.ProjectSet.Any(p => p.Id == projectId.Value);
                if (!projectExists)
                    throw new BillingRatesException("Selected Project was not found!");
            }

            if (startDate.Date >= endDate.Date)
                throw new BillingRatesException("Selected End Date is bofore or on selected Start Date!");

            // Overlap only within the same (User + scope)
            var billingRates = DataContext.BillingRatesSet
                .Where(a => a.UserAccountId == userAccountId);

            if (clientId.HasValue)
                billingRates = billingRates.Where(a => a.ClientId == clientId && a.ProjectId == null);
            else if (projectId.HasValue)
                billingRates = billingRates.Where(a => a.ProjectId == projectId && a.ClientId == null);
            else
                billingRates = billingRates.Where(a => a.ClientId == null && a.ProjectId == null);

            foreach (var a in billingRates)
            {
                if (a.Id == id)
                    continue;

                if (startDate >= a.StartDate.Date && startDate.Date <= a.EndDate.Date ||
                    endDate.Date >= a.StartDate.Date && endDate.Date <= a.EndDate.Date ||
                    a.StartDate.Date >= startDate.Date && a.StartDate.Date <= endDate.Date ||
                    a.EndDate.Date >= startDate.Date && a.EndDate.Date <= endDate.Date)
                {
                    throw new BillingRatesException(
                        "Billing Rates period will overlap with another period!");
                }
            }

            var record = DataContext.BillingRatesSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new BillingRates
                {
                    UserAccountId = userAccountId
                };
                DataContext.BillingRatesSet.Add(record);
            }

            record.UserAccountId = userAccountId;
            record.ClientId = clientId;
            record.ProjectId = projectId;
            record.Rate = rate;
            record.StartDate = DateExtensions.ChangeTime(startDate, 0, 0, 0, 0);
            record.EndDate = DateExtensions.ChangeTime(endDate, 0, 0, 0, 0);

            DataContextSaveChanges();

            return record;
        }

        public ProjectTeamRatesResult GetProjectTeamRates(Guid projectId, DateTime asOfDate)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var project = DataContext.ProjectSet.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
                throw new BillingRatesException("Selected Project was not found!");

            var asOf = asOfDate.Date;
            var clientId = project.ClientId;
            var clientName = project.Client != null
                ? project.Client.EntityName
                : DataContext.ClientEntitySet.Where(c => c.Id == clientId).Select(c => c.EntityName).FirstOrDefault();

            var projectUserIds = DataContext.UserIdentityProjectSet
                .Where(a => a.ProjectId == projectId)
                .Select(a => a.UserAccountId);

            var clientUserIds = DataContext.UserIdentityClientSet
                .Where(a => a.ClientId == clientId)
                .Select(a => a.UserAccountId);

            var teamUserIds = projectUserIds.Union(clientUserIds).Distinct().ToList();

            var users = DataContext.UserAccountSet
                .Where(u => teamUserIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.Surname,
                    u.AccountName
                })
                .OrderBy(u => u.Surname)
                .ThenBy(u => u.FirstName)
                .ToList();

            var userIds = users.Select(u => u.Id).ToList();

            var rates = DataContext.BillingRatesSet
                .Where(r => userIds.Contains(r.UserAccountId)
                            && r.StartDate <= asOf
                            && r.EndDate >= asOf
                            && (
                                (r.ProjectId == projectId && r.ClientId == null)
                                || (r.ClientId == clientId && r.ProjectId == null)
                                || (r.ClientId == null && r.ProjectId == null)
                            ))
                .ToList();

            var team = new List<ProjectTeamRateRow>();
            foreach (var user in users)
            {
                var projectRate = FindRateForDate(rates, user.Id, asOf, null, projectId);
                var clientRate = FindRateForDate(rates, user.Id, asOf, clientId, null);
                var defaultRate = FindRateForDate(rates, user.Id, asOf, null, null);

                decimal? effectiveRate = null;
                string effectiveScope = null;
                if (projectRate.HasValue)
                {
                    effectiveRate = projectRate;
                    effectiveScope = "Project";
                }
                else if (clientRate.HasValue)
                {
                    effectiveRate = clientRate;
                    effectiveScope = "Client";
                }
                else if (defaultRate.HasValue)
                {
                    effectiveRate = defaultRate;
                    effectiveScope = "Default";
                }

                team.Add(new ProjectTeamRateRow
                {
                    UserAccountId = user.Id,
                    FirstName = user.FirstName,
                    Surname = user.Surname,
                    AccountName = user.AccountName,
                    ProjectRate = projectRate,
                    ClientRate = clientRate,
                    DefaultRate = defaultRate,
                    EffectiveRate = effectiveRate,
                    EffectiveScope = effectiveScope
                });
            }

            return new ProjectTeamRatesResult
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                ClientId = clientId,
                ClientName = clientName,
                AsOfDate = asOf,
                Team = team
            };
        }

        public UserRatesForProjectContextResult GetUserRatesForProjectContext(Guid userAccountId, Guid projectId)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var project = DataContext.ProjectSet.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
                throw new BillingRatesException("Selected Project was not found!");

            var user = DataContext.UserAccountSet.FirstOrDefault(u => u.Id == userAccountId);
            if (user == null)
                throw new BillingRatesException("Selected User was not found!");

            var clientId = project.ClientId;
            var clientName = project.Client != null
                ? project.Client.EntityName
                : DataContext.ClientEntitySet.Where(c => c.Id == clientId).Select(c => c.EntityName).FirstOrDefault();

            var projectRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId && r.ProjectId == projectId && r.ClientId == null)
                .OrderBy(r => r.StartDate)
                .ToList();

            var clientRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId && r.ClientId == clientId && r.ProjectId == null)
                .OrderBy(r => r.StartDate)
                .ToList();

            var defaultRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId && r.ClientId == null && r.ProjectId == null)
                .OrderBy(r => r.StartDate)
                .ToList();

            return new UserRatesForProjectContextResult
            {
                UserAccountId = user.Id,
                UserName = (user.FirstName + " " + user.Surname).Trim(),
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                ClientId = clientId,
                ClientName = clientName,
                ProjectRates = projectRates,
                ClientRates = clientRates,
                DefaultRates = defaultRates
            };
        }

        /// <summary>
        /// Finds the rate for a user on a date within a specific scope bucket.
        /// Pass clientId for client scope, projectId for project scope, or neither for default.
        /// </summary>
        private static decimal? FindRateForDate(IEnumerable<BillingRates> rates, Guid userAccountId, DateTime asOf,
            Guid? clientId, Guid? projectId)
        {
            BillingRates match;
            if (projectId.HasValue)
            {
                match = rates.FirstOrDefault(r =>
                    r.UserAccountId == userAccountId
                    && r.ProjectId == projectId
                    && r.ClientId == null
                    && r.StartDate.Date <= asOf
                    && r.EndDate.Date >= asOf);
            }
            else if (clientId.HasValue)
            {
                match = rates.FirstOrDefault(r =>
                    r.UserAccountId == userAccountId
                    && r.ClientId == clientId
                    && r.ProjectId == null
                    && r.StartDate.Date <= asOf
                    && r.EndDate.Date >= asOf);
            }
            else
            {
                match = rates.FirstOrDefault(r =>
                    r.UserAccountId == userAccountId
                    && r.ClientId == null
                    && r.ProjectId == null
                    && r.StartDate.Date <= asOf
                    && r.EndDate.Date >= asOf);
            }

            return match != null ? match.Rate : (decimal?)null;
        }

        #endregion
    }
}
