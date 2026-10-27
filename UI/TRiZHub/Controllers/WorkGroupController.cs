#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Provider.WorkTeamData;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models.WorkGroupModels;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class WorkGroupController : TCRControllerBase
    {
        public WorkGroupController()
        {
            WorkGroupProvider = new WorkGroupProvider(Context, CurrentUser);
        }

        public WorkGroupController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            WorkGroupProvider = new WorkGroupProvider(Context, CurrentUser);
        }

        private IWorkGroupProvider WorkGroupProvider { get; }

        private T Guarded<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (WorkGroupException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpGet]
        public WorkGroupTree Tree()
        {
            return Guarded(() => WorkGroupProvider.Tree());
        }

        [HttpPost]
        public Guid SaveGroup(WorkGroupSaveModel model)
        {
            CheckModelState();
            var managers = (model.Managers ?? new List<WorkGroupManagerSaveModel>())
                .Select(m => new WorkGroupManagerSave
                {
                    UserAccountId = m.UserAccountId,
                    CanView = m.CanView
                })
                .ToList();
            return Guarded(() => WorkGroupProvider.SaveGroup(
                model.Id, model.Name, model.Description, model.ClientIds, model.IsActive, managers));
        }

        [HttpPost]
        public bool DeleteGroup(WorkGroupIdModel model)
        {
            CheckModelState();
            return Guarded(() =>
            {
                WorkGroupProvider.DeleteGroup(model.Id);
                return true;
            });
        }

        [HttpPost]
        public Guid SaveTeam(WorkTeamSaveModel model)
        {
            CheckModelState();
            return Guarded(() => WorkGroupProvider.SaveTeam(model.Id, model.WorkGroupId, model.TeamTypeId, model.IsActive));
        }

        [HttpPost]
        public bool DeleteTeam(WorkGroupIdModel model)
        {
            CheckModelState();
            return Guarded(() =>
            {
                WorkGroupProvider.DeleteTeam(model.Id);
                return true;
            });
        }

        [HttpPost]
        public Guid SavePerson(WorkTeamPersonSaveModel model)
        {
            CheckModelState();
            return Guarded(() => WorkGroupProvider.SavePerson(
                model.WorkTeamId,
                model.UserAccountId,
                (WorkTeamRoleType)model.Role,
                model.IsHomeTeam,
                model.CanView));
        }

        [HttpPost]
        public bool DeletePerson(WorkGroupIdModel model)
        {
            CheckModelState();
            return Guarded(() =>
            {
                WorkGroupProvider.DeletePerson(model.Id);
                return true;
            });
        }
    }
}
