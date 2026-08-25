#region Usings

using System;
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

        #endregion
    }
}
