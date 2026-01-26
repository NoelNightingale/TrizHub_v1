#region Usings

using System;
using TRiZHub.BL.Provider.ReportData.ReportAttributes;

#endregion

namespace TRiZHub.BL.Provider.ReportData.ReportModels.TimesheetSummary
{
    public class ProjectTimesheetSummaryModel
    {
        [ReportHiddenColumn(1)]
        public virtual Guid Id { get; set; }

        [ReportHiddenColumn(2)]
        public virtual Guid? ProjectLeadId { get; set; }

        public virtual string ProjectName { get; set; }

        public virtual string ClientName { get; set; }

        public virtual bool Billable { get; set; }
    }
}