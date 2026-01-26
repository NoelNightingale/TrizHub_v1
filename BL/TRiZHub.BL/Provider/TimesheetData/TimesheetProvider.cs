#region Usings

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Provider.TimesheetData
{
    public class TimesheetProvider : TRiZHubProvider, ITimesheetProvider
    {
        IList<PrivilegeType> getTokens;
        #region Constructor

        public TimesheetProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            getTokens = new List<PrivilegeType>();
            getTokens.Add(PrivilegeType.TimesheetCapture);
            getTokens.Add(PrivilegeType.TimesheetCaptureForOtherAccounts);
        }

        #endregion

        #region Timesheet Entry

        public IQueryable<TimesheetEntry> TimesheetFilterList(DateTime startDate, DateTime endDate)
        {
            AuthenticateList(getTokens);
            return
                DataContext.TimesheetEntrySet.Where(
                    a => a.DateEntry >= startDate && a.DateEntry <= endDate && a.IsActive);
        }

        public void DeleteTimesheetEntry(Guid id)
        {
            AuthenticateList(getTokens);
            var record = GetTimesheetEntry(id);
            var billingCycleList = DataContext.BillingCycleEntrySet;
            foreach (var a in billingCycleList)
            {
                if (record.DateEntry >= a.StartDate.Date && record.DateEntry <= a.EndDate.Date && a.IsClosed)
                {
                    throw new TimesheetException("Billing Cycle Period for: " + a.StartDate.ToShortDateString() + " - " + a.EndDate.ToShortDateString() +
                                                 " is closed");
                }
            }
            //record.IsActive = false;
            DataContext.TimesheetEntrySet.Remove(record);
            DataContextSaveChanges();
        }

        public TimesheetEntry GetTimesheetEntry(Guid id)
        {
            AuthenticateList(getTokens);
            return DataContext.TimesheetEntrySet
                .Include(a => a.Activity)
                .Include(a => a.Team)
                .Include(a => a.Project)
                .Include(a => a.SubProject)
                .Include(a => a.UserAccount)
                .FirstOrDefault(a => a.Id == id);
        }

        public TimesheetEntry SaveTimesheetEntry(Guid? id, Guid userAccountId, Guid projectId,
            Guid? subProjectId, Guid teamId, Guid activityId,
            string comments, decimal hours, DateTime dateEntry)
        {

            //Users by defualt must have timsheet enrty privilage so dont uathenticate
            AuthenticateList(getTokens);

            if (id == Guid.Empty)
                id = null;

            //set time to 00:00:00 - the UI Calendar contorller is adding some UTC timing offset and cannot figure out how to fix
            dateEntry = dateEntry.AddHours(dateEntry.Hour * -1);


            // for existing entry
            if (id != null)
            {
                // check if existing record has changes if not skip
                var entry = DataContext.TimesheetEntrySet.FirstOrDefault(a => a.UserAccountId == userAccountId &&
                                                                              a.ProjectId == projectId &&
                                                                              a.SubProjectId == subProjectId &&
                                                                              a.TeamId == teamId &&
                                                                              a.ActivityId == activityId &&
                                                                              a.DateEntry == dateEntry &&
                                                                              a.Hours == hours &&
                                                                              a.Comments == comments &&
                                                                              a.Id == id);
                if (entry != null)
                {
                    return entry;
                }

                // check if existing record changes already exists TODO


                var billingCycleList = DataContext.BillingCycleEntrySet;

                //if (billingCycleList == null)
                //{
                //    throw new TimesheetException("No Billing Cycles defined please create billing cycle to continue");
                //}

                //if changes to existing enrty then check if billing cycle is closed
                foreach (var a in billingCycleList)
                {
                    if (dateEntry.Date >= a.StartDate.Date && dateEntry.Date <= a.EndDate.Date && a.IsClosed)
                    {
                        throw new TimesheetException("Billing Cycle Period for: " + a.StartDate.ToShortDateString() + " - " +
                                                     a.EndDate.ToShortDateString() +
                                                     " is closed");
                    }
                }


                var record = DataContext.TimesheetEntrySet.FirstOrDefault(a => a.Id == id);

                record.UserAccountId = userAccountId;
                record.ProjectId = projectId;
                record.SubProjectId = subProjectId;
                record.TeamId = teamId;
                record.ActivityId = activityId;
                record.Comments = comments;
                record.Hours = hours;
                record.DateEntry = dateEntry;

                DataContextSaveChanges();

                return record;
            }

            // for new entry
            else
            {

                // check if billing cycle is closed
                var billingCycleList = DataContext.BillingCycleEntrySet;

                //if (billingCycleList == null)
                //{
                //    throw new TimesheetException("No Billing Cycles defined please create billing cycle to continue");
                //}

                foreach (var a in billingCycleList)
                {
                    if (dateEntry.Date >= a.StartDate.Date && dateEntry.Date <= a.EndDate.Date && a.IsClosed)
                    {
                        throw new TimesheetException("Billing Cycle Period for: " + a.StartDate.ToShortDateString() + " - " + a.EndDate.ToShortDateString() +
                                                     " is closed");
                    }
                }


                var record = DataContext.TimesheetEntrySet.FirstOrDefault(a => a.Id == id);
                if (record == null)
                {
                    record = new TimesheetEntry
                    {
                        CreatedByAccountId = CurrentUser.Id,
                        DateCreated = DateTime.UtcNow,
                        IsActive = true
                    };
                    DataContext.TimesheetEntrySet.Add(record);
                }

                record.UserAccountId = userAccountId;
                record.ProjectId = projectId;
                record.SubProjectId = subProjectId;
                record.TeamId = teamId;
                record.ActivityId = activityId;
                record.Comments = comments;
                record.Hours = hours;
                record.DateEntry = dateEntry;

                DataContextSaveChanges();

                return record;
            }
        }

        #endregion


//        public List<TimesheetReportProcedureModel> CallTImesheetStoreProcedure1(DateTime startDate, DateTime endDate)
//        {
//            return DataContext.ExecuteTimesheetReportProcedure(startDate, endDate);
//        }
    }
}