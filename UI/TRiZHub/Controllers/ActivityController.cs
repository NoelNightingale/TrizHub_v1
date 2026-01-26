#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.ActivityData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.ActivityModels;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class ActivityController : TCRControllerBase
    {
        #region Ctor

        public ActivityController()
        {
            AppSettings = new AppSettings(Context);
            ActivityProvider = new ActivityProvider(Context, CurrentUser);
        }

        public ActivityController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            ActivityProvider = new ActivityProvider(Context, CurrentUser);
        }

        private IAppSettings AppSettings { get; }
        private IActivityProvider ActivityProvider { get; }

        #endregion

        #region Dropdown List

        /// <summary>
        /// Retrieve list of all Activities ordered by Description
        /// </summary>
        [HttpGet]
        public List<ActivityDropdownModel> ActivityDropdown()
        {
            var returnList = new List<ActivityDropdownModel>();
            returnList.AddRange(
                ActivityProvider.ActivityList().Where(a => a.IsActive == true)
                    .Select(a => new ActivityDropdownModel {Id = a.Id, ActivityName = a.ActivityName}));
            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }

        #endregion

        #region Activity 

        /// <summary>
        /// Create or update Activity
        /// </summary>
        [HttpPost]
        public ActivityEditModel SaveActivity(ActivityEditModel model)
        {
            try
            {
                CheckModelState();

                if (model.Id == null || model.Id == Guid.Empty)
                {
                    var activity = ActivityProvider.GetActivity(model.Id ?? Guid.Empty);
                    if (activity != null)
                        model.Id = activity.Id;
                }

                CheckModelState();

                var record = ActivityProvider.SaveActivity(model.Id,
                    model.ActivityName, model.IsActive);

                var activityReturn = new ActivityEditModel
                {
                    Id = record.Id,
                    ActivityName = record.ActivityName,
                    IsActive = record.IsActive,
                };

                return activityReturn;
            }
            catch (ActivityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve single Activity based on id
        /// </summary>
        [HttpGet]
        public ActivityEditModel GetActivity(Guid id)
        {
            try
            {
                var activity = ActivityProvider.GetActivity(id);

                var model = new ActivityEditModel
                {
                    Id = activity.Id,
                    ActivityName = activity.ActivityName,
                    IsActive = activity.IsActive
                };
                return model;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retieve list of all Activities filtered and sorted based on input values
        /// </summary>
        [HttpPost]
        public GridResultModel<ActivityGridModel> ActivityGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = ActivityProvider.ActivityList()
                .Select(a => new ActivityGridModel
                {
                    Id = a.Id,
                    ActivityName = a.ActivityName,
                    IsActive = a.IsActive,

                });


            var totalNumberOfRecords = filteredQuery.Count();

            //filteredQuery = filteredQuery.OrderBy(a => a.ActivityName);
            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.ActivityName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "activityname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ActivityName)
                        : filteredQuery.OrderByDescending(r => r.ActivityName);
                    break;
                case "isactive":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.IsActive)
                        : filteredQuery.OrderByDescending(r => r.IsActive);
                    break;
            }

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.ActivityName.Contains(model.Searchfor));
            }

            var returnList = filteredQuery.ToList();

            return new GridResultModel<ActivityGridModel>(returnList, totalNumberOfRecords);
        }

        #endregion
    }
}