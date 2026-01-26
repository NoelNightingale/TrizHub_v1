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

        void DeleteTimesheetEntry(Guid id);

        TimesheetEntry GetTimesheetEntry(Guid id);

        IQueryable<TimesheetEntry> TimesheetFilterList(DateTime startDate, DateTime endDate);

        #endregion


        #region Noel Timesheet StoreProc

        //Gan net vir nou void doen :P
//        List<TimesheetReportProcedureModel> CallTImesheetStoreProcedure(DateTime startDate, DateTime endDate);

        #endregion
    }
}