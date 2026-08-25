#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.BillingRatesData;

#endregion

namespace TRiZHub.BL.Provider.BillingRatesData
{
    public interface IBillingRatesProvider : ITRiZHubProvider
    {
        IQueryable<BillingRates> BillingRatesFilterList(Guid? userAccountId, Guid? clientId, Guid? projectId);

        BillingRates SaveBillingRates(Guid? id, Guid userAccountId, decimal rate, DateTime startDate,
            DateTime endDate, Guid? clientId, Guid? projectId);

        BillingRates GetBillingRates(Guid id);

        void DeleteBillingRatesEntry(Guid id);

        ProjectTeamRatesResult GetProjectTeamRates(Guid projectId, DateTime asOfDate);

        UserRatesForProjectContextResult GetUserRatesForProjectContext(Guid userAccountId, Guid projectId);
    }
}
