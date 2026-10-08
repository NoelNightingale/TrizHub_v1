#region Usings

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.WorkTeamData;
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

        #region Allocation check

        private readonly Dictionary<string, EffectiveAllocations> allocationCache = new Dictionary<string, EffectiveAllocations>();

        /// <summary>The UI calendar adds a UTC offset in hours; entries are stored with the hours stripped.</summary>
        private static DateTime NormalizeEntryDate(DateTime dateEntry)
        {
            return dateEntry.AddHours(dateEntry.Hour * -1);
        }

        private EffectiveAllocations AllocationsOn(Guid userAccountId, DateTime day)
        {
            var key = userAccountId + "|" + day.ToString("yyyyMMdd");
            EffectiveAllocations allocations;
            if (!allocationCache.TryGetValue(key, out allocations))
            {
                allocations = new EffectiveAllocationProvider(DataContext, CurrentUser).For(userAccountId, day, day);
                allocationCache[key] = allocations;
            }
            return allocations;
        }

        public string ProjectNotAllowedReason(Guid? id, Guid userAccountId, Guid projectId, Guid? subProjectId, DateTime dateEntry)
        {
            var day = NormalizeEntryDate(dateEntry).Date;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                var existing = DataContext.TimesheetEntrySet
                    .Where(a => a.Id == id.Value)
                    .Select(a => new { a.UserAccountId, a.ProjectId, a.SubProjectId, a.DateEntry })
                    .FirstOrDefault();
                if (existing != null
                    && existing.UserAccountId == userAccountId
                    && existing.ProjectId == projectId
                    && existing.SubProjectId == subProjectId
                    && existing.DateEntry.Date == day)
                    return null;
            }

            var project = DataContext.ProjectSet
                .Where(p => p.Id == projectId)
                .Select(p => new { p.ClientId, p.ProjectName, p.ProjectNumber })
                .FirstOrDefault();
            if (project == null)
                return day.ToString("yyyy-MM-dd") + ": the project could not be found.";

            if (subProjectId.HasValue && !DataContext.SubProjectSet.Any(s => s.Id == subProjectId.Value && s.ProjectId == projectId))
                return day.ToString("yyyy-MM-dd") + ": the subproject does not belong to project " + project.ProjectName + ".";

            if (AllocationsOn(userAccountId, day).Allows(project.ClientId, projectId, subProjectId, day))
                return null;

            var projectLabel = string.IsNullOrWhiteSpace(project.ProjectNumber)
                ? project.ProjectName
                : "[" + project.ProjectNumber + "] " + project.ProjectName;
            return day.ToString("yyyy-MM-dd") + ": not allocated to " + projectLabel + " on this date.";
        }

        private void EnsureProjectAllowedOnDate(Guid? id, Guid userAccountId, Guid projectId, Guid? subProjectId, DateTime dateEntry)
        {
            var reason = ProjectNotAllowedReason(id, userAccountId, projectId, subProjectId, dateEntry);
            if (reason != null)
                throw new TimesheetException(reason);
        }

        #endregion

        #region User access

        private static readonly DateTime SqlMinDate = new DateTime(1753, 1, 1);

        private bool HasGlobalAccess(Guid userAccountId)
        {
            return CurrentUser != null
                   && (userAccountId == CurrentUser.Id || UserIsAllowed(PrivilegeType.TimesheetCaptureForOtherAccounts));
        }

        /// <summary>
        /// Null when the current user sees every entry of the user; otherwise the target's team periods the current
        /// user reaches for timesheets in [from, to] (empty when there are none).
        /// </summary>
        private List<WorkTeamTypePeriod> TeamAccessPeriods(Guid userAccountId, DateTime from, DateTime to)
        {
            if (HasGlobalAccess(userAccountId))
                return null;
            if (CurrentUser == null)
                return new List<WorkTeamTypePeriod>();

            var fromDay = from < SqlMinDate ? SqlMinDate : from.Date;
            var toDay = to.Date < fromDay ? fromDay : to.Date;
            return new WorkTeamAccessProvider(DataContext, CurrentUser)
                .ReachablePeriods(CurrentUser.Id, userAccountId, WorkTeamCapability.Timesheets, fromDay, toDay);
        }

        /// <summary>Own entries, TimesheetCaptureForOtherAccounts, or a team lead/manager whose reach covers the entry date.</summary>
        private void EnsureCanAccessUser(Guid userAccountId, DateTime date)
        {
            var periods = TeamAccessPeriods(userAccountId, date, date);
            if (periods != null && periods.Count == 0)
                throw new TimesheetException("You are not allowed to access timesheets for this person on " +
                                             date.ToString("yyyy-MM-dd") + ".");
        }

        #endregion

        #region Timesheet Entry

        public IQueryable<TimesheetEntry> TimesheetFilterList(Guid userAccountId, DateTime startDate, DateTime endDate)
        {
            AuthenticateList(getTokens);
            var entries = DataContext.TimesheetEntrySet.Where(
                a => a.UserAccountId == userAccountId && a.DateEntry >= startDate && a.DateEntry <= endDate && a.IsActive);

            var periods = TeamAccessPeriods(userAccountId, startDate, endDate);
            if (periods == null)
                return entries;
            if (periods.Count == 0)
                throw new TimesheetException("You are not allowed to access timesheets for this person.");

            IQueryable<TimesheetEntry> visible = null;
            foreach (var period in periods)
            {
                var periodStart = period.StartDate.Date;
                var inPeriod = entries.Where(a => a.DateEntry >= periodStart);
                if (period.EndDate.HasValue)
                {
                    var afterEnd = period.EndDate.Value.Date.AddDays(1);
                    inPeriod = inPeriod.Where(a => a.DateEntry < afterEnd);
                }
                visible = visible == null ? inPeriod : visible.Union(inPeriod);
            }
            return visible;
        }

        public void DeleteTimesheetEntry(Guid id)
        {
            AuthenticateList(getTokens);
            var record = GetTimesheetEntry(id);
            if (record == null)
                throw new TimesheetException("The timesheet entry could not be found.");
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
            var record = DataContext.TimesheetEntrySet
                .Include(a => a.Activity)
                .Include(a => a.Team)
                .Include(a => a.Project)
                .Include(a => a.SubProject)
                .Include(a => a.UserAccount)
                .FirstOrDefault(a => a.Id == id);
            if (record != null)
                EnsureCanAccessUser(record.UserAccountId, record.DateEntry);
            return record;
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
            var originalDateEntry = dateEntry;
            dateEntry = NormalizeEntryDate(dateEntry);

            EnsureCanAccessUser(userAccountId, dateEntry.Date);
            if (id != null)
            {
                var stored = DataContext.TimesheetEntrySet
                    .Where(a => a.Id == id)
                    .Select(a => new { a.UserAccountId, a.DateEntry })
                    .FirstOrDefault();
                if (stored == null)
                    throw new TimesheetException("The timesheet entry could not be found.");
                EnsureCanAccessUser(stored.UserAccountId, stored.DateEntry.Date);
            }

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


                EnsureProjectAllowedOnDate(id, userAccountId, projectId, subProjectId, originalDateEntry);

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


                EnsureProjectAllowedOnDate(null, userAccountId, projectId, subProjectId, originalDateEntry);

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