#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.BillingRatesData;

#endregion

namespace TRiZHub.BL.Provider.BillingRatesData
{
    public interface IBillingRatesProvider : ITRiZHubProvider
    {
        IQueryable<BillingRates> BillingRatesFilterList(Guid userAccountId);

        BillingRates SaveBillingRates(Guid? id, Guid userAccountId, decimal rate, DateTime startDate,
            DateTime endDate);

        BillingRates GetBillingRates(Guid id);

        void DeleteBillingRatesEntry(Guid id);
    }
}