#region Usings

using System;
using TRiZHub.BL.Provider.ReportData.ReportAttributes;

#endregion

namespace TRiZHub.BL.Provider.ReportData.ReportModels.TimesheetSummary
{
    public class TimesheetSummaryEntryModel
    {
        [ReportHiddenColumn(1)]
        public virtual Guid Id { get; set; }

        [ReportHiddenColumn(2)]
        public virtual Guid UserAccountId { get; set; }

        [ReportHiddenColumn(3)]
        public virtual Guid ProjectId { get; set; }

        [ReportHiddenColumn(4)]
        public virtual Guid? SubProjectId { get; set; }

        [ReportHiddenColumn(5)]
        public virtual Guid TeamId { get; set; }

        [ReportHiddenColumn(6)]
        public virtual Guid ActivityId { get; set; }

        public virtual string Client { get; set; }

        public virtual string Comments { get; set; }

        public virtual decimal Hours { get; set; }

        public DateTime DateEntry { get; set; }

        public decimal Cost { get; set; }

        public string DateEntryFormatted
        {
            get { return DateEntry.ToLongDateString(); }
        }
    }
}