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

            var cycleIds = returnList.Select(a => a.Id).ToList();
            var exemptions = BillingCycleProvider.ExemptionList()
                .Where(e => cycleIds.Contains(e.BillingCycleEntryId))
                .Select(e => new { e.BillingCycleEntryId, Name = e.UserAccount.FirstName + " " + e.UserAccount.Surname })
                .ToList();
            foreach (var row in returnList)
            {
                row.ExemptUserNames = exemptions.Where(e => e.BillingCycleEntryId == row.Id)
                    .Select(e => e.Name).OrderBy(n => n).ToList();
                row.ExemptUserCount = row.ExemptUserNames.Count;
            }

            return new GridResultModel<BillingCycleGridModel>(returnList, totalNumberOfRecords);
        }

        private BillingCycleModel BuildBillingCycleModel(Guid id)
        {
            var record = BillingCycleProvider.GetBillingCycleEntry(id);
            if (record == null)
                throw new BillingCyleException("The billing cycle could not be found.");

            var exemptUsers = BillingCycleProvider.ExemptionList()
                .Where(e => e.BillingCycleEntryId == id)
                .Select(e => new BillingCycleExemptUserModel
                {
                    Id = e.UserAccountId,
                    Name = e.UserAccount.FirstName + " " + e.UserAccount.Surname,
                    Active = e.UserAccount.Active
                })
                .OrderBy(e => e.Name)
                .ToList();

            return new BillingCycleModel
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
                IsActive = record.IsActive,
                ExemptUsers = exemptUsers,
                ExemptUserAccountIds = exemptUsers.Select(e => e.Id).ToList()
            };
        }

        /// <summary>
        /// Retrieve single Billingcycle based on id
        /// </summary>
        [HttpGet]
        public BillingCycleModel BillingCycleGet(Guid? id)
        {
            try
            {
                return BuildBillingCycleModel(id.Value);
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

                var ownsTransaction = !Context.IsTransactionActive();
                if (ownsTransaction)
                    Context.BeginTransaction();
                Guid savedId;
                try
                {
                    var record = BillingCycleProvider.SaveBillingCycleEntry(model.Id, model.Cycle,
                        model.Year, model.StartDate, model.EndDate, model.Weekdays,
                        model.PublicHolidays, model.WorkDays, model.IsClosed);

                    if (model.ExemptUserAccountIds != null)
                        BillingCycleProvider.SaveExemptions(record.Id, model.ExemptUserAccountIds);

                    if (ownsTransaction)
                        Context.CommitTransaction();
                    savedId = record.Id;
                }
                catch
                {
                    if (ownsTransaction)
                        Context.RollbackTransaction();
                    throw;
                }

                return BuildBillingCycleModel(savedId);
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

        /// <summary>
        /// Close or reopen a Billingcycle
        /// </summary>
        [HttpPost]
        public BillingCycleModel BillingCycleSetStatus(BillingCycleStatusModel model)
        {
            try
            {
                BillingCycleProvider.SetClosed(model.Id, model.IsClosed);
                return BuildBillingCycleModel(model.Id);
            }
            catch (BillingCyleException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Whether the user's timesheet is read-only in the Billingcycle (closed and the user is not exempt)
        /// </summary>
        [HttpGet]
        public BillingCycleAccessModel BillingCycleAccess(Guid? id, Guid? userId)
        {
            if (id == null || userId == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "A billing cycle and user are required."));

            var model = new BillingCycleAccessModel { BillingCycleId = id.Value, UserAccountId = userId.Value };

            // Inactive cycles do not block capture, so they are reported as open.
            var record = BillingCycleProvider.GetBillingCycleEntry(id.Value);
            if (record == null)
                return model;

            model.StartDate = record.StartDate;
            model.EndDate = record.EndDate;
            model.IsClosed = record.IsClosed;
            model.IsExempt = BillingCycleProvider.IsUserExempt(record.Id, userId.Value);
            model.IsReadOnly = model.IsClosed && !model.IsExempt;

            var range = record.StartDate.ToString("yyyy/MM/dd") + " – " + record.EndDate.ToString("yyyy/MM/dd");
            if (model.IsReadOnly)
                model.Message = "Billing period " + range + " is closed. This timesheet is read-only.";
            else if (model.IsClosed)
                model.Message = "Billing period " + range + " is closed, but this user is exempt and can still capture time.";

            return model;
        }

        #endregion

        #region Exemptions

        /// <summary>
        /// Active users that can be exempted from a closed Billingcycle
        /// </summary>
        [HttpGet]
        public List<BillingCycleExemptUserModel> BillingCycleExemptionCandidates()
        {
            return BillingCycleProvider.ExemptionCandidates()
                .Where(a => !a.FirstName.Equals("Importer"))
                .Select(a => new BillingCycleExemptUserModel
                {
                    Id = a.Id,
                    Name = a.FirstName + " " + a.Surname,
                    Active = a.Active
                })
                .OrderBy(a => a.Name)
                .ToList();
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
                        Year = a.Year,
                        IsClosed = a.IsClosed
                    }).OrderByDescending(b => b.Startdate)
                .ToList();
        }

        #endregion
    }
}