#region Usings

using System;
using System.Collections.Generic;

#endregion Usings

namespace TRiZHub.BL.Provider.ReportData
{
    public interface IReportProvider : ITRiZHubProvider
    {
        #region Timesheets

        byte[] GenerateTimesheetSummaryClientReporter(DateTime startDate, DateTime endDate,
                String userAccounts, String clients, String projects, String projectWildCardSearch, bool showPhases);

        byte[] GenerateTimesheetSummaryOld(DateTime startDate, DateTime endDate,
            String userAccounts, String clients, String projects, String projectWildCardSearch, String employers,
            bool showBillingCycle, bool showRates, bool showPhases, bool showOnlyBillbale = false);

        byte[] GenerateTimesheetSummary(DateTime startDate, DateTime endDate,
            String userAccounts, String clients, String projects, String projectWildCardSearch, String employers, bool showUnassigned,
            bool showBillingCycle, bool showRates, bool showPhases, bool showOnlyBillbale = false);

        byte[] GenerateTimesheetDetailClientReporter(DateTime startDate, DateTime endDate, String clients, String projects,
            String userAccounts, bool showPhases);

        byte[] GenerateTimesheetDetailOld(DateTime startDate, DateTime endDate, String projects,
            String userAccountId, String clients, String employers, bool showRates, bool showPhases);

        byte[] GenerateTimesheetDetail(DateTime startDate, DateTime endDate, String projects,
            String userAccountId, String clients, String employers, bool showUnassigned, bool showRates, bool showPhases);

        byte[] GenerateBillingReportClientReporter(DateTime startDate, DateTime endDate, String clients, List<Guid> projectIds, List<Guid> employerIds, bool showUnassigned, bool showRates);

        byte[] GenerateBillingReport(DateTime startDate, DateTime endDate, List<Guid> projectIds, List<Guid> employerIds, bool showUnassigned, bool showRates);

        #endregion Timesheets

        #region Scorecards

        //byte[] GenerateScorecardEmployeeSummary(Guid scorecardTemplateId, Guid[] scorecardTemplatePeriodsIds,
        //    Guid employeeId, Int32 scoreCardStatus, out string outFileName);

        byte[] GenerateScorecardStatusSummary(bool searchAllYears, string[] reviewYears, bool searchAllPeriods, Guid[] reviewPeriods, int submitted, int locked, int employeeHasScorecard, Guid[] employees, Guid[] clients, Guid[] lineManagers, Guid[] evaluators, Guid[] scorecards, out string fileName);

        byte[] GenerateScorecardFinalCombined(bool searchAllYears, string[] reviewYears, bool searchAllPeriods,
            Guid[] scorecardTemplatePeriodsIds, int detailLevel, int scoreCardStatus, int locked,
            Guid[] employees, Guid[] clients, Guid[] lineManagers, Guid[] evaluators, Guid scorecardTemplateId,
            out string fileName);

        //        byte[] GenerateScorecardFinalCombined(bool searchAllYears, string[] reviewYears, bool searchAllPeriods, Guid[] reviewPeriods, int detailLevel, int submitted, int locked, Guid[] employees, Guid[] clients, Guid[] lineManagers, Guid[] evaluators, Guid[] scorecards, out string fileName);
        //        byte[] GenerateScorecardDetail(bool searchAllYears, string[] reviewYears, bool searchAllPeriods, Guid[] reviewPeriods, int detailLevel, int submitted, int locked, int employeeHasScorecard, Guid[] employees, Guid[] clients, Guid[] lineManagers, Guid[] evaluators, Guid[] scorecards, out string fileName);

        #endregion Scorecards

        #region User

        byte[] GenrateUserSummary(Guid? userID, bool showInactive);

        byte[] GenerateProjectAllocation(string userAccounts, bool onlyActiveUsers, bool onlyActiveClients, bool onlyActiveProjects, bool onlyActiveSubProjects);
//        byte[] GenerateProjectAllocationCSV(string userAccounts, bool onlyActiveUsers, bool onlyActiveClients, bool onlyActiveProjects, bool onlyActiveSubProjects);
        byte[] GenerateRoleAllocation(List<Guid> userAccounts, bool includeInactiveRoles, bool includeInactiveUsers);

        #endregion User

        byte[] GenerateUserAssetRegisterSummary();
    }
}