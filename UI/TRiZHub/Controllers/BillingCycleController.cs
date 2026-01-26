#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TCR.Lib.Utility;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.BillingCycleData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.BillingCycleModels;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class BillingCycleController : TCRControllerBase
    {
        #region Ctor

        public BillingCycleController()
        {
            AppSettings = new AppSettings(Context);
            BillingCycleProvider = new BillingCycleProvider(Context, CurrentUser);
        }

        public BillingCycleController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            BillingCycleProvider = new BillingCycleProvider(Context, CurrentUser);
        }

        private IAppSettings AppSettings { get; }
        private IBillingCycleProvider BillingCycleProvider { get; }

        #endregion

        #region Billing Cycle

        /// <summary>
        /// Retrieve list of active Billingcycles filtered and sorted based on input values
        /// </summary>
        [HttpPost]
        public GridResultModel<BillingCycleGridModel> BillingCycleGrid(GridModel model)
        {
            var filteredQuery = BillingCycleProvider.BillingCycleFillterList()
                .Where(a => a.IsActive).Select(a => new BillingCycleGridModel
                {
                    Id = a.Id,
                    Cycle = a.Cycle,
                    Year = a.Year,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Weekdays = a.Weekdays,
                    PublicHolidays = a.PublicHolidays,
                    WorkDays = a.WorkDays,
                    CreatedByAccountId = a.CreatedByAccountId,
                    DateCreated = a.DateCreated,
                    IsClosed = a.IsClosed,
                    IsActive = a.IsActive
                });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.StartDate.Year.ToString() == model.Searchfor);
            }


            var totalNumberOfRecords = filteredQuery.Count();

            //filteredQuery = filteredQuery.OrderBy(a => a.Cycle);
            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.Cycle); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "cycle":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Cycle)
                        : filteredQuery.OrderByDescending(r => r.Cycle);
                    break;
                case "year":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Year)
                        : filteredQuery.OrderByDescending(r => r.Year);
                    break;
                case "startdate":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.StartDate)
                        : filteredQuery.OrderByDescending(r => r.StartDate);
                    break;
                case "enddate":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EndDate)
                        : filteredQuery.OrderByDescending(r => r.EndDate);
                    break;
                case "weekdays":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Weekdays)
                        : filteredQuery.OrderByDescending(r => r.Weekdays);
                    break;
                case "publicholidays":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.PublicHolidays)
                        : filteredQuery.OrderByDescending(r => r.PublicHolidays);
                    break;
                case "workdays":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.WorkDays)
                        : filteredQuery.OrderByDescending(r => r.WorkDays);
                    break;
                case "isclosed":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.IsClosed)
                        : filteredQuery.OrderByDescending(r => r.IsClosed);
                    break;

            }

            var returnList = filteredQuery.ToList();

            return new GridResultModel<BillingCycleGridModel>(returnList, totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve single Billingcycle based on id
        /// </summary>
        [HttpGet]
        public BillingCycleModel BillingCycleGet(Guid? id)
        {
            try
            {
                var record = BillingCycleProvider.GetBillingCycleEntry(id.Value);

                var model = new BillingCycleModel
                {
                    Id = record.Id,
                    Cycle = record.Cycle,
                    Year = record.Year,
                    StartDate = record.StartDate,
                    EndDate = record.EndDate,
                    Weekdays = record.Weekdays,
                    PublicHolidays = record.PublicHolidays,
                    WorkDays = record.WorkDays,
                    CreatedByAccountId = record.CreatedByAccountId,
                    DateCreated = record.DateCreated,
                    IsClosed = record.IsClosed,
                    IsActive = record.IsActive
                };

                return model;
            }
            catch (BillingCyleException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Create or update Billingcycle
        /// </summary>
        [HttpPost]
        public BillingCycleModel BillingCycleSave(BillingCycleModel model)
        {
            try
            {
                if (model.Id == null || model.Id == Guid.Empty)
                {
                    var billingCycle = BillingCycleProvider.GetBillingCycleEntry(model.Id ?? Guid.Empty);
                    if (billingCycle != null)
                        model.Id = billingCycle.Id;
                }

                CheckModelState();

                var record = BillingCycleProvider.SaveBillingCycleEntry(model.Id, model.Cycle,
                    model.Year, model.StartDate, model.EndDate, model.Weekdays,
                    model.PublicHolidays, model.WorkDays, model.IsClosed);

                var result = BillingCycleProvider.GetBillingCycleEntry(record.Id);

                var returnResult = new BillingCycleModel
                {
                    Id = result.Id,
                    Cycle = result.Cycle,
                    Year = result.Year,
                    StartDate = result.StartDate,
                    EndDate = result.EndDate,
                    Weekdays = result.Weekdays,
                    PublicHolidays = result.PublicHolidays,
                    WorkDays = result.WorkDays,
                    CreatedByAccountId = result.CreatedByAccountId,
                    DateCreated = result.DateCreated,
                    IsClosed = result.IsClosed,
                    IsActive = result.IsActive
                };

                return returnResult;
            }
            catch (BillingCyleException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Create or update list of BillingCycles
        /// </summary>
        [HttpPost]
        public void BillingCycleListSave(List<BillingCycleModel> model)
        {
            try
            {
                foreach (var billingCycleModel in model)
                {
                    if (billingCycleModel.Id == null || billingCycleModel.Id == Guid.Empty)
                    {
                        var billingCycle = BillingCycleProvider.GetBillingCycleEntry(billingCycleModel.Id ?? Guid.Empty);
                        if (billingCycle != null)
                            billingCycleModel.Id = billingCycle.Id;
                    }
                }

                CheckModelState();

                if (!Context.IsTransactionActive())
                    Context.BeginTransaction();
                foreach (var billingCycleModel in model)
                {
                    BillingCycleProvider.SaveBillingCycleEntry(billingCycleModel.Id, billingCycleModel.Cycle,
                        billingCycleModel.Year
                        , billingCycleModel.StartDate, billingCycleModel.EndDate, billingCycleModel.Weekdays,
                        billingCycleModel.PublicHolidays
                        , billingCycleModel.WorkDays, billingCycleModel.IsClosed);
                }
                Context.CommitTransaction();
            }
            catch (BillingCyleException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Billingcycle based on id
        /// </summary>
        [HttpPost]
        public void BillingCycleDelete(BillingCycleModel model)
        {
            try
            {
                BillingCycleProvider.DeleteBillingCycleEntry(model.Id.Value);
            }
            catch (BillingCyleException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }




        #endregion

        #region Dropdown List

        /// <summary>
        /// Retrieve list of Billingcycles ordered by Startdate
        /// </summary>
        [HttpGet]
        public List<BillingCycleDropdownModel> BillingCycleDropdown()
        {
            return BillingCycleProvider.BillingCycleFillterList()
                .Select(a =>
                    new BillingCycleDropdownModel
                    {
                        Id = a.Id,
                        Cycle = a.Cycle,
                        Enddate = a.EndDate,
                        Startdate = a.StartDate,
                        Year = a.Year
                    }).OrderByDescending(b => b.Startdate)
                .ToList();
        }

        #endregion
    }
}