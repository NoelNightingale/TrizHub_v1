#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Provider.TimesheetData;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models.TimesheetTemplateModels;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class TimesheetTemplateController : TCRControllerBase
    {
        public TimesheetTemplateController()
        {
            AppSettings = new AppSettings(Context);
            TimesheetTemplateProvider = new TimesheetTemplateProvider(Context, CurrentUser);
        }

        public TimesheetTemplateController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            TimesheetTemplateProvider = new TimesheetTemplateProvider(Context, CurrentUser);
        }

        private IAppSettings AppSettings { get; }
        private ITimesheetTemplateProvider TimesheetTemplateProvider { get; }

        [HttpPost]
        public List<TimesheetTemplateModel> List(TimesheetTemplateListRequest model)
        {
            try
            {
                CheckModelState();
                return TimesheetTemplateProvider.ListForUser(model.UserAccountId)
                    .Select(MapToModel)
                    .ToList();
            }
            catch (TimesheetTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpPost]
        public TimesheetTemplateModel Save(TimesheetTemplateSaveModel model)
        {
            try
            {
                CheckModelState();
                var items = (model.Rows ?? new List<TimesheetTemplateItemModel>())
                    .Select(r => new TimesheetTemplateItem
                    {
                        DayOffset = r.DayOffset,
                        ProjectId = r.ProjectGridId,
                        ProjectDescription = r.ProjectDescription,
                        ClientEntityName = r.ClientEntityName,
                        Billable = r.Billable,
                        SubProjectId = r.SubProjectId,
                        TeamId = r.TeamId,
                        ActivityId = r.ActivityId,
                        Hours = r.Hours,
                        Comments = r.Comments ?? string.Empty
                    })
                    .ToList();

                var saved = TimesheetTemplateProvider.SaveFromClipboard(
                    model.UserAccountId,
                    model.Label,
                    model.Type,
                    items);
                return MapToModel(saved);
            }
            catch (TimesheetTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpPost]
        public TimesheetTemplateModel Rename(TimesheetTemplateRenameModel model)
        {
            try
            {
                CheckModelState();
                var saved = TimesheetTemplateProvider.Rename(model.Id, model.Label);
                return MapToModel(saved);
            }
            catch (TimesheetTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpPost]
        public void Delete(TimesheetTemplateRenameModel model)
        {
            try
            {
                CheckModelState();
                TimesheetTemplateProvider.Delete(model.Id);
            }
            catch (TimesheetTemplateException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        private static TimesheetTemplateModel MapToModel(TimesheetTemplate t)
        {
            var rows = (t.Items ?? Enumerable.Empty<TimesheetTemplateItem>())
                .OrderBy(i => i.SortOrder)
                .Select(i => new TimesheetTemplateItemModel
                {
                    DayOffset = i.DayOffset,
                    ProjectGridId = i.ProjectId,
                    ProjectDescription = i.ProjectDescription,
                    ClientEntityName = i.ClientEntityName,
                    Billable = i.Billable,
                    SubProjectId = i.SubProjectId,
                    TeamId = i.TeamId,
                    ActivityId = i.ActivityId,
                    Hours = i.Hours,
                    Comments = i.Comments
                })
                .ToList();

            return new TimesheetTemplateModel
            {
                Id = t.Id,
                UserAccountId = t.UserAccountId,
                Label = t.Name,
                Type = t.TemplateType,
                CopiedAt = t.DateModified.ToString("o"),
                RowCount = rows.Count,
                Rows = rows
            };
        }
    }
}
