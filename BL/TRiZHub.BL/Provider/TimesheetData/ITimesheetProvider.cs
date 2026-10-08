#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Scripts.TimesheetReportProcedure;

#endregion

namespace TRiZHub.BL.Provider.TimesheetData
{
    public interface ITimesheetProvider : ITRiZHubProvider
    {
        #region Timesheet Entry

        TimesheetEntry SaveTimesheetEntry(Guid? id, Guid userAccountId, Guid projectId,
            Guid? subProjectId, Guid teamId, Guid activityId,
            string comments, decimal hours, DateTime dateEntry);

        /// <summary>
        /// Null when the user may log the project line on the entry's date, otherwise the reason. Existing entries are
        /// only checked when the user, project, subproject or date changes.
        /// </summary>
        string ProjectNotAllowedReason(Guid? id, Guid userAccountId, Guid projectId, Guid? subProjectId, DateTime dateEntry);

        void DeleteTimesheetEntry(Guid id);

        TimesheetEntry GetTimesheetEntry(Guid id);

        /// <summary>One user's entries; team leads and managers only see dates their team reach covers.</summary>
        IQueryable<TimesheetEntry> TimesheetFilterList(Guid userAccountId, DateTime startDate, DateTime endDate);

        #endregion


        #region Noel Timesheet StoreProc

        //Gan net vir nou void doen :P
//        List<TimesheetReportProcedureModel> CallTImesheetStoreProcedure(DateTime startDate, DateTime endDate);

        #endregion
    }
}