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

        public IQueryable<BillingRates> BillingRatesFilterList(Guid userAccountId)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);
            return DataContext.BillingRatesSet.Where(a => a.UserAccountId == userAccountId);
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
            DateTime endDate)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            if (id == Guid.Empty)
                id = null;


            if (startDate.Date >= endDate.Date)
            throw new BillingRatesException("Selected End Date is bofore or on selected Start Date!");


            //check if billing rates overlapp 
            var billingRates = BillingRatesFilterList(userAccountId);

           foreach (var a in billingRates)
            {
                if (a.Id == id)
                {
                    continue;
                }

                if (startDate >= a.StartDate.Date && startDate.Date <= a.EndDate.Date ||
                            endDate.Date >= a.StartDate.Date && endDate.Date <= a.EndDate.Date ||
                            a.StartDate.Date >= startDate.Date && a.StartDate.Date <= endDate.Date ||
                            a.EndDate.Date >= startDate.Date && a.EndDate.Date <= startDate.Date)
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
            record.Rate = rate;
            record.StartDate = DateExtensions.ChangeTime(startDate, 0,0,0,0);
            record.EndDate = DateExtensions.ChangeTime(endDate, 0, 0, 0, 0);

            DataContextSaveChanges();

            return record;
        }

        #endregion
    }    
}