#region Usings

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.TimesheetData
{
    public class TimesheetTemplateProvider : TRiZHubProvider, ITimesheetTemplateProvider
    {
        private readonly IList<PrivilegeType> getTokens;

        public TimesheetTemplateProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            getTokens = new List<PrivilegeType>
            {
                PrivilegeType.TimesheetCapture,
                PrivilegeType.TimesheetCaptureForOtherAccounts
            };
        }

        public List<TimesheetTemplate> ListForUser(Guid userAccountId)
        {
            AuthenticateList(getTokens);
            EnsureCanAccessUser(userAccountId);

            return DataContext.TimesheetTemplateSet
                .Include(t => t.Items)
                .Where(t => t.UserAccountId == userAccountId && t.IsActive)
                .OrderByDescending(t => t.DateModified)
                .ToList();
        }

        public TimesheetTemplate GetWithItems(Guid id)
        {
            AuthenticateList(getTokens);
            var record = DataContext.TimesheetTemplateSet
                .Include(t => t.Items)
                .FirstOrDefault(t => t.Id == id && t.IsActive);
            if (record == null)
                throw new TimesheetTemplateException("Template not found.");
            EnsureCanAccessUser(record.UserAccountId);
            return record;
        }

        public TimesheetTemplate SaveFromClipboard(Guid userAccountId, string name, string templateType,
            IList<TimesheetTemplateItem> items)
        {
            AuthenticateList(getTokens);
            EnsureCanAccessUser(userAccountId);

            if (string.IsNullOrWhiteSpace(name))
                throw new TimesheetTemplateException("Template name is required.");
            if (templateType != "day" && templateType != "week")
                throw new TimesheetTemplateException("Template type must be day or week.");
            if (items == null || items.Count == 0)
                throw new TimesheetTemplateException("Template must have at least one row.");

            var now = DateTime.Now;
            var template = new TimesheetTemplate
            {
                UserAccountId = userAccountId,
                Name = name.Trim(),
                TemplateType = templateType,
                IsActive = true,
                DateCreated = now,
                DateModified = now,
                Items = new List<TimesheetTemplateItem>()
            };

            for (var i = 0; i < items.Count; i++)
            {
                var src = items[i];
                if (src.ProjectId == Guid.Empty || src.TeamId == Guid.Empty || src.ActivityId == Guid.Empty)
                    throw new TimesheetTemplateException("Each template row needs project, team and activity.");

                template.Items.Add(new TimesheetTemplateItem
                {
                    DayOffset = src.DayOffset,
                    SortOrder = i,
                    ProjectId = src.ProjectId,
                    SubProjectId = src.SubProjectId,
                    TeamId = src.TeamId,
                    ActivityId = src.ActivityId,
                    Hours = src.Hours,
                    Comments = src.Comments ?? string.Empty,
                    ProjectDescription = src.ProjectDescription,
                    ClientEntityName = src.ClientEntityName,
                    Billable = src.Billable
                });
            }

            DataContext.TimesheetTemplateSet.Add(template);
            DataContextSaveChanges();
            return GetWithItems(template.Id);
        }

        public TimesheetTemplate Rename(Guid id, string name)
        {
            AuthenticateList(getTokens);
            if (string.IsNullOrWhiteSpace(name))
                throw new TimesheetTemplateException("Template name is required.");

            var record = DataContext.TimesheetTemplateSet.FirstOrDefault(t => t.Id == id && t.IsActive);
            if (record == null)
                throw new TimesheetTemplateException("Template not found.");
            EnsureCanAccessUser(record.UserAccountId);

            record.Name = name.Trim();
            record.DateModified = DateTime.Now;
            DataContextSaveChanges();
            return GetWithItems(record.Id);
        }

        public void Delete(Guid id)
        {
            AuthenticateList(getTokens);
            var record = DataContext.TimesheetTemplateSet
                .Include(t => t.Items)
                .FirstOrDefault(t => t.Id == id);
            if (record == null)
                return;
            EnsureCanAccessUser(record.UserAccountId);

            if (record.Items != null)
            {
                foreach (var item in record.Items.ToList())
                    DataContext.TimesheetTemplateItemSet.Remove(item);
            }
            DataContext.TimesheetTemplateSet.Remove(record);
            DataContextSaveChanges();
        }

        private void EnsureCanAccessUser(Guid userAccountId)
        {
            if (CurrentUser == null)
                throw new TimesheetTemplateException("Not authenticated.");

            if (CurrentUser.Id == userAccountId)
                return;

            if (!UserIsAllowed(PrivilegeType.TimesheetCaptureForOtherAccounts))
                throw new TimesheetTemplateException("You cannot manage templates for other users.");
        }
    }
}
