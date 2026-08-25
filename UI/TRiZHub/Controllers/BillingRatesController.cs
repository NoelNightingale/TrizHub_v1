#region Usings

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        /// Filter by UserAccountId and/or ClientId and/or ProjectId.
        /// </summary>
        [HttpPost]
        public GridResultModel<BillingRatesGridModel> BillingRatesGrid(BillingRatesSearchModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = BillingRatesProvider.BillingRatesFilterList(
                    model.UserAccountId, model.ClientId, model.ProjectId)
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

            filteredQuery = filteredQuery.OrderBy(a => a.StartDate);

            var returnList = filteredQuery.ToList();

            return new GridResultModel<BillingRatesGridModel>(returnList, totalNumberOfRecords);
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
