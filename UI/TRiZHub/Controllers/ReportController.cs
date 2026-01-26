#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.ReportData;
using TRiZHub.Models;
using TRiZHub.Models.ReportModels;

using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRiZHub.BL.Provider.ProjectData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Controllers.Filters;

using System.Net.Http.Headers;

#endregion Usings

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class ReportController : TCRControllerBase
    {
        private IReportProvider ReportProvider { get; }
        private IAppSettings AppSettings { get; }

        #region Constructor

        public ReportController()
        {
            AppSettings = new AppSettings(Context);
            ReportProvider = new ReportProvider(Context, CurrentUser);
        }

        public ReportController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            ReportProvider = new ReportProvider(Context, CurrentUser);
        }

        #endregion Constructor

        [HttpGet]
        public HttpResponseMessage TimesheetSummaryExcelClient(DateTime? startDate, DateTime? endDate, String userAccountId, String clients,
                                String projects, String projectWildCardSearch, bool showPhases)
        {
            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.MinValue.AddYears(1970);
                    startDate.Value.AddYears(1970);
                }
                if (endDate == null)
                {
                    endDate = DateTime.MinValue.AddYears(2070);
                    endDate.Value.AddYears(2170);
                }
                var excel = ReportProvider.GenerateTimesheetSummaryClientReporter(startDate.Value,
                   endDate.Value, userAccountId, clients, projects, projectWildCardSearch, showPhases);

                return generateStreamResponse(excel, "TimesheetSummary- " + DateTime.Now.ToShortDateString() + ".xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public HttpResponseMessage TimesheetSummaryExcel(DateTime? startDate, DateTime? endDate, String userAccountId, String clients,
                                String projects, String projectWildCardSearch, String employers, bool showUnassigned, bool showRates, bool showPhases)
        {
            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.MinValue.AddYears(1970);
                    startDate.Value.AddYears(1970);
                }
                if (endDate == null)
                {
                    endDate = DateTime.MinValue.AddYears(2070);
                    endDate.Value.AddYears(2170);
                }
                if (projectWildCardSearch == null)
                {
                    projectWildCardSearch = "*";
                }
                var excel = ReportProvider.GenerateTimesheetSummary(startDate.Value,
                   endDate.Value, userAccountId, clients, projects, projectWildCardSearch, employers, showUnassigned,
                   true, showRates, showPhases);

                return generateStreamResponse(excel, "TimesheetSummary- " + DateTime.Now.ToShortDateString() + ".xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public HttpResponseMessage TimesheetSummaryExcelOld(DateTime? startDate, DateTime? endDate, String userAccountId, String clients,
                                String projects, String projectWildCardSearch, String employers, bool showRates, bool showPhases)
        {
            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.MinValue.AddYears(1970);
                    startDate.Value.AddYears(1970);
                }
                if (endDate == null)
                {
                    endDate = DateTime.MinValue.AddYears(2070);
                    endDate.Value.AddYears(2170);
                }
                if (projectWildCardSearch == null)
                {
                    projectWildCardSearch = "*";
                }
                var excel = ReportProvider.GenerateTimesheetSummaryOld(startDate.Value,
                   endDate.Value, userAccountId, clients, projects, projectWildCardSearch, employers,
                   true, showRates, showPhases);

                return generateStreamResponse(excel, "TimesheetSummary- " + DateTime.Now.ToShortDateString() + ".xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public HttpResponseMessage TimesheetDetailExcelClient(DateTime? startDate, DateTime? endDate, String userAccountId, String clients,
                                String projects, bool showPhases)
        {
            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.MinValue.AddYears(1970);
                    startDate.Value.AddYears(1970);
                }
                if (endDate == null)
                {
                    endDate = DateTime.MinValue.AddYears(2070);
                    endDate.Value.AddYears(2170);
                }
                var excel = ReportProvider.GenerateTimesheetDetailClientReporter(startDate.Value, endDate.Value, clients, projects, userAccountId, showPhases);

                return generateStreamResponse(excel, "TimesheetDetail- " + DateTime.Now.ToShortDateString() + ".xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public HttpResponseMessage TimesheetDetailExcel(DateTime? startDate, DateTime? endDate, String userAccountId, String clients,
                                String projects, String employers, bool showUnassigned, bool showRates, bool showPhases)
        {
            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.MinValue.AddYears(1970);
                    startDate.Value.AddYears(1970);
                }
                if (endDate == null)
                {
                    endDate = DateTime.MinValue.AddYears(2070);
                    endDate.Value.AddYears(2170);
                }

                var excel = ReportProvider.GenerateTimesheetDetail(startDate.Value,
                   endDate.Value, projects, userAccountId, clients, employers, showUnassigned, showRates, showPhases);

                return generateStreamResponse(excel, "TimesheetDetail- " + DateTime.Now.ToShortDateString() + ".xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //[HttpGet]
        //public HttpResponseMessage TimesheetDetailExcelOld(DateTime? startDate, DateTime? endDate, String userAccountId, String clients,
        //                        String projects, String employers, bool showRates, bool showPhases)
        //{
        //    try
        //    {
        //        if (startDate == null)
        //        {
        //            startDate = DateTime.MinValue.AddYears(1970);
        //            startDate.Value.AddYears(1970);
        //        }
        //        if (endDate == null)
        //        {
        //            endDate = DateTime.MinValue.AddYears(2070);
        //            endDate.Value.AddYears(2170);
        //        }

        //        var excel = ReportProvider.GenerateTimesheetDetailOld(startDate.Value,
        //           endDate.Value, projects, userAccountId, clients, employers, showRates, showPhases);

        //        return generateStreamResponse(excel, "TimesheetDetail- " + DateTime.Now.ToShortDateString() + ".xlsx");
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        [HttpGet]
        public HttpResponseMessage BillableReportExcelClient(DateTime? startDate, DateTime? endDate, String userAccountId, String clients,
                                String projects, String employers, bool showUnassigned, bool showPhases)
        {
            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.MinValue.AddYears(1970);
                    startDate.Value.AddYears(1970);
                }
                if (endDate == null)
                {
                    endDate = DateTime.MinValue.AddYears(2070);
                    endDate.Value.AddYears(2170);
                }

                List<Guid> projectIds = new List<Guid>();
                if (projects != null && projects != "All")
                    foreach (var id in projects.Split(','))
                    {
                        projectIds.Add(new Guid(id));
                    }

                List<Guid> employersIds = new List<Guid>();
                if (employers != null && employers != "All")
                    foreach (var id in employers.Split(','))
                    {
                        employersIds.Add(new Guid(id));
                    }

                var excel = ReportProvider.GenerateBillingReportClientReporter(startDate.Value, endDate.Value, clients, projectIds, new List<Guid>(), false, false);

                return generateStreamResponse(excel, "BillingReport - " + DateTime.Now.ToShortDateString() + ".xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public HttpResponseMessage BillableReportExcel(DateTime? startDate, DateTime? endDate, String userAccountId, String clients,
                                String projects, String employers, bool showUnassigned, bool showRates, bool showPhases)
        {
            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.MinValue.AddYears(1970);
                    startDate.Value.AddYears(1970);
                }
                if (endDate == null)
                {
                    endDate = DateTime.MinValue.AddYears(2070);
                    endDate.Value.AddYears(2170);
                }
                List<Guid> projectIds = new List<Guid>();
                if (projects != null && projects != "All")
                    foreach (var id in projects.Split(','))
                    {
                        projectIds.Add(new Guid(id));
                    }

                List<Guid> employersIds = new List<Guid>();
                if (employers != null && employers != "All")
                    foreach (var id in employers.Split(','))
                    {
                        employersIds.Add(new Guid(id));
                    }

                var excel = ReportProvider.GenerateBillingReport(startDate.Value, endDate.Value, projectIds, employersIds, showUnassigned, showRates);

                return generateStreamResponse(excel, "BillingReport - " + DateTime.Now.ToShortDateString() + ".xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private HttpResponseMessage generateStreamResponse(byte[] excel, String filename)
        {
            HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            var stream = new MemoryStream(excel);
//            Stream s = GenerateStreamFromString("a,b \n c,d");
            response.Content = new StreamContent(stream);

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = filename
            };
            return response;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        //[HttpPost]
        //public HttpResponseMessage ScorecardEmployeeSummaryExcel(ScorecardReportModel model)
        //{
        //    try
        //    {
        //        TODO: Correct values being passed in Method...
        //        string filename;
        //        if (model.ScorecardTemplatePeriods != null)
        //        {
        //            int length = model.ScorecardTemplatePeriods.Split(',').Length;
        //            if (length > 0)
        //                model.ScorecardTemplatePeriodsIds = new Guid[length];
        //            for (int i = 0; i < length; i++)
        //                model.ScorecardTemplatePeriodsIds[i] = new Guid(model.ScorecardTemplatePeriods.Split(',')[i]);
        //        }
        //        var excel = ReportProvider.GenerateScorecardEmployeeSummary(model.ScorecardTemplateId,
        //            model.ScorecardTemplatePeriodsIds, model.EmployeeId, model.ScoreCardStatus, out filename);

        //        return generateStreamResponse(excel, filename);
        //    }
        //    catch (ReportException e)
        //    {
        //        throw e;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        [HttpPost]
        public HttpResponseMessage ScorecardStatusSummary(ScorecardReportModel model)
        {
            try
            {
                string filename;
                model = ProcessScorecardReportModel(model);

                //                throw new NotImplementedException();
                model.EmployeeHasScorecard = 1; // Force to has scorecard for now
                var excel = ReportProvider.GenerateScorecardStatusSummary(model.SearchAllYears,
                    model.ReviewYears, model.SearchAllPeriods, model.ReviewPeriods,
                    model.Submitted, model.Locked, model.EmployeeHasScorecard, model.Employees,
                    model.Clients, model.LineManagers, model.Evaluators, model.Scorecards, out filename);

                return generateStreamResponse(excel, filename);
            }
            catch (ReportException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public HttpResponseMessage Scorecards(ScorecardReportModel model)
        {
            try
            {
                string filename;
                model = ProcessScorecardReportModel(model);

                //                throw new NotImplementedException();

                var excel = ReportProvider.GenerateScorecardFinalCombined(model.SearchAllYears,
                    model.ReviewYears, model.SearchAllPeriods, model.ReviewPeriods, model.DetailLevel,
                    model.Submitted, model.Locked, model.Employees,
                    model.Clients, model.LineManagers, model.Evaluators, model.Scorecards[0], out filename);

                return generateStreamResponse(excel, filename);
            }
            catch (ReportException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public HttpResponseMessage ProjectAllocation(string userAccounts, bool onlyActiveUsers, bool onlyActiveClients, bool onlyActiveProjects, bool onlyActiveSubProjects)
        {
            try
            {                
                var excel = ReportProvider.GenerateProjectAllocation(userAccounts, onlyActiveUsers, onlyActiveClients, onlyActiveProjects, onlyActiveSubProjects);

                return generateStreamResponse(excel, "ProjectAllocation- " + DateTime.Now.ToShortDateString() + ".xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public HttpResponseMessage RoleAllocation(string userAccounts, bool includeInactiveRoles, bool includeInactiveUsers)
        {
            try
            {
                List<Guid> userGuids = new List<Guid>();
                if (userAccounts != "All")
                {
                    userGuids = userAccounts.Split(',').Select(Guid.Parse).ToList();
                }

                var excel = ReportProvider.GenerateRoleAllocation(userGuids, includeInactiveRoles, includeInactiveUsers);

                return generateStreamResponse(excel, "RoleAllocation- " + DateTime.Now.ToShortDateString() + ".xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static ScorecardReportModel ProcessScorecardReportModel(ScorecardReportModel model)
        {
            if (model.ReviewYearsString != null && model.ReviewYearsString != "[]")
            {
                model.ReviewYearsString = RemoveChars(model.ReviewYearsString);
                model.ReviewYears = model.ReviewYearsString.Split(',');
            }

            if (model.ReviewPeriodIds != null && model.ReviewPeriodIds != "[]")
            {
                model.ReviewPeriodIds = RemoveChars(model.ReviewPeriodIds);

                int length = model.ReviewPeriodIds.Split(',').Length;
                if (length > 0)
                    model.ReviewPeriods = new Guid[length];
                for (int i = 0; i < length; i++)
                    model.ReviewPeriods[i] = new Guid(model.ReviewPeriodIds.Split(',')[i]);
            }

            if (model.EmployeeIds != null && model.EmployeeIds != "[]")
            {
                model.EmployeeIds = RemoveChars(model.EmployeeIds);

                int length = model.EmployeeIds.Split(',').Length;
                if (length > 0)
                    model.Employees = new Guid[length];
                for (int i = 0; i < length; i++)
                    model.Employees[i] = new Guid(model.EmployeeIds.Split(',')[i]);
            }

            if (model.ClientIds != null && model.ClientIds != "[]")
            {
                model.ClientIds = RemoveChars(model.ClientIds);

                int length = model.ClientIds.Split(',').Length;
                if (length > 0)
                    model.Clients = new Guid[length];
                for (int i = 0; i < length; i++)
                    model.Clients[i] = new Guid(model.ClientIds.Split(',')[i]);
            }

            if (model.LineManagerIds != null && model.LineManagerIds != "[]")
            {
                model.LineManagerIds = RemoveChars(model.LineManagerIds);

                int length = model.LineManagerIds.Split(',').Length;
                if (length > 0)
                    model.LineManagers = new Guid[length];
                for (int i = 0; i < length; i++)
                    model.LineManagers[i] = new Guid(model.LineManagerIds.Split(',')[i]);
            }

            if (model.EvaluatorIds != null && model.EvaluatorIds != "[]")
            {
                model.EvaluatorIds = RemoveChars(model.EvaluatorIds);

                int length = model.EvaluatorIds.Split(',').Length;
                if (length > 0)
                    model.Evaluators = new Guid[length];
                for (int i = 0; i < length; i++)
                    model.Evaluators[i] = new Guid(model.EvaluatorIds.Split(',')[i]);
            }

            if (model.ScorecardIds != null && model.ScorecardIds != "[]")
            {
                model.ScorecardIds = RemoveChars(model.ScorecardIds);

                int length = model.ScorecardIds.Split(',').Length;
                if (length > 0)
                    model.Scorecards = new Guid[length];
                for (int i = 0; i < length; i++)
                    model.Scorecards[i] = new Guid(model.ScorecardIds.Split(',')[i]);
            }

            return model;
        }

        public static string RemoveChars(string input)
        {
            input = input.Replace("[", "");
            input = input.Replace("]", "");
            input = input.Replace(@"\", "");
            input = input.Replace("\"", "");

            return input;
        }

        [HttpGet]
        public HttpResponseMessage UserSummaryExcel(Guid userAccountId, bool showInactive = true)
        {
            try
            {
                var excel = ReportProvider.GenrateUserSummary(userAccountId, showInactive);

                return generateStreamResponse(excel, "UserSummary.xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public HttpResponseMessage UserAssetRegisterSummaryExcel()
        {
            try
            {
                var excel = ReportProvider.GenerateUserAssetRegisterSummary();

                return generateStreamResponse(excel, "AssetRegister.xlsx");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}