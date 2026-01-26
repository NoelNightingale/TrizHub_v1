#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.ProjectData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Provider.TimesheetData;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.ReportModels;
using TRiZHub.Models.TimesheetModels;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class TimesheetController : TCRControllerBase
    {
        #region Ctor

        public TimesheetController()
        {
            AppSettings = new AppSettings(Context);
            TimesheetProvider = new TimesheetProvider(Context, CurrentUser);
            ProjectProvider = new ProjectProvider(Context, CurrentUser);
        }

        public TimesheetController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            TimesheetProvider = new TimesheetProvider(Context, CurrentUser);
            ProjectProvider = new ProjectProvider(Context, CurrentUser);
        }

        private IAppSettings AppSettings { get; }
        private ITimesheetProvider TimesheetProvider { get; }
        private IProjectProvider ProjectProvider { get; }

        #endregion

        #region Timesheet

        /// <summary>
        /// Retrieve list of Timesheets based on filter and sort input values
        /// </summary>
        [HttpPost]
        public GridResultModel<TimesheetGridModel> TimesheetGrid(TimesheetSearchModel model)
        {
            var begin = SetupGridParams(model);
            if (model.UserId == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "Filter options needs to be provided."));
            if (model.StartDate == null) model.StartDate = new DateTime();
            if (model.EndDate == null) model.EndDate = DateTime.MaxValue;


            var filteredQuery = TimesheetProvider.TimesheetFilterList(model.StartDate.Value, model.EndDate.Value)
                .Where(a => a.UserAccountId == model.UserId).Select(a => new TimesheetGridModel
                {
                    Id = a.Id,
                    UserAccountId = a.UserAccountId,
                    UserAccountName = a.UserAccount.AccountName,
                    ProjectId = a.ProjectId,
                    ProjectName = a.Project.ProjectName,
                    //ProjectDescription = (a.Project.ProjectNumber == null || a.Project.ProjectNumber.Equals("")) ? a.Project.ProjectName : ("[" + a.Project.ProjectNumber + "] " + a.Project.ProjectName),
                    SubProjectId = a.SubProjectId,
                    SubProjectName = a.SubProject != null ? a.SubProject.ProjectName : "",
                    ProjectDescription = a.SubProject.SubProjectNumber == null ? (a.Project.ProjectNumber == null || a.Project.ProjectNumber.Equals("")) ? a.Project.ProjectName : ("[" + a.Project.ProjectNumber + "] " + a.Project.ProjectName) : (a.Project.ProjectNumber == null || a.Project.ProjectNumber.Equals("")) ? a.Project.ProjectName : ("[" + a.Project.ProjectNumber + "-" + a.SubProject.SubProjectNumber + "] " + a.Project.ProjectName + " [" + a.SubProject.ProjectName + "]"),
                    Project = new Project()
                    {
                        Id = a.ProjectId,
                        IsActive = a.IsActive,
                        ProjectId = a.ProjectId,
                        ProjectName = a.Project.ProjectName,
                        Description = a.SubProject.SubProjectNumber == null ? (a.Project.ProjectNumber == null || a.Project.ProjectNumber.Equals("")) ? a.Project.ProjectName : ("[" + a.Project.ProjectNumber + "] " + a.Project.ProjectName) : (a.Project.ProjectNumber == null || a.Project.ProjectNumber.Equals("")) ? a.Project.ProjectName : ("[" + a.Project.ProjectNumber + "-" + a.SubProject.SubProjectNumber + "] " + a.Project.ProjectName + " [" + a.SubProject.ProjectName + "]"),
                        SubProjectId = a.SubProjectId,
                        SubProjectName = a.SubProject.ProjectName
                    },
                    TeamId = a.TeamId,
                    TeamName = a.Team.TeamName,
                    ActivityId = a.ActivityId,
                    ActivityName = a.Activity.ActivityName,
                    ClientEntityId = a.Project.ClientId,
                    ClientEntityName = a.Project.Client.EntityName,
                    Comments = a.Comments,
                    DateEntry = a.DateEntry,
                    Hours = a.Hours,
                    ProjectGridId = a.SubProjectId != null ? a.SubProjectId.Value : a.ProjectId,
                    Billable = a.Project.Billable
                });

            if (model.ProjectId != null)
                filteredQuery = filteredQuery.Where(a => a.ProjectId == model.ProjectId);

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.ProjectName.Contains(model.Searchfor));
            }

            // Billable check
            if (model.BillingOption == 1)
            {
                filteredQuery = filteredQuery.Where(a => a.Billable);
            }
            else if(model.BillingOption == 2) {
                filteredQuery = filteredQuery.Where(a => !a.Billable);
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.DateEntry); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "project":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ProjectName)
                        : filteredQuery.OrderByDescending(r => r.ProjectName);
                    break;
                case "client":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ClientEntityName)
                        : filteredQuery.OrderByDescending(r => r.ClientEntityName);
                    break;
                case "billable":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Billable)
                        : filteredQuery.OrderByDescending(r => r.Billable);
                    break;
                case "subprojectname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.SubProjectName)
                        : filteredQuery.OrderByDescending(r => r.SubProjectName);
                    break;
                case "team":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.TeamName)
                        : filteredQuery.OrderByDescending(r => r.TeamName);
                    break;
                case "activity":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ActivityName)
                        : filteredQuery.OrderByDescending(r => r.ActivityName);
                    break;
                case "cliententityname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ClientEntityName)
                        : filteredQuery.OrderByDescending(r => r.ClientEntityName);
                    break;
                case "comments":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Comments)
                        : filteredQuery.OrderByDescending(r => r.Comments);
                    break;
                case "date":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.DateEntry)
                        : filteredQuery.OrderByDescending(r => r.DateEntry);
                    break;
                case "hours":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Hours)
                        : filteredQuery.OrderByDescending(r => r.Hours);
                    break;
            }


            //filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);
            //filteredQuery = filteredQuery.Skip(begin).Take(totalNumberOfRecords);

            return new GridResultModel<TimesheetGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve single Timesheet entry based on id
        /// </summary>
        [HttpGet]
        public TimesheetModel TimesheetGet(Guid? id)
        {
            try
            {
                var record = TimesheetProvider.GetTimesheetEntry(id.Value);

                var model = new TimesheetModel
                {
                    Id = record.Id,
                    UserAccountId = record.UserAccountId,
                    UserAccountName = record.UserAccount.AccountName,
                    ProjectId = record.ProjectId,
                    ProjectName = record.Project.ProjectName,
                    SubProjectId = record.SubProjectId,
                    SubProjectName = record.SubProject != null ? record.SubProject.ProjectName : "",
                    TeamId = record.TeamId,
                    TeamName = record.Team.TeamName,
                    ActivityId = record.ActivityId,
                    ActivityName = record.Activity.ActivityName,
                    ClientEntityId = record.Project.ClientId,
                    ClientEntityName = record.Project.Client.EntityName,
                    Comments = record.Comments,
                    DateEntry = record.DateEntry,
                    Hours = record.Hours
                };

                return model;
            }
            catch (TimesheetException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Create or update Timesheet entry
        /// </summary>
        [HttpPost]
        public TimesheetModel TimesheetSave(TimesheetModel model)
        {
            try
            {
                if (model.Id == null || model.Id == Guid.Empty)
                {
                    var project = ProjectProvider.GetProject(model.ProjectGridId ?? Guid.Empty);
                    var subProject = ProjectProvider.GetSubProject(model.ProjectGridId ?? Guid.Empty);
                    if (project != null)
                        model.ProjectId = project.Id;
                    if (subProject != null)
                        model.SubProjectId = subProject.Id;
                }

                CheckModelState();


                var record = TimesheetProvider.SaveTimesheetEntry(model.Id, model.UserAccountId,
                    model.ProjectId.Value, model.SubProjectId, model.TeamId, model.ActivityId,
                    model.Comments, model.Hours, model.DateEntry.ToLocalTime());

                var result = TimesheetProvider.GetTimesheetEntry(record.Id);

                var returnResult = new TimesheetModel
                {
                    Id = result.Id,
                    UserAccountId = result.UserAccountId,
                    UserAccountName = result.UserAccount.AccountName,
                    ProjectId = result.ProjectId,
                    ProjectName = result.Project.ProjectName,
                    SubProjectId = result.SubProjectId,
                    SubProjectName = result.SubProject != null ? result.SubProject.ProjectName : "",
                    TeamId = result.TeamId,
                    TeamName = result.Team.TeamName,
                    ActivityId = result.ActivityId,
                    ActivityName = result.Activity.ActivityName,
                    ClientEntityId = result.Project.ClientId,
                    ClientEntityName = result.Project.Client.EntityName,
                    Comments = result.Comments,
                    DateEntry = result.DateEntry,
                    Hours = result.Hours
                };

                return returnResult;
            }
            catch (TimesheetException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Create or update list of Timesheet entries
        /// </summary>
        [HttpPost]
        public void TimesheetListSave(List<TimesheetModel> model)
        {
            try
            {
                foreach (var timesheetModel in model)
                {
                    if (timesheetModel.Id == null || timesheetModel.Id == Guid.Empty)
                    {
                        var project = ProjectProvider.GetProject(timesheetModel.ProjectGridId ?? Guid.Empty);
                        var subProject = ProjectProvider.GetSubProject(timesheetModel.ProjectGridId ?? Guid.Empty);
                        if (project != null)
                            timesheetModel.ProjectId = project.Id;
                        if (subProject != null)
                        {
                            timesheetModel.ProjectId = subProject.ProjectId;
                            timesheetModel.SubProjectId = subProject.Id;
                        }
                    }
                }

                CheckModelState();

                if (!Context.IsTransactionActive())
                    Context.BeginTransaction();
                foreach (var timesheetModel in model)
                {
                    TimesheetProvider.SaveTimesheetEntry(timesheetModel.Id, timesheetModel.UserAccountId,
                        timesheetModel.ProjectId.Value, timesheetModel.SubProjectId, timesheetModel.TeamId,
                        timesheetModel.ActivityId, timesheetModel.Comments, timesheetModel.Hours,
                        timesheetModel.DateEntry);
                }
                Context.CommitTransaction();
            }
            catch (TimesheetException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Timesheet entry
        /// </summary>
        [HttpPost]
        public void TimesheetDelete(TimesheetModel model)
        {
            try
            {
                TimesheetProvider.DeleteTimesheetEntry(model.Id.Value);
            }
            catch (TimesheetException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion
    }
}