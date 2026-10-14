#region Usings

using System;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.BillingCycleData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using System.Collections.Generic;
#endregion

namespace TRiZHub.BL.Provider.BillingCycleData
{
    public class BillingCycleProvider : TRiZHubProvider, IBillingCycleProvider
    {

        IList<PrivilegeType> getTokens;

        #region Constructor

        public BillingCycleProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            getTokens = new List<PrivilegeType>();
            getTokens.Add(PrivilegeType.BillingCycleMaintenance);
            getTokens.Add(PrivilegeType.CustomerReportAccess);
            getTokens.Add(PrivilegeType.ReportGenerationTimesheet);
            getTokens.Add(PrivilegeType.TimesheetCapture);
            getTokens.Add(PrivilegeType.TimesheetCaptureForOtherAccounts);
        }

        #endregion

        #region Billing Cycle Entry 



        public IQueryable<BillingCycleEntry> BillingCycleFillterList()
        {
            AuthenticateList(getTokens);
            return DataContext.BillingCycleEntrySet;
        }

        public void DeleteBillingCycleEntry(Guid id)
        {
            Authenticate(PrivilegeType.BillingCycleMaintenance);
            var record = GetBillingCycleEntry(id);

            if (record != null)
            {
                record.IsActive = false;
                DataContextSaveChanges();
            }
        }

        public BillingCycleEntry GetBillingCycleEntry(Guid id)
        {
            AuthenticateList(getTokens);
            return DataContext.BillingCycleEntrySet.Include(a => a.CreatedByAccount).FirstOrDefault(a => a.Id == id && a.IsActive == true);
        }

        public BillingCycleEntry SaveBillingCycleEntry(Guid? id, short cycle, short year, DateTime startDate,
            DateTime endDate, short weekdays, short publicHolidays, short workDays, bool isClosed)
        {
            Authenticate(PrivilegeType.BillingCycleMaintenance);

            if (id == Guid.Empty)
                id = null;

            // for existing entry 
            if (id != null)
            {
                //check if existing entry has changes if not skip
                var entry = DataContext.BillingCycleEntrySet.FirstOrDefault(a => a.StartDate == startDate &&
                                                                                 a.EndDate == endDate &&
                                                                                 a.Id == id);

                if (startDate.Date > endDate.Date)
                {
                    throw new BillingCyleException("Selected End Date is before selected Start Date!");
                }

                //check that billing cycles dont overlapp 
                var billingCycleList = DataContext.BillingCycleEntrySet;

                if (billingCycleList != null)
                {
                    foreach (var a in billingCycleList)
                    {
                        if (a.Id == id || a.IsActive == false)
                        {
                            continue;
                        }
                        else
                        {
                            if (startDate.Date >= a.StartDate.Date && startDate.Date <= a.EndDate.Date ||
                                endDate.Date >= a.StartDate.Date && endDate.Date <= a.EndDate.Date ||
                                a.StartDate.Date >= startDate.Date && a.StartDate.Date <= endDate.Date ||
                                a.EndDate.Date >= startDate.Date && a.EndDate.Date <= startDate.Date)
                            {
                                throw new BillingCyleException(
                               "Billing Cycle period will overlap with another period!");
                            }
                        }
                    }
                }

                var record = DataContext.BillingCycleEntrySet.FirstOrDefault(a => a.Id == id);
                if (record == null)
                {
                    record = new BillingCycleEntry
                    {
                        CreatedByAccountId = CurrentUser.Id,
                        DateCreated = DateTime.UtcNow,
                        IsActive = true
                    };
                    DataContext.BillingCycleEntrySet.Add(record);
                }

                record.Cycle = cycle;
                record.Year = year;
                record.StartDate = startDate;
                record.EndDate = endDate;
                record.Weekdays = weekdays;
                record.PublicHolidays = publicHolidays;
                record.WorkDays = workDays;
                record.IsClosed = isClosed;

                DataContextSaveChanges();

                return record;
            }
            // for new entry 
            else
            {
                if(startDate.Date > endDate.Date)
                {
                    throw new BillingCyleException("Selected End Date is bofore selected Start Date!");
                }

                //check that billing cycles dont overlapp 
                var billingCycleList = DataContext.BillingCycleEntrySet;

                if (billingCycleList != null)
                {
                    foreach (var a in billingCycleList)
                    {
                        if (a.Id == id)
                        {
                            continue;
                        }
                        else
                        {

                            if (startDate.Date >= a.StartDate.Date && startDate.Date <= a.EndDate.Date ||
                                endDate.Date >= a.StartDate.Date && endDate.Date <= a.EndDate.Date ||
                                a.StartDate.Date >= startDate.Date && a.StartDate.Date <= endDate.Date ||
                                a.EndDate.Date >= startDate.Date && a.EndDate.Date <= startDate.Date)
                            {
                                throw new BillingCyleException("Billing Cycle period will overlap with another period!");
                            }
                        }
                        }
                }

                var record = DataContext.BillingCycleEntrySet.FirstOrDefault(a => a.Id == id);
                if (record == null)
                {
                    record = new BillingCycleEntry
                    {
                        CreatedByAccountId = CurrentUser.Id,
                        DateCreated = DateTime.UtcNow,
                        IsActive = true
                    };
                    DataContext.BillingCycleEntrySet.Add(record);
                }

                record.Cycle = cycle;
                record.Year = year;
                record.StartDate = startDate;
                record.EndDate = endDate;
                record.Weekdays = weekdays;
                record.PublicHolidays = publicHolidays;
                record.WorkDays = workDays;
                record.IsClosed = isClosed;

                DataContextSaveChanges();

                return record;
            }
        }

        public BillingCycleEntry SetClosed(Guid id, bool isClosed)
        {
            Authenticate(PrivilegeType.BillingCycleMaintenance);
            var record = DataContext.BillingCycleEntrySet.FirstOrDefault(a => a.Id == id && a.IsActive);
            if (record == null)
                throw new BillingCyleException("The billing cycle could not be found.");

            if (record.IsClosed != isClosed)
            {
                record.IsClosed = isClosed;
                DataContextSaveChanges();
            }
            return record;
        }

        #endregion

        #region Exemptions

        public BillingCycleEntry ClosingCycleFor(Guid userAccountId, DateTime day)
        {
            AuthenticateList(getTokens);
            var date = day.Date;
            var nextDay = date.AddDays(1);
            return DataContext.BillingCycleEntrySet
                .Where(a => a.IsActive && a.IsClosed
                            && a.StartDate < nextDay && a.EndDate >= date
                            && !a.Exemptions.Any(e => e.UserAccountId == userAccountId))
                .OrderBy(a => a.StartDate)
                .FirstOrDefault();
        }

        public bool IsUserExempt(Guid billingCycleId, Guid userAccountId)
        {
            AuthenticateList(getTokens);
            return DataContext.BillingCycleExemptionSet
                .Any(e => e.BillingCycleEntryId == billingCycleId && e.UserAccountId == userAccountId);
        }

        public IQueryable<BillingCycleExemption> ExemptionList()
        {
            AuthenticateList(getTokens);
            return DataContext.BillingCycleExemptionSet;
        }

        public void SaveExemptions(Guid billingCycleId, IList<Guid> userAccountIds)
        {
            Authenticate(PrivilegeType.BillingCycleMaintenance);
            if (!DataContext.BillingCycleEntrySet.Any(a => a.Id == billingCycleId && a.IsActive))
                throw new BillingCyleException("The billing cycle could not be found.");

            var wanted = new HashSet<Guid>(userAccountIds ?? new List<Guid>());
            var existing = DataContext.BillingCycleExemptionSet
                .Where(e => e.BillingCycleEntryId == billingCycleId)
                .ToList();

            foreach (var exemption in existing.Where(e => !wanted.Contains(e.UserAccountId)))
                DataContext.BillingCycleExemptionSet.Remove(exemption);

            var existingUserIds = new HashSet<Guid>(existing.Select(e => e.UserAccountId));
            foreach (var userAccountId in wanted.Where(u => !existingUserIds.Contains(u)))
            {
                DataContext.BillingCycleExemptionSet.Add(new BillingCycleExemption
                {
                    BillingCycleEntryId = billingCycleId,
                    UserAccountId = userAccountId,
                    CreatedByAccountId = CurrentUser.Id,
                    DateCreated = DateTime.UtcNow
                });
            }

            DataContextSaveChanges();
        }

        public IQueryable<UserAccount> ExemptionCandidates()
        {
            Authenticate(PrivilegeType.BillingCycleMaintenance);
            return DataContext.UserAccountSet.Where(a => a.Active);
        }

        #endregion
    }
}