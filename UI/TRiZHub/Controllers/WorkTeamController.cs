#region Usings

using System;
using System.Collections.Generic;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.WorkTeamData;
using TRiZHub.Controllers.Filters;

#endregion

namespace TRiZHub.Controllers
{
    /// <summary>
    /// Kept for the timesheet team-type preselect. Reach is empty until that specification is rewritten
    /// against work groups.
    /// </summary>
    [Authorize]
    [NoCache]
    public class WorkTeamController : TCRControllerBase
    {
        public WorkTeamController()
        {
            WorkTeamAccessProvider = new WorkTeamAccessProvider(Context, CurrentUser);
        }

        public WorkTeamController(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            WorkTeamAccessProvider = new WorkTeamAccessProvider(Context, CurrentUser);
        }

        private IWorkTeamAccessProvider WorkTeamAccessProvider { get; }

        [HttpGet]
        public List<WorkTeamTypePeriod> TeamTypesForUser(Guid id, DateTime start, DateTime end)
        {
            var from = start.Date;
            var to = end.Date < from ? from : end.Date;

            var allowed = CurrentUser != null
                          && (id == CurrentUser.Id
                              || CurrentUser.IsSystemAdmin
                              || (CurrentUser.AllowedPrivileges != null
                                  && CurrentUser.AllowedPrivileges.Contains(PrivilegeType.TimesheetCaptureForOtherAccounts))
                              || WorkTeamAccessProvider.CanActOn(CurrentUser.Id, id, WorkTeamCapability.Timesheets, from, to));
            if (!allowed)
                return new List<WorkTeamTypePeriod>();

            return WorkTeamAccessProvider.TeamTypePeriodsFor(id, from, to);
        }
    }
}
