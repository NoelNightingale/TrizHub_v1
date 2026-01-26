#region Usings

using System;
using TRiZHub.BL.Provider.ReportData.ReportAttributes;

#endregion

namespace TRiZHub.BL.Provider.ReportData.ReportModels.TimesheetSummary
{
    public class ActivityTimesheetSummaryModel
    {
        [ReportHiddenColumn(1)]
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }
    }
}