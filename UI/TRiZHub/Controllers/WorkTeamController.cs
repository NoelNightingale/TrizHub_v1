#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TCR.Lib.BL;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.WorkTeamData;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.ProjectModels;
using TRiZHub.Models.WorkTeamModels;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class WorkTeamController : TCRControllerBase
    {
        #region Ctor

        public WorkTeamController()
        {
            WorkTeamProvider = new WorkTeamProvider(Context, CurrentUser);
            WorkTeamAccessProvider = new WorkTeamAccessProvider(Context, CurrentUser);
        }

        public WorkTeamController(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            WorkTeamProvider = new WorkTeamProvider(Context, CurrentUser);
            WorkTeamAccessProvider = new WorkTeamAccessProvider(Context, CurrentUser);
        }

        private IWorkTeamProvider WorkTeamProvider { get; }
        private IWorkTeamAccessProvider WorkTeamAccessProvider { get; }

        #endregion

        #region Helpers

        private T Guarded<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (WorkTeamException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (GenericSecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, e.Message));
            }
        }

        private static DateTime DayOf(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
        }

        private static DateTime? DayOf(DateTime? value)
        {
            return value.HasValue ? DayOf(value.Value) : (DateTime?) null;
        }

        #endregion

        #region Team

        /// <summary>
        /// Active teams the current user can maintain
        /// </summary>
        [HttpGet]
        public List<WorkTeamDropdownModel> WorkTeamDropdown()
        {
            return WorkTeamProvider.WorkTeamList()
                .Where(t => t.IsActive)
                .Select(t => new WorkTeamDropdownModel { Id = t.Id, Name = t.Name })
                .OrderBy(t => t.Name)
                .ToList();
        }

        /// <summary>
        /// Teams the current user can maintain, filtered and sorted
        /// </summary>
        [HttpPost]
        public GridResultModel<WorkTeamGridModel> WorkTeamGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var today = DateTime.Today;
            var filteredQuery = WorkTeamProvider.WorkTeamList()
                .Where(t => model.ShowInactive || t.IsActive)
                .Select(t => new WorkTeamGridModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    TeamTypeName = t.TeamType.TeamName,
                    CurrentPeople = t.Members
                        .Where(m => m.StartDate <= today && (m.EndDate == null || m.EndDate >= today))
                        .Select(m => m.UserAccountId)
                        .Distinct()
                        .Count(),
                    IsActive = t.IsActive
                });

            if (model.Searchfor != "null")
            {
                filteredQuery = filteredQuery.Where(r => r.Name.Contains(model.Searchfor)
                                                         || r.TeamTypeName.Contains(model.Searchfor));
            }

            var totalNumberOfRecords = filteredQuery.Count();

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            var ascending = model.SortOrder == "ASC";
            switch (model.SortKey)
            {
                case "teamtypename":
                    filteredQuery = ascending ? filteredQuery.OrderBy(r => r.TeamTypeName) : filteredQuery.OrderByDescending(r => r.TeamTypeName);
                    break;
                case "currentpeople":
                    filteredQuery = ascending ? filteredQuery.OrderBy(r => r.CurrentPeople) : filteredQuery.OrderByDescending(r => r.CurrentPeople);
                    break;
                case "isactive":
                    filteredQuery = ascending ? filteredQuery.OrderBy(r => r.IsActive) : filteredQuery.OrderByDescending(r => r.IsActive);
                    break;
                case "name":
                    filteredQuery = ascending ? filteredQuery.OrderBy(r => r.Name) : filteredQuery.OrderByDescending(r => r.Name);
                    break;
                default:
                    filteredQuery = filteredQuery.OrderBy(r => r.Name);
                    break;
            }

            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<WorkTeamGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Single team with the current user's permissions on it
        /// </summary>
        [HttpGet]
        public WorkTeamEditModel GetWorkTeam(Guid id)
        {
            return Guarded(() =>
            {
                var team = WorkTeamProvider.GetWorkTeam(id);
                return new WorkTeamEditModel
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    IsActive = team.IsActive,
                    TeamTypeId = team.TeamTypeId,
                    TeamTypeName = team.TeamType == null ? null : team.TeamType.TeamName,
                    AllowTeamTimesheets = team.AllowTeamTimesheets,
                    AllowTeamScorecards = team.AllowTeamScorecards,
                    LeadsManageAllocations = team.LeadsManageAllocations,
                    LeadsManageRates = team.LeadsManageRates,
                    Permissions = WorkTeamProvider.PermissionsFor(team.Id)
                };
            });
        }

        /// <summary>
        /// Create or update a team. Administrators edit everything; managers only the lead flags.
        /// </summary>
        [HttpPost]
        public WorkTeamEditModel SaveWorkTeam(WorkTeamEditModel model)
        {
            CheckModelState();

            return Guarded(() =>
            {
                var record = WorkTeamProvider.SaveWorkTeam(
                    model.Id == Guid.Empty ? null : model.Id,
                    model.Name,
                    model.Description,
                    model.IsActive,
                    model.TeamTypeId.Value,
                    model.AllowTeamTimesheets,
                    model.AllowTeamScorecards,
                    model.LeadsManageAllocations,
                    model.LeadsManageRates);

                return GetWorkTeam(record.Id);
            });
        }

        #endregion

        #region People

        /// <summary>
        /// Memberships of a team; ended memberships only when requested
        /// </summary>
        [HttpGet]
        public List<WorkTeamMemberModel> WorkTeamMembers(Guid id, bool includeEnded = false)
        {
            return Guarded(() =>
            {
                var today = DateTime.Today;
                var query = WorkTeamProvider.MemberList(id);
                if (!includeEnded)
                    query = query.Where(m => m.EndDate == null || m.EndDate >= today);

                return query
                    .Select(m => new WorkTeamMemberModel
                    {
                        Id = m.Id,
                        WorkTeamId = m.WorkTeamId,
                        UserAccountId = m.UserAccountId,
                        UserName = m.UserAccount.FirstName + " " + m.UserAccount.Surname,
                        Role = m.Role,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate,
                        IsCurrent = m.StartDate <= today && (m.EndDate == null || m.EndDate >= today)
                    })
                    .ToList()
                    .OrderBy(m => m.Role)
                    .ThenBy(m => m.UserName)
                    .ThenBy(m => m.StartDate)
                    .ToList();
            });
        }

        /// <summary>
        /// Add or change a dated membership
        /// </summary>
        [HttpPost]
        public WorkTeamMemberModel SaveWorkTeamMember(WorkTeamMemberModel model)
        {
            CheckModelState();

            return Guarded(() =>
            {
                var record = WorkTeamProvider.SaveMember(
                    model.Id == Guid.Empty ? null : model.Id,
                    model.WorkTeamId,
                    model.UserAccountId,
                    model.Role,
                    DayOf(model.StartDate),
                    DayOf(model.EndDate));

                model.Id = record.Id;
                model.StartDate = record.StartDate;
                model.EndDate = record.EndDate;
                return model;
            });
        }

        /// <summary>
        /// Remove a membership row entirely. Ending a membership (setting an end date) keeps history.
        /// </summary>
        [HttpGet]
        public bool DeleteWorkTeamMember(Guid id)
        {
            return Guarded(() =>
            {
                WorkTeamProvider.DeleteMember(id);
                return true;
            });
        }

        #endregion

        #region Timesheet defaults

        /// <summary>
        /// The user's team memberships (any role) overlapping [start, end] with each team's team type, so the timesheet
        /// can pre-select a team type when exactly one covers a row's date.
        /// </summary>
        [HttpGet]
        public List<WorkTeamTypePeriod> TeamTypesForUser(Guid id, DateTime start, DateTime end)
        {
            var from = start.Date;
            var to = end.Date < from ? from : end.Date;

            var allowed = id == CurrentUser.Id
                          || (CurrentUser.AllowedPrivileges != null
                              && CurrentUser.AllowedPrivileges.Contains(PrivilegeType.TimesheetCaptureForOtherAccounts))
                          || CurrentUser.IsSystemAdmin
                          || WorkTeamAccessProvider.CanActOn(CurrentUser.Id, id, WorkTeamCapability.Timesheets, from, to);
            if (!allowed)
                return new List<WorkTeamTypePeriod>();

            return WorkTeamAccessProvider.TeamTypePeriodsFor(id, from, to);
        }

        #endregion

        #region Allocations

        /// <summary>
        /// Client / project / subproject tree with this team's allocations selected
        /// </summary>
        [HttpGet]
        public List<UserIdentityProjectModel> WorkTeamAllocationTree(Guid id, bool includeInactive = false)
        {
            return Guarded(() =>
            {
                var selection = new ClientTreeSelection();
                foreach (var client in WorkTeamProvider.ClientAllocations(id))
                    selection.ClientIds.Add(client.ClientId);
                foreach (var project in WorkTeamProvider.ProjectAllocations(id))
                {
                    if (project.SubProjectId.HasValue)
                        selection.SubProjectIds.Add(project.SubProjectId.Value);
                    else
                        selection.ProjectIds.Add(project.ProjectId);
                }

                return ClientTreeBuilder.Build(
                    WorkTeamProvider.ClientCatalog().ToList(),
                    WorkTeamProvider.ProjectCatalog().ToList(),
                    WorkTeamProvider.SubProjectCatalog().ToList(),
                    selection,
                    includeInactive);
            });
        }

        /// <summary>
        /// Replace the team's allocations with the posted selection
        /// </summary>
        [HttpPost]
        public bool SaveWorkTeamAllocations(WorkTeamAllocationSaveModel model)
        {
            CheckModelState();

            return Guarded(() =>
            {
                List<Guid> clientIds;
                List<KeyValuePair<Guid, Guid?>> projectIds;
                ClientTreeBuilder.SplitSelection(model.Selection, out clientIds, out projectIds);

                WorkTeamProvider.SaveAllocations(model.WorkTeamId, clientIds, projectIds);
                return true;
            });
        }

        #endregion
    }
}
