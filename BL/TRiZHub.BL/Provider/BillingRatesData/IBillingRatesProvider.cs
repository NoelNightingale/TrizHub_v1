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
        IQueryable<BillingRates> BillingRatesFilterList(Guid? userAccountId, Guid? clientId, Guid? projectId,
            string scope = null, DateTime? activeOn = null,
            IList<Guid> userAccountIds = null, IList<Guid> clientIds = null, IList<Guid> projectIds = null);

        BillingRates SaveBillingRates(Guid? id, Guid userAccountId, decimal rate, DateTime startDate,
            DateTime endDate, Guid? clientId, Guid? projectId);

        BillingRates GetBillingRates(Guid id);

        void DeleteBillingRatesEntry(Guid id);

        ProjectTeamRatesResult GetProjectTeamRates(Guid projectId, DateTime asOfDate);

        UserRatesForProjectContextResult GetUserRatesForProjectContext(Guid userAccountId, Guid projectId);

        ClientTeamRatesResult GetClientTeamRates(Guid clientId, DateTime asOfDate);

        UserRatesForClientContextResult GetUserRatesForClientContext(Guid userAccountId, Guid clientId);

        UserRatesAsOfResult GetUserRatesAsOf(Guid userAccountId, DateTime asOfDate);

        BillingRatesFilterOptionsResult GetFilterOptions(IList<Guid> userAccountIds, IList<Guid> clientIds,
            IList<Guid> projectIds);

        /// <summary>
        /// Effective rates as of a date for the selected users/clients/projects
        /// (Project → Client → Default cascade per context).
        /// </summary>
        List<BillingRatesEffectiveRow> GetEffectiveRates(IList<Guid> userAccountIds, IList<Guid> clientIds,
            IList<Guid> projectIds, DateTime asOf);

        /// <summary>
        /// Excel export of periods or effective rates for the current filter selection (no paging).
        /// </summary>
        byte[] ExportBillingRatesExcel(IList<Guid> userAccountIds, IList<Guid> clientIds, IList<Guid> projectIds,
            string scope, DateTime? activeOn, string resultMode);
    }
}
