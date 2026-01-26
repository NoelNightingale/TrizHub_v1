#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.BillingCycleData;

#endregion

namespace TRiZHub.BL.Provider.BillingCycleData
{
    public interface IBillingCycleProvider : ITRiZHubProvider
    {
        #region Billing Cycle Entry

        BillingCycleEntry SaveBillingCycleEntry(Guid? id, short cycle, short year, DateTime startDate, DateTime endDate,
            short weekdays, short publicHolidays, short workDays, bool isClosed);

        void DeleteBillingCycleEntry(Guid id);

        BillingCycleEntry GetBillingCycleEntry(Guid id);

        IQueryable<BillingCycleEntry> BillingCycleFillterList();

        #endregion
    }
}