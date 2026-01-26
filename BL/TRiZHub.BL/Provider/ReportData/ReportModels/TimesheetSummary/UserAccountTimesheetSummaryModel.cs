#region Usings

using System;
using TRiZHub.BL.Provider.ReportData.ReportAttributes;

#endregion

namespace TRiZHub.BL.Provider.ReportData.ReportModels.TimesheetSummary
{
    public class UserAccountTimesheetSummaryModel
    {
        [ReportHiddenColumn(1)]
        public virtual Guid Id { get; set; }

        public virtual string Firstname { get; set; }

        public virtual string Surname { get; set; }

        public virtual string Fullname { get; set; }
    }
}