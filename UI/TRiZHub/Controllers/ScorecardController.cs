#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.ScorecardData;
using TRiZHub.BL.Provider.ScorecardTemplateData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Provider.TeamJobDesignationData;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.ScorecardModels;
using TRiZHub.Models.ScorecardTemplateModels;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class ScorecardController : TCRControllerBase
    {
        #region Ctor

        public ScorecardController()
        {
            AppSettings = new AppSettings(Context);
            ScorecardTemplateProvider = new ScorecardTemplateProvider(Context, CurrentUser);
            ScorecardProvider = new ScorecardProvider(Context, CurrentUser);
            TeamJobDesignationProvider = new TeamJobDesignationProvider(Context, CurrentUser);
        }

        public ScorecardController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            ScorecardTemplateProvider = new ScorecardTemplateProvider(Context, CurrentUser);
            ScorecardProvider = new ScorecardProvider(Context, CurrentUser);
            TeamJobDesignationProvider = new TeamJobDesignationProvider(Context, CurrentUser);
        }

        private IAppSettings AppSettings { get; }
        private IScorecardTemplateProvider ScorecardTemplateProvider { get; }
        private IScorecardProvider ScorecardProvider { get; }
        private ITeamJobDesignationProvider TeamJobDesignationProvider { get; }

        #endregion

        #region Scorecard Template 

        /// <summary>
        /// Retrieve list of Scorecard Templates based on filtered and sorted input values 
        /// </summary>
        [HttpPost]
        public GridResultModel<ScorecardTemplateGridModel> ScorecardTemplateGrid(GridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery =
                ScorecardTemplateProvider.ScorecardTemplateList().Select(a => new ScorecardTemplateGridModel
                {
                    Id = a.Id,
                    IsActive = a.IsActive,
                    ScorecardCode = a.ScorecardCode,
                    ScorecardName = a.ScorecardName
                });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.ScorecardName.Contains(model.Searchfor));
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.ScorecardName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "scorecardname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardName)
                        : filteredQuery.OrderByDescending(r => r.ScorecardName);
                    break;
                case "scorecardcode":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardCode)
                        : filteredQuery.OrderByDescending(r => r.ScorecardCode);
                    break;
                case "isactive":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.IsActive)
                        : filteredQuery.OrderByDescending(r => r.IsActive);
                    break;
            }


            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<ScorecardTemplateGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve single Scorecard Template
        /// </summary>
        [HttpGet]
        public ScorecardTemplateModel ScorecardTemplateGet(Guid? id)
        {
            try
            {
                var record = ScorecardTemplateProvider.GetScorecardTemplate(id.Value);

                var model = new ScorecardTemplateModel
                {
                    Id = record.Id,
                    IsActive = record.IsActive,
                    ScorecardCode = record.ScorecardCode,
                    ScorecardName = record.ScorecardName,
                    ExcellentWeight = record.ExcellentWeight,
                    AdequateWeight = record.AdequateWeight,
                    InadequateWeight = record.InadequateWeight,
                    TotalAvailableWeight =
                        record.ScorecardTemplateItems.Any()
                            ? 100 - record.ScorecardTemplateItems.Sum(a => a.Weight)
                            : 100
                };

                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Create or update Scorecard Template
        /// </summary>
        [HttpPost]
        public ScorecardTemplateModel ScorecardTemplateSave(ScorecardTemplateModel model)
        {
            try
            {
                CheckModelState();

                var record = ScorecardTemplateProvider.SaveScorecardTemplate(model.Id, model.ScorecardName, model.ScorecardCode, model.ExcellentWeight, model.AdequateWeight, model.InadequateWeight, model.IsActive);

                model = new ScorecardTemplateModel
                {
                    Id = record.Id,
                    IsActive = record.IsActive,
                    ScorecardCode = record.ScorecardCode,
                    ScorecardName = record.ScorecardName,
                    ExcellentWeight = record.ExcellentWeight,
                    AdequateWeight = record.AdequateWeight,
                    InadequateWeight = record.InadequateWeight,
                };

                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Scorecard Template
        /// </summary>
        [HttpPost]
        public void ScorecardTemplateDelete(ScorecardTemplateModel model)
        {
            try
            {
                CheckModelState();

                ScorecardTemplateProvider.DeleteScorecardTemplate(model.Id);
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion

        #region Scorecard Template Period
        /// <summary>
        /// Retrieve list of Scorecard Template periods based on filter and sort input values
        /// </summary>
        [HttpPost]
        public GridResultModel<ScorecardTemplatePeriodGridModel> ScorecardTemplatePeriodGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery =
                ScorecardTemplateProvider.ScorecardTemplatePeriodList(new List<Guid> { model.Id.Value })
                    .Select(a => new ScorecardTemplatePeriodGridModel
                    {
                        Id = a.Id,
                        Description = a.Description,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        ReviewYear = a.ReviewYear,
                        IsVariable = a.IsVariable,
                        ReportSortOrder = a.ReportSortOrder
                    });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.Description.Contains(model.Searchfor));
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.Description); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "description":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Description)
                        : filteredQuery.OrderByDescending(r => r.Description);
                    break;
                case "variable":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.IsVariable)
                        : filteredQuery.OrderByDescending(r => r.IsVariable);
                    break;
                case "startdate":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.IsVariable).ThenBy(r => r.StartDate)
                        : filteredQuery.OrderByDescending(r => r.IsVariable).ThenByDescending(r => r.StartDate);
                    break;
                case "enddate":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.IsVariable).ThenBy(r => r.EndDate)
                        : filteredQuery.OrderByDescending(r => r.IsVariable).ThenByDescending(r => r.EndDate);
                    break;
                case "reviewyear":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ReviewYear)
                        : filteredQuery.OrderByDescending(r => r.ReviewYear);
                    break;
                case "reportsortorder":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ReportSortOrder)
                        : filteredQuery.OrderByDescending(r => r.ReportSortOrder);
                    break;
            }


            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<ScorecardTemplatePeriodGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve single Scorecard Template period based in id
        /// </summary>
        [HttpGet]
        public ScorecardTemplatePeriodModel ScorecardTemplatePeriodGet(Guid? id)
        {
            try
            {
                var record = ScorecardTemplateProvider.GetScorecardTemplatePeriod(id.Value);

                var model = new ScorecardTemplatePeriodModel
                {
                    Id = record.Id,
                    Description = record.Description,
                    StartDate = record.StartDate,
                    EndDate = record.EndDate,
                    ReviewYear = record.ReviewYear,
                    IsVariable = record.IsVariable,
                    ReportSortOrder = record.ReportSortOrder
                };

                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Save Scorecard Template Period
        /// </summary>
        [HttpPost]
        public ScorecardTemplatePeriodModel ScorecardTemplatePeriodSave(ScorecardTemplatePeriodModel model)
        {
            try
            {
                CheckModelState();

                var record = ScorecardTemplateProvider.SaveScorecardTemplatePeriod(model.Id, model.ScorecardTemplateId,
                    model.StartDate.ToLocalTime(), model.EndDate.ToLocalTime(), model.Description, model.ReviewYear, model.IsVariable, model.ReportSortOrder);

                model.Id = record.Id;

                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Scorecard Template Period
        /// </summary>
        [HttpPost]
        public void ScorecardTemplatePeriodDelete(ScorecardTemplatePeriodModel model)
        {
            try
            {
                CheckModelState();

                ScorecardTemplateProvider.DeleteScorecardTemplatePeriod(model.Id);
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion

        #region Scorecard Template Item

        /// <summary>
        /// Retrieve list of Scorecard Template Items based on filter and sort input values
        /// </summary>
        [HttpPost]
        public GridResultModel<ScorecardTemplateItemGridModel> ScorecardTemplateItemGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery =
                ScorecardTemplateProvider.ScorecardTemplateItemList(model.Id.Value)
                    .Select(a => new ScorecardTemplateItemGridModel
                    {
                        Id = a.Id,
                        Description = a.Description,
                        Weight = a.Weight,
                        Order = a.Order
                    });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.Description.Contains(model.Searchfor));
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.Description); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "description":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Description)
                        : filteredQuery.OrderByDescending(r => r.Description);
                    break;
                case "order":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Order)
                        : filteredQuery.OrderByDescending(r => r.Order);
                    break;
                case "weight":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Weight)
                        : filteredQuery.OrderByDescending(r => r.Weight);
                    break;
            }


            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<ScorecardTemplateItemGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve single Scorecard Template Item
        /// </summary>
        [HttpGet]
        public ScorecardTemplateItemModel ScorecardTemplateItemGet(Guid? id)
        {
            try
            {
                var record = ScorecardTemplateProvider.GetScorecardTemplateItem(id.Value);

                var model = new ScorecardTemplateItemModel
                {
                    Id = record.Id,
                    Description = record.Description,
                    Definition = record.Definition,
                    Weight = record.Weight,
                    ScorecardScoring = record.ScorecardScoring,
                    Maximum = record.Maximum,
                    Minimum = record.Minimum,
                    ManualDefinition = record.ManualDefinition,
                    ExcellentDefinition = record.ExcellentDefinition,
                    AdequateDefinition = record.AdequateDefinition,
                    InadequateDefinition = record.InadequateDefinition,
                    Order = record.Order,
                };


                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Save Scorecard Template Item
        /// </summary>
        [HttpPost]
        public ScorecardTemplateItemModel ScorecardTemplateItemSave(ScorecardTemplateItemModel model)
        {
            try
            {
                CheckModelState();

                Context.BeginTransaction();

                var record = ScorecardTemplateProvider.SaveScorecardTemplateItem(model.Id, model.ScorecardTemplateId, model.Description, model.Definition, model.Weight, model.ScorecardScoring, model.Minimum, model.Maximum,
                    model.ManualDefinition, model.ExcellentDefinition, model.AdequateDefinition, model.InadequateDefinition, model.Order);

                model.Id = record.Id;

                Context.CommitTransaction();

                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Scorecard Template Item
        /// </summary>
        [HttpPost]
        public void ScorecardTemplateItemDelete(ScorecardTemplateItemModel model)
        {
            try
            {
                CheckModelState();

                ScorecardTemplateProvider.DeleteScorecardTemplateItem(model.Id);
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion

        #region Scorecard

        /// <summary>
        /// Retrieve list of Scorecards based on filter and sort input values
        /// </summary>
        [HttpPost]
        public GridResultModel<ScorecardGridModel> ScorecardGrid(GridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = ScorecardProvider.ScorecardList()
                    .Where(a => a.EvaluatorId == CurrentUser.Id)
                    .ToList()
                    .Select(a => new ScorecardGridModel
                    {
                        ScorecardPeriodId = a.ScorecardTemplatePeriodId,
                        ScorecardPeriod = a.ScorecardTemplatePeriod.IsVariable && a.VariableStart != null ? a.VariableStart.Value.ToString(@"yyyy\/MM\/dd") + " - " + a.VariableEnd.Value.ToString(@"yyyy\/MM\/dd") : a.ScorecardTemplatePeriod.StartDate.ToString(@"yyyy\/MM\/dd") + " - " + a.ScorecardTemplatePeriod.EndDate.ToString(@"yyyy\/MM\/dd"),
                        ScorecardId = a.Id,
                        //code
                        ScorecardName = a.ScorecardTemplate.ScorecardName,
                        EmployeeName = a.Employee.FirstName + " " + a.Employee.Surname,
                        EvaluatorName = a.Evaluator.FirstName + " " + a.Evaluator.Surname,
                        Completed = a.Completed,
                        DateCreated = a.DateCreated,
                        locked = a.locked

                    });


            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(r =>
                        r.EmployeeName.ToLower().Contains(model.CustomSearchModel.EmployeeName.ToLower())
                        || r.ScorecardName.ToLower().Contains(model.CustomSearchModel.ScoreCardName.ToLower())
                        || r.EvaluatorName.ToLower().Contains(model.CustomSearchModel.EvaluatorName.ToLower())
                    );
            }

            filteredQuery = filteredQuery.Where(r => r.locked == model.CustomSearchModel.Locked);
            filteredQuery = filteredQuery.Where(r => r.Completed == model.CustomSearchModel.Submitted);

            if (model.CustomSearchModel.Year != 0)
            {
                filteredQuery = filteredQuery.Where(r => r.ScorecardPeriod.Split('-')[0].Split('/')[0] == model.CustomSearchModel.Year.ToString());
            }

            if (model.CustomSearchModel.VariablePeriod)
            {
                if (model.CustomSearchModel.PeriodStart != null)
                {
                    filteredQuery = filteredQuery.Where(r => r.ScorecardPeriodId == model.CustomSearchModel.PeriodId);
                }
            }
            else
            {
                if (model.CustomSearchModel.PeriodStart != null)
                {
                    filteredQuery = filteredQuery.Where(r => r.ScorecardPeriod == model.CustomSearchModel.PeriodStart.Value.ToString(@"yyyy\/MM\/dd") + " - " + model.CustomSearchModel.PeriodEnd.Value.ToString(@"yyyy\/MM\/dd"));
                }
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.ScorecardName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "scorecardname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardName)
                        : filteredQuery.OrderByDescending(r => r.ScorecardName);
                    break;
                case "period":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardPeriod)
                        : filteredQuery.OrderByDescending(r => r.ScorecardPeriod);
                    break;
                case "scorecardcode":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardCode)
                        : filteredQuery.OrderByDescending(r => r.ScorecardCode);
                    break;
                case "evaluatorname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EvaluatorName)
                        : filteredQuery.OrderByDescending(r => r.EvaluatorName);
                    break;
                case "employeename":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EmployeeName)
                        : filteredQuery.OrderByDescending(r => r.EmployeeName);
                    break;
                case "completed":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Completed)
                        : filteredQuery.OrderByDescending(r => r.Completed);
                    break;
                case "datecreated":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.DateCreated)
                        : filteredQuery.OrderByDescending(r => r.DateCreated);
                    break;
                case "locked":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.locked)
                        : filteredQuery.OrderByDescending(r => r.locked);
                    break;
            }


            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<ScorecardGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve list of Scorecards based on current user and filter and sort input values
        /// </summary>
        [HttpPost]
        public GridResultModel<ScorecardGridModel> MyScorecardGrid(GridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery =
                ScorecardProvider.ScorecardList()
                    .Where(a => a.EmployeeId == CurrentUser.Id && a.Completed == true)
                    .ToList()
                    .Select(a => new ScorecardGridModel
                    {
                        ScorecardPeriodId = a.Id,
                        ScorecardPeriod = a.ScorecardTemplatePeriod.IsVariable && a.VariableStart != null ? a.VariableStart.Value.ToString(@"yyyy\/MM\/dd") + " - " + a.VariableEnd.Value.ToString(@"yyyy\/MM\/dd") : a.ScorecardTemplatePeriod.StartDate.ToString(@"yyyy\/MM\/dd") + " - " + a.ScorecardTemplatePeriod.EndDate.ToString(@"yyyy\/MM\/dd"),

                        ScorecardId = a.Id,

                        ScorecardName = a.ScorecardTemplate.ScorecardName,
                        EmployeeName = a.Employee.FirstName + " " + a.Employee.Surname,
                        EvaluatorName = a.Evaluator.FirstName + " " + a.Evaluator.Surname,
                        Completed = a.Completed,
                        DateCreated = a.DateCreated,
                        locked = a.locked

                    });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(r =>
                          r.EmployeeName.ToLower().Contains(model.CustomSearchModel.EmployeeName.ToLower())
                        || r.ScorecardName.ToLower().Contains(model.CustomSearchModel.ScoreCardName.ToLower())
                        || r.EvaluatorName.ToLower().Contains(model.CustomSearchModel.EvaluatorName.ToLower())
                    );
            }

            filteredQuery = filteredQuery.Where(r => r.locked == model.CustomSearchModel.Locked);
            filteredQuery = filteredQuery.Where(r => r.Completed == model.CustomSearchModel.Submitted);

            if (model.CustomSearchModel.Year != 0)
            {
                filteredQuery = filteredQuery.Where(r => r.ScorecardPeriod.Split('-')[0].Split('/')[0] == model.CustomSearchModel.Year.ToString());
            }

            if (model.CustomSearchModel.VariablePeriod)
            {
                if (model.CustomSearchModel.PeriodStart != null)
                {
                    filteredQuery = filteredQuery.Where(r => r.ScorecardPeriodId == model.CustomSearchModel.PeriodId);
                }
            }
            else
            {
                if (model.CustomSearchModel.PeriodStart != null)
                {
                    filteredQuery = filteredQuery.Where(r => r.ScorecardPeriod == model.CustomSearchModel.PeriodStart.Value.ToString(@"yyyy\/MM\/dd") + " - " + model.CustomSearchModel.PeriodEnd.Value.ToString(@"yyyy\/MM\/dd"));
                }
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.ScorecardName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "scorecardname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardName)
                        : filteredQuery.OrderByDescending(r => r.ScorecardName);
                    break;
                case "period":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardPeriod)
                        : filteredQuery.OrderByDescending(r => r.ScorecardPeriod);
                    break;
                case "scorecardcode":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardCode)
                        : filteredQuery.OrderByDescending(r => r.ScorecardCode);
                    break;
                case "evaluatorname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EvaluatorName)
                        : filteredQuery.OrderByDescending(r => r.EvaluatorName);
                    break;
                case "employeename":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EmployeeName)
                        : filteredQuery.OrderByDescending(r => r.EmployeeName);
                    break;
                case "completed":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Completed)
                        : filteredQuery.OrderByDescending(r => r.Completed);
                    break;
                case "datecreated":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.DateCreated)
                        : filteredQuery.OrderByDescending(r => r.DateCreated);
                    break;
                case "locked":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.locked)
                        : filteredQuery.OrderByDescending(r => r.locked);
                    break;
            }


            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<ScorecardGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve list of Scorecards based on team leader's employees and filter and sorted input values 
        /// </summary>
        [HttpPost]
        public GridResultModel<ScorecardGridModel> TeamScorecardGrid(GridModel model)
        {
            var begin = SetupGridParams(model);

            var teamJobDesignationids = TeamJobDesignationProvider.TeamJobDesignationtLineLeadFilterList(CurrentUser.Id).Where(a => a.StartDate < DateTime.Now && (a.EndDate == null || a.EndDate > DateTime.Now)).Select(a => a.UserAccountId).ToList();

            var filteredQuery = ScorecardProvider.ScorecardList()
                                .Where(a => teamJobDesignationids.Contains(a.EmployeeId) && a.Completed == true)
                                .ToList()
                                .Select(a => new ScorecardGridModel
                                {
                                    ScorecardPeriodId = a.Id,
                                    ScorecardPeriod = a.ScorecardTemplatePeriod.IsVariable && a.VariableStart != null ? a.VariableStart.Value.ToString(@"yyyy\/MM\/dd") + " - " + a.VariableEnd.Value.ToString(@"yyyy\/MM\/dd") : a.ScorecardTemplatePeriod.StartDate.ToString(@"yyyy\/MM\/dd") + " - " + a.ScorecardTemplatePeriod.EndDate.ToString(@"yyyy\/MM\/dd"),

                                    ScorecardId = a.Id,

                                    ScorecardName = a.ScorecardTemplate.ScorecardName,
                                    EmployeeName = a.Employee.FirstName + " " + a.Employee.Surname,
                                    EvaluatorName = a.Evaluator.FirstName + " " + a.Evaluator.Surname,
                                    Completed = a.Completed,
                                    DateCreated = a.DateCreated,
                                    locked = a.locked

                                });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(r =>
                          r.EmployeeName.ToLower().Contains(model.CustomSearchModel.EmployeeName.ToLower())
                        || r.ScorecardName.ToLower().Contains(model.CustomSearchModel.ScoreCardName.ToLower())
                        || r.EvaluatorName.ToLower().Contains(model.CustomSearchModel.EvaluatorName.ToLower())
                    );
            }

            filteredQuery = filteredQuery.Where(r => r.locked == model.CustomSearchModel.Locked);
            filteredQuery = filteredQuery.Where(r => r.Completed == model.CustomSearchModel.Submitted);

            if (model.CustomSearchModel.Year != 0)
            {
                filteredQuery = filteredQuery.Where(r => r.ScorecardPeriod.Split('-')[0].Split('/')[0] == model.CustomSearchModel.Year.ToString());
            }

            if (model.CustomSearchModel.VariablePeriod)
            {
                if (model.CustomSearchModel.PeriodStart != null)
                {
                    filteredQuery = filteredQuery.Where(r => r.ScorecardPeriodId == model.CustomSearchModel.PeriodId);
                }
            }
            else
            {
                if (model.CustomSearchModel.PeriodStart != null)
                {
                    filteredQuery = filteredQuery.Where(r => r.ScorecardPeriod == model.CustomSearchModel.PeriodStart.Value.ToString(@"yyyy\/MM\/dd") + " - " + model.CustomSearchModel.PeriodEnd.Value.ToString(@"yyyy\/MM\/dd"));
                }
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.ScorecardName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "scorecardname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardName)
                        : filteredQuery.OrderByDescending(r => r.ScorecardName);
                    break;
                case "period":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardPeriod)
                        : filteredQuery.OrderByDescending(r => r.ScorecardPeriod);
                    break;
                case "scorecardcode":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardCode)
                        : filteredQuery.OrderByDescending(r => r.ScorecardCode);
                    break;
                case "evaluatorname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EvaluatorName)
                        : filteredQuery.OrderByDescending(r => r.EvaluatorName);
                    break;
                case "employeename":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EmployeeName)
                        : filteredQuery.OrderByDescending(r => r.EmployeeName);
                    break;
                case "completed":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Completed)
                        : filteredQuery.OrderByDescending(r => r.Completed);
                    break;
                case "datecreated":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.DateCreated)
                        : filteredQuery.OrderByDescending(r => r.DateCreated);
                    break;
                case "locked":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.locked)
                        : filteredQuery.OrderByDescending(r => r.locked);
                    break;
            }


            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<ScorecardGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve list of Scorecards based on filter and sort input values
        /// </summary>
        [HttpPost]
        public GridResultModel<ScorecardGridModel> ScorecardGridAdmin(GridModel model)
        {
            var begin = SetupGridParams(model);
            var filteredQuery = ScorecardProvider.ScorecardList().ToList()
                    .Select(a => new ScorecardGridModel
                    {
                        ScorecardPeriodId = a.ScorecardTemplatePeriodId,
                        ScorecardPeriod = a.ScorecardTemplatePeriod.IsVariable && a.VariableStart != null ? a.VariableStart.Value.ToString(@"yyyy\/MM\/dd") + " - " + a.VariableEnd.Value.ToString(@"yyyy\/MM\/dd") : a.ScorecardTemplatePeriod.StartDate.ToString(@"yyyy\/MM\/dd") + " - " + a.ScorecardTemplatePeriod.EndDate.ToString(@"yyyy\/MM\/dd"),
                        ScorecardId = a.Id,
                        //code
                        ScorecardName = a.ScorecardTemplate.ScorecardName,
                        EmployeeName = a.Employee.FirstName + " " + a.Employee.Surname,
                        EvaluatorName = a.Evaluator.FirstName + " " + a.Evaluator.Surname,
                        Completed = a.Completed,
                        DateCreated = a.DateCreated,
                        locked = a.locked

                    });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(r =>
                         r.EmployeeName.ToLower().Contains(model.CustomSearchModel.EmployeeName.ToLower())
                        || r.ScorecardName.ToLower().Contains(model.CustomSearchModel.ScoreCardName.ToLower())
                        || r.EvaluatorName.ToLower().Contains(model.CustomSearchModel.EvaluatorName.ToLower())
                    );
            }

            filteredQuery = filteredQuery.Where(r => r.locked == model.CustomSearchModel.Locked);
            filteredQuery = filteredQuery.Where(r => r.Completed == model.CustomSearchModel.Submitted);

            if (model.CustomSearchModel.Year != 0)
            {
                filteredQuery = filteredQuery.Where(r => r.ScorecardPeriod.Split('-')[0].Split('/')[0] == model.CustomSearchModel.Year.ToString());
            }

            if (model.CustomSearchModel.VariablePeriod)
            {
                if (model.CustomSearchModel.PeriodStart != null)
                {
                    filteredQuery = filteredQuery.Where(r => r.ScorecardPeriodId == model.CustomSearchModel.PeriodId);
                }
            }
            else
            {
                if (model.CustomSearchModel.PeriodStart != null)
                {
                    filteredQuery = filteredQuery.Where(r => r.ScorecardPeriod == model.CustomSearchModel.PeriodStart.Value.ToString(@"yyyy\/MM\/dd") + " - " + model.CustomSearchModel.PeriodEnd.Value.ToString(@"yyyy\/MM\/dd"));
                }
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.ScorecardName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "scorecardname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardName)
                        : filteredQuery.OrderByDescending(r => r.ScorecardName);
                    break;
                case "period":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardPeriod)
                        : filteredQuery.OrderByDescending(r => r.ScorecardPeriod);
                    break;
                case "scorecardcode":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ScorecardCode)
                        : filteredQuery.OrderByDescending(r => r.ScorecardCode);
                    break;
                case "evaluatorname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EvaluatorName)
                        : filteredQuery.OrderByDescending(r => r.EvaluatorName);
                    break;
                case "employeename":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EmployeeName)
                        : filteredQuery.OrderByDescending(r => r.EmployeeName);
                    break;
                case "completed":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Completed)
                        : filteredQuery.OrderByDescending(r => r.Completed);
                    break;
                case "datecreated":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.DateCreated)
                        : filteredQuery.OrderByDescending(r => r.DateCreated);
                    break;
                case "locked":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.locked)
                        : filteredQuery.OrderByDescending(r => r.locked);
                    break;
            }


            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<ScorecardGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        // NOT IN USER 
        /// <summary>
        /// N/A
        /// </summary>
        [HttpGet]
        public ScorecardModel ScorecardGet1(Guid? id)
        {
            try
            {
                var record = ScorecardProvider.GetScorecard(id.Value);

                var model = new ScorecardModel
                {
                    ScorecardId = record.Id,
                    ScorecardTemplateId = record.ScorecardTemplateId,
                    ScorecardTemplatePeriodId = record.ScorecardTemplatePeriodId,
                    //CODE
                    ScorecardName = record.ScorecardTemplate.ScorecardName,
                    DateCreated = record.DateCreated,
                    Completed = record.Completed,
                    EmployeeId = record.EmployeeId,
                    EmployeeName = record.Employee.FirstName + " " + record.Employee.Surname,
                    EvaluatorId = record.EvaluatorId,
                    EvaluatorName = record.Evaluator.FirstName + " " + record.Employee.Surname,
                    ScorecardPeriodName = record.ScorecardTemplatePeriod.Description,
                    EmployeeMessage = record.EmployeeMessage,
                    EvaluatorMessage = record.EvaluatorMessage,
                };

                return model;
            }
            catch (ScorecardException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }
        //

        /// <summary>
        /// Create or update Scorecard
        /// </summary>
        [HttpPost]
        public ScorecardModel ScorecardSave(ScorecardModel model)
        {
            try
            {
                CheckModelState();
                ScorecardProvider.BeginTransaction();

                var master = ScorecardProvider.SaveScorecard(model.ScorecardId, model.ScorecardTemplateId,
                    model.EvaluatorId,
                    model.EmployeeId, model.ScorecardTemplatePeriodId, model.rated, model.Completed, model.createdBy, model.DateCreated, model.EvaluatorMessage, model.EmployeeMessage, model.VariableStart, model.VariableEnd, model.VariableYear);


                ScorecardProvider.CommitTransaction();

                return model;
            }
            catch (ScorecardException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Create or update Scorecard
        /// </summary>
        [HttpPost]
        public void ScorecardReassign(ScorecardModel model)
        {
            try
            {
                CheckModelState();

                var scorecard = ScorecardProvider.GetScorecard(model.ScorecardId.Value);
                if (scorecard.EmployeeId == model.EvaluatorId)
                {
                    throw new ScorecardException("A score card cannot have the same Evaluator and Employee.");
                }

                ScorecardProvider.BeginTransaction();
                ScorecardProvider.ReassignScorecard(model.ScorecardId, model.EvaluatorId);
                ScorecardProvider.CommitTransaction();
            }
            catch (ScorecardException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion

        #region Scorecard Record

        /// <summary>
        /// Retrieve single Scorecard
        /// </summary>
        [HttpGet]
        public ScorecardCollectionModel ScorecardGet(Guid? id)
        {
            try
            {
                var record = ScorecardProvider.GetScorecard(id.Value);

                var model = new ScorecardCollectionModel
                {
                    ScorecardModel = new ScorecardModel
                    {
                        ScorecardId = record.Id,
                        ScorecardTemplateId = record.ScorecardTemplateId,
                        ScorecardTemplatePeriodId = record.ScorecardTemplatePeriodId,
                        ScorecardName = record.ScorecardTemplate.ScorecardName,
                        DateCreated = record.DateCreated,
                        Completed = record.Completed,
                        EmployeeId = record.EmployeeId,
                        EmployeeName = record.Employee.FirstName + " " + record.Employee.Surname,
                        EvaluatorId = record.EvaluatorId,
                        EvaluatorName = record.Evaluator.FirstName + " " + record.Evaluator.Surname,
                        ScorecardPeriodName = record.ScorecardTemplatePeriod.Description + " " + record.ScorecardTemplatePeriod.StartDate.ToString(@"yyyy\/MM\/dd") + " - " + record.ScorecardTemplatePeriod.EndDate.ToString(@"yyyy\/MM\/dd"),
                        EvaluatorMessage = record.EvaluatorMessage,
                        EmployeeMessage = record.EmployeeMessage,
                        locked = record.locked
                    },
                    ScorecardRecordModels = record.ScorecardRecords.Select(
                        a => new ScorecardRecordModel
                        {
                            ScorecardRecordId = a.Id,
                            ScorecardTemplateItemId = a.ScorecardTemplateItemId,
                            Description = a.ScorecardTemplateItem.Description,
                            Definition = a.ScorecardTemplateItem.Definition,
                            Weight = a.ScorecardTemplateItem.Weight,
                            ScoreType = a.Rating,
                            ManualDefinition = a.ScorecardTemplateItem.ManualDefinition,
                            EDefinition = a.ScorecardTemplateItem.ExcellentDefinition,
                            ADefinition = a.ScorecardTemplateItem.AdequateDefinition,
                            IDefinition = a.ScorecardTemplateItem.InadequateDefinition,
                            Minimum = a.ScorecardTemplateItem.Minimum,
                            Maximum = a.ScorecardTemplateItem.Maximum,
                            ScorecardScoring = a.ScorecardTemplateItem.ScorecardScoring,
                            Value = a.Value,
                            Order = a.ScorecardTemplateItem.Order,
                            EvaluatorHtmlComment = a.EvaluatorHtmlComment,
                            EmployeeHtmlComment = a.EmployeeHtmlComment
                        }).ToList()
                };

                // Get all records
                var allTemplateItemsInUse = model.ScorecardRecordModels.Select(a => a.ScorecardTemplateItemId).ToList();

                var scorecardTemplates = ScorecardTemplateProvider.ScorecardTemplateItemList(record.ScorecardTemplatePeriod.ScorecardTemplateId).Where(a => !allTemplateItemsInUse.Contains(a.Id)).ToList();

                foreach (var scorecardTemplateItem in scorecardTemplates)
                {
                    model.ScorecardRecordModels.Add(new ScorecardRecordModel
                    {
                        Description = scorecardTemplateItem.Description,
                        Definition = scorecardTemplateItem.Definition,
                        ScorecardTemplateItemId = scorecardTemplateItem.Id,
                        Weight = scorecardTemplateItem.Weight,
                        ManualDefinition = scorecardTemplateItem.ManualDefinition,
                        EDefinition = scorecardTemplateItem.ExcellentDefinition,
                        ADefinition = scorecardTemplateItem.AdequateDefinition,
                        IDefinition = scorecardTemplateItem.InadequateDefinition,
                        Minimum = scorecardTemplateItem.Minimum,
                        Maximum = scorecardTemplateItem.Maximum,
                        ScorecardScoring = scorecardTemplateItem.ScorecardScoring,
                        Value = null,
                        Order = scorecardTemplateItem.Order
                    });
                }

                model.ScorecardRecordModels = model.ScorecardRecordModels.ToList();

                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Save comment on Scorecard based on id
        /// </summary>
        [HttpPost]
        public ScorecardCollectionModel ScorecardCommentSave(ScorecardCollectionModel model)
        {
            try
            {
                CheckModelState();

                ScorecardProvider.SaveEmployeeComment(model.ScorecardModel.ScorecardId, model.ScorecardModel.EmployeeMessage);

                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpPost]
        public ScorecardCollectionModel SaveScoreCardRecordEmployeeComment(ScorecardCollectionModel model)
        {
            try
            {
                CheckModelState();

                foreach (var record in model.ScorecardRecordModels)
                {
                    ScorecardProvider.SaveScoreCardRecordEmployeeComment(record.ScorecardRecordId, record.EmployeeHtmlComment);
                }

                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }


        /// <summary>
        /// Save Scorecard Record
        /// </summary>
        [HttpPost]
        public ScorecardCollectionModel ScorecardRecordSave(ScorecardCollectionModel model)
        {
            try
            {
                CheckModelState();

                Context.BeginTransaction();

                foreach (var scorecardRecordModel in model.ScorecardRecordModels)
                {
                    /* Check if record does contain a value (either aei or manual) and mark it completed */
                    bool completed = false;
                    if (scorecardRecordModel.Value != null || scorecardRecordModel.ScoreType != null)
                        completed = true;

                    /* Make sure value for manual score is between min and max */
                    if (scorecardRecordModel.Value != null && (scorecardRecordModel.Value < scorecardRecordModel.Minimum || scorecardRecordModel.Value > scorecardRecordModel.Maximum))
                        throw new ScorecardTemplateException(
                               "One or more of the manual scores contains a value outside the allowed Minimum and Maximum");
                    ScorecardProvider.SaveScorecardRecord(scorecardRecordModel.ScorecardRecordId,
                        model.ScorecardModel.ScorecardId.Value,
                        scorecardRecordModel.ScorecardTemplateItemId, scorecardRecordModel.ScoreType, scorecardRecordModel.Value, completed, scorecardRecordModel.EvaluatorHtmlComment, scorecardRecordModel.EmployeeHtmlComment);
                }

                var record = ScorecardProvider.GetScorecard(model.ScorecardModel.ScorecardId.Value);

                // Check if Completed was saved previously cannot mark uncomplete after the fact
                bool scorecardCompleted = false;
                if (record.Completed)
                    scorecardCompleted = true;
                else
                    scorecardCompleted = model.ScorecardModel.Completed;

                ScorecardProvider.SaveScorecard(record.Id, record.ScorecardTemplateId, record.EvaluatorId,
                   record.EmployeeId, record.ScorecardTemplatePeriodId, record.Rated, scorecardCompleted, record.CreatedBy, record.DateCreated, model.ScorecardModel.EvaluatorMessage,
                    model.ScorecardModel.EmployeeMessage, record.VariableStart, record.VariableEnd, record.VariableYear);

                if (model.ScorecardModel.Completed)
                    if (!record.ScorecardRecords.All(a => a.Completed))
                        throw new ScorecardTemplateException(
                            "Scorecard cannot be marked as Complete, not all records have values.");
                    else
                        ScorecardProvider.SaveScorecard(record.Id, record.ScorecardTemplateId, record.EvaluatorId,
                   record.EmployeeId, record.ScorecardTemplatePeriodId, record.Rated, scorecardCompleted, record.CreatedBy, record.DateCreated, model.ScorecardModel.EvaluatorMessage,
                    model.ScorecardModel.EmployeeMessage, record.VariableStart, record.VariableEnd, record.VariableYear);

                Context.CommitTransaction();

                return model;
            }
            catch (ScorecardTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Scorecard
        /// </summary>
        [HttpPost]
        public void ScorecardDelete(ScorecardModel model)
        {
            try
            {
                ScorecardProvider.DeleteScoreCard(model.ScorecardId.Value);
            }
            catch (ScorecardException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Save Unsubmit status on Scorecard based on id
        /// </summary>
        [HttpPost]
        public void ScorecardUnsubmit(ScorecardModel model)
        {
            try
            {
                ScorecardProvider.UnsubmitScoreCard(model.ScorecardId.Value);
            }
            catch (ScorecardException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Save Unsubmit status on Scorecard based on id
        /// </summary>
        [HttpPost]
        public void ScorecardSubmit(ScorecardModel model)
        {
            try
            {
                var records = ScorecardProvider.ScorecardRecordList(model.ScorecardId.Value);

                if (records.Where(sr => sr.Completed == false).Count() > 0 || records.Count() < 1)
                {
                    throw new ScorecardException(
                            "Scorecard cannot be submitted, not all records have values.");
                }

                ScorecardProvider.SubmitScoreCard(model.ScorecardId.Value);
            }
            catch (ScorecardException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        public void ScoreCardLock(ScorecardModel model)
        {
            try
            {
                ScorecardProvider.LockScoreCard(model.ScorecardId.Value);
            }
            catch (ScorecardException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion

        #region Dropdown List

        /// <summary>
        /// Retrieve list of active Scorecard Templates
        /// </summary>
        [HttpGet]
        public List<ScorecardTemplateDropdownModel> ScorecardTemplateDropdown()
        {
            var returnList = new List<ScorecardTemplateDropdownModel>();
            returnList.AddRange(ScorecardTemplateProvider.ScorecardTemplateList().Where(s => s.IsActive == true)
                .Select(a => new ScorecardTemplateDropdownModel
                {
                    Id = a.Id,
                    ScorecardCode = a.ScorecardCode,
                    ScorecardName = a.ScorecardName
                }));
            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }

        /// <summary>
        /// Retrieve list of all Scorecard Templates
        /// </summary>
        [HttpGet]
        public List<ScorecardTemplateDropdownModel> ScorecardTemplateDropdownAll()
        {
            var returnList = new List<ScorecardTemplateDropdownModel>();
            returnList.AddRange(ScorecardTemplateProvider.ScorecardTemplateList()
                .Select(a => new ScorecardTemplateDropdownModel
                {
                    Id = a.Id,
                    ScorecardCode = a.ScorecardCode,
                    ScorecardName = a.ScorecardName,
                    Active = a.IsActive
                }));
            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }

        /// <summary>
        /// Retrieve list of all Scorecard Templates for years specified
        /// </summary>
        [HttpPost]
        public List<ScorecardTemplateDropdownModel> ScorecardTemplateDropdownListYearMultiple(int[] years)
        {
            var returnList = new List<ScorecardTemplateDropdownModel>();
            returnList.AddRange(ScorecardTemplateProvider.ScorecardTemplateDropdownListYearMultiple(years)
                .Select(a => new ScorecardTemplateDropdownModel
                {
                    Id = a.Id,
                    ScorecardCode = a.ScorecardCode,
                    ScorecardName = a.ScorecardName,
                    Active = a.IsActive
                }));
            return returnList.ToList().OrderBy(a => a.Description).Distinct().ToList();
        }

        /// <summary>
        /// Retrieve list of all Scorecard Template Periods
        /// </summary>
        [HttpGet]
        public List<ScorecardTemplatePeriodDropdownModel> ScorecardTemplatePeriodDropdown(Guid id)
        {
            var returnList = new List<ScorecardTemplatePeriodDropdownModel>();
            returnList.AddRange(ScorecardTemplateProvider.ScorecardTemplatePeriodList(new List<Guid> { id })
                .Select(a => new ScorecardTemplatePeriodDropdownModel
                {
                    Id = a.Id,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    IsVariable = a.IsVariable,
                    Name = a.Description,
                    ReportSortOrder = a.ReportSortOrder
                }));
            return returnList.ToList().OrderBy(a => a.ReportSortOrder).ThenBy(a => a.Description).ToList();
        }

        /// <summary>
        /// Retrieve list of all years that have been assigned to Scorecard Template Periods
        /// </summary>
        [HttpGet]
        //[Route("api/scorecard/ScorecardTemplatePeriodDropdownYear/{year}")]
        public List<int> ScorecardTemplatePeriodDropdownYear()
        {
            return ScorecardTemplateProvider.ScorecardTemplatePeriodYearList().OrderByDescending(a => a).ToList();
        }

        /// <summary>
        /// Retrieve list of all Scorecard Template Periods for a specific year
        /// </summary>
        [HttpGet]
        [Route("api/scorecard/ScorecardTemplatePeriodDropdownYear/{year}")]
        public List<ScorecardTemplatePeriodDropdownModel> ScorecardTemplatePeriodDropdownYear(int year)
        {
            var returnList = new List<ScorecardTemplatePeriodDropdownModel>();
            returnList.AddRange(ScorecardTemplateProvider.ScorecardTemplatePeriodList(year)
                .Select(a => new ScorecardTemplatePeriodDropdownModel
                {
                    Id = a.Id,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    IsVariable = a.IsVariable,
                    Name = a.Description,
                    ReportSortOrder = a.ReportSortOrder
                }));
            return returnList.ToList().OrderBy(a => a.ReportSortOrder).ThenBy(a => a.StartDate).ToList();
        }

        /// <summary>
        /// Retrieve list of all Scorecard Template Periods for a specific year
        /// </summary>
        [HttpPost]
        public List<ScorecardTemplatePeriodDropdownModel> ScorecardTemplatePeriodDropdownYearMultiple(int[] years)
        {
            var returnList = new List<ScorecardTemplatePeriodDropdownModel>();
            returnList.AddRange(ScorecardTemplateProvider.ScorecardTemplatePeriodListMultiple(years)
                .Select(a => new ScorecardTemplatePeriodDropdownModel
                {
                    Id = a.Id,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    IsVariable = a.IsVariable,
                    Name = a.Description,
                    ScorecardName = a.ScorecardTemplate.ScorecardName,
                    ReportSortOrder = a.ReportSortOrder
                }));
            return returnList.ToList().OrderBy(a => a.ScorecardName).ThenBy(a => a.ReportSortOrder).ThenBy(a => a.StartDate).ToList();
        }

        /// <summary>
        /// Retrieve list of all Scorecard Template Periods for a specific year
        /// </summary>
        [HttpPost]
        public List<ScorecardTemplatePeriodDropdownModel> ScorecardTemplatePeriodSearchDropdownList(ScorecardTemplatePeriodSearchModel model)
        {
            var returnList = new List<ScorecardTemplatePeriodDropdownModel>();
            returnList.AddRange(ScorecardTemplateProvider.ScorecardTemplatePeriodList(model.ScorecardTemplateItemIds).Where(p => model.ReviewYears.Contains(p.ReviewYear))
                .Select(a => new ScorecardTemplatePeriodDropdownModel
                {
                    Id = a.Id,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    IsVariable = a.IsVariable,
                    Name = a.Description,
                    ScorecardName = a.ScorecardTemplate.ScorecardName,
                    ReportSortOrder = a.ReportSortOrder
                }));
            return returnList.ToList().OrderBy(a => a.ScorecardName).ThenBy(a => a.ReportSortOrder).ThenBy(a => a.StartDate).ToList();
        }

        [HttpGet]
        public List<int> ScorecardTemplateYearDropdownList(Guid id)
        {
            var years = ScorecardTemplateProvider.ScorecardTemplatePeriodList(new List<Guid> { id }).Select(a => a.ReviewYear).Distinct().OrderByDescending(a => a).ToList();
            return years;
        }

        /// <summary>
        /// Retrieve list of all Scorecard Template Periods for a specific year
        /// </summary>
        [HttpPost]
        public List<ScorecardTemplatePeriodDropdownModel> ScorecardTemplateYearDropdownList(int[] years)
        {
            var returnList = new List<ScorecardTemplatePeriodDropdownModel>();
            returnList.AddRange(ScorecardTemplateProvider.ScorecardTemplatePeriodListMultiple(years)
                .Select(a => new ScorecardTemplatePeriodDropdownModel
                {
                    Id = a.Id,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    IsVariable = a.IsVariable,
                    Name = a.Description,
                    ScorecardName = a.ScorecardTemplate.ScorecardName,
                    ReportSortOrder = a.ReportSortOrder
                }));
            return returnList.ToList().OrderBy(a => a.ScorecardName).ThenBy(a => a.ReportSortOrder).ThenBy(a => a.StartDate).ToList();
        }

        #endregion
    }
}