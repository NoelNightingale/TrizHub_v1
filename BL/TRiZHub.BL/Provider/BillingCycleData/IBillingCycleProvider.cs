#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.BillingCycleData;
using TRiZHub.BL.Entities.SecurityData;

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

        BillingCycleEntry SetClosed(Guid id, bool isClosed);

        #endregion

        #region Exemptions

        /// <summary>The active, closed cycle covering <paramref name="day"/> that the user is not exempt from; null when capture is allowed.</summary>
        BillingCycleEntry ClosingCycleFor(Guid userAccountId, DateTime day);

        bool IsUserExempt(Guid billingCycleId, Guid userAccountId);

        IQueryable<BillingCycleExemption> ExemptionList();

        /// <summary>Replaces the cycle's exemption list with <paramref name="userAccountIds"/>.</summary>
        void SaveExemptions(Guid billingCycleId, IList<Guid> userAccountIds);

        IQueryable<UserAccount> ExemptionCandidates();

        #endregion
    }
}