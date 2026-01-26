#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Provider.TeamData;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.TeamModels;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class TeamController : TCRControllerBase
    {
        #region Ctor

        public TeamController()
        {
            AppSettings = new AppSettings(Context);
            TeamProvider = new TeamProvider(Context, CurrentUser);
        }

        public TeamController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            TeamProvider = new TeamProvider(Context, CurrentUser);
        }

        private IAppSettings AppSettings { get; }
        private ITeamProvider TeamProvider { get; }

        #endregion

        #region Team

        #endregion

        #region Dropdown List

        /// <summary>
        /// Retrieve list of active Teams
        /// </summary>
        [HttpGet]
        public List<TeamDropdownModel> TeamDropdown()
        {
            var returnList = new List<TeamDropdownModel>();
            returnList.AddRange(
                TeamProvider.TeamList().Where(t => t.IsActive == true).Select(a => new TeamDropdownModel {Id = a.Id, TeamName = a.TeamName}));
            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }

        #endregion

        #region Team
        /// <summary>
        /// Create or update team
        /// </summary>
        [HttpPost]
        public TeamEditModel SaveTeam(TeamEditModel model)
        {
            try
            {
                CheckModelState();

                if (model.Id == null || model.Id == Guid.Empty)
                {
                    var team = TeamProvider.GetTeam(model.Id ?? Guid.Empty);
                    if (team != null)
                        model.Id = team.Id;
                }

                CheckModelState();

                var record = TeamProvider.SaveTeam(model.Id,
                    model.TeamName, model.IsActive);

                var teamReturn = new TeamEditModel
                {
                    Id = record.Id,
                    TeamName = record.TeamName,
                    IsActive = record.IsActive,
                };

                return teamReturn;
            }
            catch (TeamException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve single Team based on ID
        /// </summary>
        [HttpGet]
        public TeamEditModel GetTeam(Guid id)
        {
            try
            {
                var team = TeamProvider.GetTeam(id);

                var model = new TeamEditModel
                {
                    Id = team.Id,
                    TeamName = team.TeamName,
                    IsActive = team.IsActive
                };
                return model;
            }
            catch (TeamException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }


        /// <summary>
        /// Retrieve list of all Teams based on filter and sort input values
        /// </summary>
        [HttpPost]
        public GridResultModel<TeamGridModel> TeamGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = TeamProvider.TeamList()
                .Select(a => new TeamGridModel
                {
                    Id = a.Id,
                    TeamName = a.TeamName,
                    IsActive = a.IsActive,

                });


            var totalNumberOfRecords = filteredQuery.Count();

            //filteredQuery = filteredQuery.OrderBy(a => a.TeamName);
            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.TeamName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "teamname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.TeamName)
                        : filteredQuery.OrderByDescending(r => r.TeamName);
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
                        r => r.TeamName.Contains(model.Searchfor));
            }

            var returnList = filteredQuery.ToList();

            return new GridResultModel<TeamGridModel>(returnList, totalNumberOfRecords);
        }

        #endregion


    }
}