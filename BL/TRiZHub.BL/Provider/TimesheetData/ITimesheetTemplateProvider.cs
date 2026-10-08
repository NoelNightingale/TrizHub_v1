#region Usings

using System;
using System.Collections.Generic;
using TRiZHub.BL.Entities.TimesheetData;

#endregion

namespace TRiZHub.BL.Provider.TimesheetData
{
    public interface ITimesheetTemplateProvider : ITRiZHubProvider
    {
        List<TimesheetTemplate> ListForUser(Guid userAccountId);

        TimesheetTemplate GetWithItems(Guid id);

        TimesheetTemplate SaveFromClipboard(Guid userAccountId, string name, string templateType,
            IList<TimesheetTemplateItem> items);

        TimesheetTemplate Rename(Guid id, string name);

        void Delete(Guid id);
    }
}
