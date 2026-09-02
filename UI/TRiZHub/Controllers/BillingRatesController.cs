#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.BillingRatesData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Models;
using TRiZHub.Models.BillingRatesModels;

#endregion

namespace TRiZHub.Controllers
{
    public class BillingRatesController : TCRControllerBase
    {
        #region Ctor

        private BillingRatesProvider BillingRatesProvider { get; }
        private SecurityProvider SecurityProvider { get; }


        public BillingRatesController()
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            BillingRatesProvider = new BillingRatesProvider(Context, CurrentUser);
        }

        public BillingRatesController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            BillingRatesProvider = new BillingRatesProvider(Context, CurrentUser);
        }

        #endregion

        #region Billing Rates 

        /// <summary>
        /// Create or update Billing Rate
        /// </summary>
        [HttpPost]
        public BillingRatesEditModel SaveBillingRates(BillingRatesEditModel model)
        {
            try
            {
                CheckModelState();

                if (model.Id == null || model.Id == Guid.Empty)
                {
                    var billingRates = BillingRatesProvider.GetBillingRates(model.Id ?? Guid.Empty);
                    if (billingRates != null)
                        model.Id = billingRates.Id;
                }

                var record = BillingRatesProvider.SaveBillingRates(model.Id,
                    model.UserAccountId, model.Rate, model.StartDate.ToLocalTime(),
                    model.EndDate.ToLocalTime(), model.ClientId, model.ProjectId);

                var billingRatesReturn = new BillingRatesEditModel
                {
                    Id = record.Id,
                    UserAccountId = record.UserAccountId,
                    ClientId = record.ClientId,
                    ProjectId = record.ProjectId,
                    Rate = record.Rate,
                    StartDate = record.StartDate,
                    EndDate = record.EndDate,
                };

                return billingRatesReturn;
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve single Billing rate based on id
        /// </summary>
        [HttpGet]
        public BillingRatesEditModel BillingRatesGet(Guid id)
        {
            try
            {
                var billingRates = BillingRatesProvider.GetBillingRates(id);

                var model = new BillingRatesEditModel
                {
                    Id = billingRates.Id,
                    UserAccountId = billingRates.UserAccountId,
                    ClientId = billingRates.ClientId,
                    ProjectId = billingRates.ProjectId,
                    Rate = billingRates.Rate,
                    StartDate = billingRates.StartDate,
                    EndDate = billingRates.EndDate,
                };
                return model;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve list of Billing rates sorted by Startdate.
        /// Filter by UserAccountId and/or ClientId and/or ProjectId, Scope, and ActiveOn.
        /// </summary>
        [HttpPost]
        public GridResultModel<BillingRatesGridModel> BillingRatesGrid(BillingRatesSearchModel model)
        {
            var begin = SetupGridParams(model);

            DateTime? activeOn = null;
            if (model.ActiveOn.HasValue)
                activeOn = model.ActiveOn.Value.ToLocalTime().Date;

            var filteredQuery = BillingRatesProvider.BillingRatesFilterList(
                    model.UserAccountId, model.ClientId, model.ProjectId, model.Scope, activeOn,
                    model.UserAccountIds, model.ClientIds, model.ProjectIds,
                    model.UserStatus, model.ClientStatus, model.ProjectStatus)
                .Select(a => new BillingRatesGridModel
                {
                    Id = a.Id,
                    UserAccountId = a.UserAccountId,
                    Account = a.UserAccount.AccountName,
                    FirstName = a.UserAccount.FirstName,
                    Surname = a.UserAccount.Surname,
                    UserName = a.UserAccount.FirstName + " " + a.UserAccount.Surname,
                    ClientId = a.ClientId,
                    ClientName = a.Client != null ? a.Client.EntityName : null,
                    ProjectId = a.ProjectId,
                    ProjectName = a.Project != null ? a.Project.ProjectName : null,
                    Scope = a.ProjectId != null ? "Project" : (a.ClientId != null ? "Client" : "Default"),
                    Rate = a.Rate,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                });

            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = "startdate";
            else
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "username":
                case "user":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.UserName)
                        : filteredQuery.OrderByDescending(r => r.UserName);
                    break;
                case "scope":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Scope)
                        : filteredQuery.OrderByDescending(r => r.Scope);
                    break;
                case "clientname":
                case "client":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ClientName)
                        : filteredQuery.OrderByDescending(r => r.ClientName);
                    break;
                case "projectname":
                case "project":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ProjectName)
                        : filteredQuery.OrderByDescending(r => r.ProjectName);
                    break;
                case "rate":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Rate)
                        : filteredQuery.OrderByDescending(r => r.Rate);
                    break;
                case "enddate":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EndDate)
                        : filteredQuery.OrderByDescending(r => r.EndDate);
                    break;
                case "startdate":
                default:
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.StartDate)
                        : filteredQuery.OrderByDescending(r => r.StartDate);
                    break;
            }

            // Preserve legacy behavior for the existing User → Billing Rates screen:
            // it expects "All periods" for one user without paging.
            var hasMultiFilters =
                (model.UserAccountIds != null && model.UserAccountIds.Count > 0)
                || (model.ClientIds != null && model.ClientIds.Count > 0)
                || (model.ProjectIds != null && model.ProjectIds.Count > 0);

            var returnAllForSingleUser =
                !hasMultiFilters &&
                model.UserAccountId.HasValue &&
                (!model.ClientId.HasValue || model.ClientId.Value == Guid.Empty) &&
                (!model.ProjectId.HasValue || model.ProjectId.Value == Guid.Empty) &&
                string.IsNullOrWhiteSpace(model.Scope) &&
                !model.ActiveOn.HasValue;

            if (!returnAllForSingleUser)
                filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            var returnList = filteredQuery.ToList();

            return new GridResultModel<BillingRatesGridModel>(returnList, totalNumberOfRecords);
        }

        /// <summary>
        /// Export current filter selection to Excel (all matching rows, no paging).
        /// ResultMode: "periods" (default) or "effective".
        /// </summary>
        [HttpPost]
        public HttpResponseMessage ExportExcel(BillingRatesSearchModel model)
        {
            try
            {
                DateTime? activeOn = null;
                if (model != null && model.ActiveOn.HasValue)
                    activeOn = model.ActiveOn.Value.ToLocalTime().Date;

                var excel = BillingRatesProvider.ExportBillingRatesExcel(
                    model != null ? model.UserAccountIds : null,
                    model != null ? model.ClientIds : null,
                    model != null ? model.ProjectIds : null,
                    model != null ? model.Scope : null,
                    activeOn,
                    model != null ? model.ResultMode : null,
                    model != null ? model.UserStatus : null,
                    model != null ? model.ClientStatus : null,
                    model != null ? model.ProjectStatus : null);

                var mode = model != null &&
                           string.Equals(model.ResultMode, "effective", StringComparison.OrdinalIgnoreCase)
                    ? "Effective"
                    : "Periods";
                var filename = "BillingRates-" + mode + "-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StreamContent(new MemoryStream(excel));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = filename
                };
                return response;
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Effective rates as of ActiveOn for the selected filter set (Project → Client → Default).
        /// </summary>
        [HttpPost]
        public GridResultModel<BillingRatesEffectiveGridModel> EffectiveRatesGrid(BillingRatesSearchModel model)
        {
            try
            {
                if (model == null || !model.ActiveOn.HasValue)
                    throw new BillingRatesException("Effective Date is required for Effective view.");

                var begin = SetupGridParams(model);
                var asOf = model.ActiveOn.Value.ToLocalTime().Date;

                var rows = BillingRatesProvider.GetEffectiveRates(
                    model.UserAccountIds, model.ClientIds, model.ProjectIds, asOf);

                IEnumerable<BillingRatesEffectiveRow> sorted = rows;
                var sortKey = string.IsNullOrWhiteSpace(model.SortKey)
                    ? "username"
                    : model.SortKey.ToLower();
                var asc = model.SortOrder == "ASC";

                switch (sortKey)
                {
                    case "clientname":
                    case "client":
                        sorted = asc
                            ? sorted.OrderBy(r => r.ClientName)
                            : sorted.OrderByDescending(r => r.ClientName);
                        break;
                    case "projectname":
                    case "project":
                        sorted = asc
                            ? sorted.OrderBy(r => r.ProjectName)
                            : sorted.OrderByDescending(r => r.ProjectName);
                        break;
                    case "effectiverate":
                    case "rate":
                        sorted = asc
                            ? sorted.OrderBy(r => r.EffectiveRate)
                            : sorted.OrderByDescending(r => r.EffectiveRate);
                        break;
                    case "effectivescope":
                    case "scope":
                        sorted = asc
                            ? sorted.OrderBy(r => r.EffectiveScope)
                            : sorted.OrderByDescending(r => r.EffectiveScope);
                        break;
                    case "username":
                    case "user":
                    default:
                        sorted = asc
                            ? sorted.OrderBy(r => r.UserName)
                            : sorted.OrderByDescending(r => r.UserName);
                        break;
                }

                var materialised = sorted.ToList();
                var total = materialised.Count;
                var page = materialised
                    .Skip(begin)
                    .Take(model.RecordsPerPage ?? 60)
                    .Select(r => new BillingRatesEffectiveGridModel
                    {
                        Id = r.RateId,
                        UserAccountId = r.UserAccountId,
                        Account = r.AccountName,
                        FirstName = r.FirstName,
                        Surname = r.Surname,
                        UserName = r.UserName,
                        ClientId = r.ClientId,
                        ClientName = r.ClientName,
                        ProjectId = r.ProjectId,
                        ProjectName = r.ProjectName,
                        EffectiveRate = r.EffectiveRate,
                        EffectiveScope = r.EffectiveScope
                    })
                    .ToList();

                return new GridResultModel<BillingRatesEffectiveGridModel>(page, total);
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Cascading User / Client / Project option lists for the standalone Billing Rates filter workbench.
        /// </summary>
        [HttpPost]
        public BillingRatesFilterOptionsModel FilterOptions(BillingRatesFilterOptionsRequest model)
        {
            try
            {
                var result = BillingRatesProvider.GetFilterOptions(
                    model != null ? model.UserAccountIds : null,
                    model != null ? model.ClientIds : null,
                    model != null ? model.ProjectIds : null,
                    model != null ? model.UserStatus : null,
                    model != null ? model.ClientStatus : null,
                    model != null ? model.ProjectStatus : null);

                return new BillingRatesFilterOptionsModel
                {
                    Users = result.Users.Select(u => new BillingRatesFilterOptionModel
                    {
                        Id = u.Id,
                        Name = u.Name
                    }).ToList(),
                    Clients = result.Clients.Select(c => new BillingRatesFilterOptionModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList(),
                    Projects = result.Projects.Select(p => new BillingRatesFilterOptionModel
                    {
                        Id = p.Id,
                        Name = p.Name
                    }).ToList()
                };
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Billing Rate based on id
        /// </summary>
        [HttpPost]
        public void BillingRatesDelete(BillingRatesEditModel model)
        {
            try
            {
                BillingRatesProvider.DeleteBillingRatesEntry(model.Id ?? Guid.Empty);
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Team roster with Project / Client / Default rates as of a date for a project.
        /// </summary>
        [HttpPost]
        public ProjectTeamRatesModel ProjectTeamRates(ProjectTeamRatesRequest model)
        {
            try
            {
                if (model == null || model.ProjectId == Guid.Empty)
                    throw new BillingRatesException("Project is required!");

                var asOf = model.AsOfDate == default(DateTime) ? DateTime.Today : model.AsOfDate.ToLocalTime().Date;
                var result = BillingRatesProvider.GetProjectTeamRates(model.ProjectId, asOf);

                return new ProjectTeamRatesModel
                {
                    ProjectId = result.ProjectId,
                    ProjectName = result.ProjectName,
                    ClientId = result.ClientId,
                    ClientName = result.ClientName,
                    AsOfDate = result.AsOfDate,
                    Team = result.Team.Select(r => new ProjectTeamRateRowModel
                    {
                        UserAccountId = r.UserAccountId,
                        FirstName = r.FirstName,
                        Surname = r.Surname,
                        AccountName = r.AccountName,
                        UserName = ((r.FirstName ?? "") + " " + (r.Surname ?? "")).Trim(),
                        ProjectRate = r.ProjectRate,
                        ClientRate = r.ClientRate,
                        DefaultRate = r.DefaultRate,
                        EffectiveRate = r.EffectiveRate,
                        EffectiveScope = r.EffectiveScope
                    }).ToList()
                };
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// All rate periods for a user in project context (project, client, and default scopes).
        /// </summary>
        [HttpGet]
        public UserRatesForProjectContextModel UserRatesForProjectContext(Guid userId, Guid projectId)
        {
            try
            {
                var result = BillingRatesProvider.GetUserRatesForProjectContext(userId, projectId);

                Func<TRiZHub.BL.Entities.BillingRatesData.BillingRates, BillingRatesGridModel> map = a =>
                    new BillingRatesGridModel
                    {
                        Id = a.Id,
                        UserAccountId = a.UserAccountId,
                        ClientId = a.ClientId,
                        ProjectId = a.ProjectId,
                        Scope = a.ProjectId != null ? "Project" : (a.ClientId != null ? "Client" : "Default"),
                        Rate = a.Rate,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate
                    };

                return new UserRatesForProjectContextModel
                {
                    UserAccountId = result.UserAccountId,
                    UserName = result.UserName,
                    ProjectId = result.ProjectId,
                    ProjectName = result.ProjectName,
                    ClientId = result.ClientId,
                    ClientName = result.ClientName,
                    ProjectRates = result.ProjectRates.Select(map).ToList(),
                    ClientRates = result.ClientRates.Select(map).ToList(),
                    DefaultRates = result.DefaultRates.Select(map).ToList()
                };
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Team roster with Client / Default rates as of a date for a client, plus project override counts.
        /// </summary>
        [HttpPost]
        public ClientTeamRatesModel ClientTeamRates(ClientTeamRatesRequest model)
        {
            try
            {
                if (model == null || model.ClientId == Guid.Empty)
                    throw new BillingRatesException("Client is required!");

                var asOf = model.AsOfDate == default(DateTime) ? DateTime.Today : model.AsOfDate.ToLocalTime().Date;
                var result = BillingRatesProvider.GetClientTeamRates(model.ClientId, asOf);

                return new ClientTeamRatesModel
                {
                    ClientId = result.ClientId,
                    ClientName = result.ClientName,
                    AsOfDate = result.AsOfDate,
                    Team = result.Team.Select(r => new ClientTeamRateRowModel
                    {
                        UserAccountId = r.UserAccountId,
                        FirstName = r.FirstName,
                        Surname = r.Surname,
                        AccountName = r.AccountName,
                        UserName = ((r.FirstName ?? "") + " " + (r.Surname ?? "")).Trim(),
                        ClientRate = r.ClientRate,
                        DefaultRate = r.DefaultRate,
                        EffectiveRate = r.EffectiveRate,
                        EffectiveScope = r.EffectiveScope,
                        ProjectOverrideCount = r.ProjectOverrideCount
                    }).ToList()
                };
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Rate periods for a user in client context (client, default, and project overrides under the client).
        /// </summary>
        [HttpGet]
        public UserRatesForClientContextModel UserRatesForClientContext(Guid userId, Guid clientId)
        {
            try
            {
                var result = BillingRatesProvider.GetUserRatesForClientContext(userId, clientId);

                Func<TRiZHub.BL.Entities.BillingRatesData.BillingRates, BillingRatesGridModel> map = a =>
                    new BillingRatesGridModel
                    {
                        Id = a.Id,
                        UserAccountId = a.UserAccountId,
                        ClientId = a.ClientId,
                        ProjectId = a.ProjectId,
                        Scope = a.ProjectId != null ? "Project" : (a.ClientId != null ? "Client" : "Default"),
                        Rate = a.Rate,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate
                    };

                return new UserRatesForClientContextModel
                {
                    UserAccountId = result.UserAccountId,
                    UserName = result.UserName,
                    ClientId = result.ClientId,
                    ClientName = result.ClientName,
                    ClientRates = result.ClientRates.Select(map).ToList(),
                    DefaultRates = result.DefaultRates.Select(map).ToList(),
                    ProjectRateGroups = result.ProjectRateGroups.Select(g => new ClientProjectRateGroupModel
                    {
                        ProjectId = g.ProjectId,
                        ProjectName = g.ProjectName,
                        Rates = g.Rates.Select(map).ToList()
                    }).ToList(),
                    ClientProjects = result.ClientProjects.Select(p => new ClientProjectOptionModel
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName
                    }).ToList()
                };
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// User billing rates as-of hierarchy (default, clients, nested projects).
        /// </summary>
        [HttpPost]
        public UserRatesAsOfModel UserRatesAsOf(UserRatesAsOfRequest request)
        {
            try
            {
                var result = BillingRatesProvider.GetUserRatesAsOf(request.UserAccountId, request.AsOfDate);
                return new UserRatesAsOfModel
                {
                    UserAccountId = result.UserAccountId,
                    UserName = result.UserName,
                    AsOfDate = result.AsOfDate,
                    DefaultRate = result.DefaultRate,
                    DefaultRateId = result.DefaultRateId,
                    Clients = result.Clients.Select(c => new UserRatesAsOfClientRowModel
                    {
                        ClientId = c.ClientId,
                        ClientName = c.ClientName,
                        ClientRate = c.ClientRate,
                        ClientRateId = c.ClientRateId,
                        EffectiveRate = c.EffectiveRate,
                        EffectiveScope = c.EffectiveScope,
                        Projects = c.Projects.Select(p => new UserRatesAsOfProjectRowModel
                        {
                            ProjectId = p.ProjectId,
                            ProjectName = p.ProjectName,
                            ProjectRate = p.ProjectRate,
                            ProjectRateId = p.ProjectRateId,
                            EffectiveRate = p.EffectiveRate,
                            EffectiveScope = p.EffectiveScope
                        }).ToList()
                    }).ToList()
                };
            }
            catch (BillingRatesException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion
    }
}
