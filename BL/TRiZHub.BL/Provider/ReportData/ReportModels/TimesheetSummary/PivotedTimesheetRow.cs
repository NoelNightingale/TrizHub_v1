using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRiZHub.BL.Entities.TimesheetData;

namespace TRiZHub.BL.Provider.ReportData.ReportModels.TimesheetSummary
{
    internal class PivotedTimesheetRow
    {
        public bool Billable { get; set; }
        public virtual string Client { get; set; }
        public virtual string ProjectName { get; set; }
        public virtual string PhaseName { get; set; }
        public virtual string ProjectTypeName { get; set; }
        public virtual string SubProjectTypeName { get; set; }

        public virtual decimal Hours { get; set; }
        public virtual decimal Cost { get; set; }

        public List<decimal> cost = new List<decimal>();
        public List<decimal> hours = new List<decimal>();
    }

    internal class TimesheetSummaryGroup
    {
        public virtual Guid UserAccountId { get; set; }
        public virtual Guid ProjectId { get; set; }
        public virtual string ProjectName { get; set; }
        public virtual Guid ClientId { get; set; }
        public virtual string ClientName { get; set; }
        public virtual bool Billable { get; set; }

        //public virtual DateTime DateEntry { get; set; }
        public virtual List<TimesheetSummaryEntry> Entries { get; set; }
    }

    internal class TimesheetSummaryEntry
    {
        public virtual Guid UserAccountId { get; set; }
        public virtual Guid ProjectId { get; set; }
        public virtual Guid? SubProjectId { get; set; }
        public virtual Guid? SubProjectTypeId { get; set; }
        public virtual Guid? EmployerId { get; set; }
        public virtual Guid? ProjectTypeId { get; set; }
        public virtual string ProjectName { get; set; }
        public virtual string ProjectCode { get; set; }
        public virtual string SubProjectName { get; set; }
        public virtual string SubProjectCode { get; set; }
        public virtual string SubProjectNumber { get; set; }
        public virtual decimal Hours { get; set; }
        public virtual decimal Rate { get; set; }
        public virtual decimal Cost { get; set; }
        public virtual DateTime DateEntry { get; set; }
    }

    internal class HourBreakdown
    {
        public virtual Guid? EmployerId { get; set; }
        public virtual Guid? UserAccountId { get; set; }
        public virtual decimal Hours { get; set; }
        public virtual decimal Cost { get; set; }
        public virtual decimal FlexCost { get; set; }
        public virtual decimal BillableHours { get; set; }
        public virtual decimal BillableCost { get; set; }
        public virtual decimal NonBillableHours { get; set; }
        public virtual decimal NonBillableCost { get; set; }

        // Efficiency

        public virtual decimal FlexHours { get; set; }
        public virtual decimal NonInvoiceableHours { get; set; }
        public virtual decimal AdminHours { get; set; }
        public virtual decimal LeaveVacationHours { get; set; }
        public virtual decimal LeaveSickHours { get; set; }
        public virtual decimal LeaveStudyHours { get; set; }
        public virtual decimal LeaveOtherHours { get; set; }
        public virtual decimal SystemIssueHours { get; set; }
        public virtual decimal TrainingHours { get; set; }
        public virtual decimal NonEligibleHours { get; set; }

        // Only used for summary
        public virtual int UserCount { get; set; }

        public virtual decimal EligibleHours { get; set; }
        public virtual decimal MonthAvailable { get; set; }
        public virtual decimal TotalAdditionalRevenue { get; set; }
    }

    internal class TimesheetDetailEntry
    {
        public virtual Guid UserAccountId { get; set; }
        public virtual string UserName { get; set; }
        public virtual Guid ProjectId { get; set; }
        public virtual Guid? SubProjectId { get; set; }
        public virtual Guid? SubProjectTypeId { get; set; }
        public virtual Guid? ClientId { get; set; }
        public virtual string ClientName { get; set; }
        public virtual Guid? EmployerId { get; set; }
        public virtual string EmployerName { get; set; }
        public virtual Guid? ProjectTypeId { get; set; }
        public virtual bool Billable { get; set; }
        public virtual string ProjectName { get; set; }
        public virtual string ProjectCode { get; set; }
        public virtual string ProjectNumber { get; set; }
        public virtual string SubProjectName { get; set; }
        public virtual string SubProjectCode { get; set; }
        public virtual string SubProjectNumber { get; set; }
        public virtual Guid? TeamId { get; set; }
        public virtual string TeamName { get; set; }
        public virtual Guid? ActivityId { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual decimal Hours { get; set; }
        public virtual decimal Rate { get; set; }
        public virtual decimal Cost { get; set; }
        public virtual string Comments { get; set; }
        public virtual DateTime DateEntry { get; set; }
    }

    internal class TimesheetSummaryRow
    {
        public bool Billable { get; set; }
        public virtual string ClientName { get; set; }
        public virtual List<TimesheetSummaryProject> Projects { get; set; }

        //public virtual string ProjectName { get; set; }
        //public virtual string PhaseName { get; set; }
        //public virtual string ProjectTypeName { get; set; }
        //public virtual string SubProjectTypeName { get; set; }

        //public virtual decimal Hours { get; set; }
        //public virtual decimal Cost { get; set; }

        //public List<decimal> cost = new List<decimal>();
        //public List<decimal> hours = new List<decimal>();

        // Employer Logic
        //public List<Guid> employers = new List<Guid>();
    }

    internal class TimesheetSummaryProject
    {
        public virtual string Name { get; set; }
        public virtual string Level { get; set; }
        public virtual string Type { get; set; }
        public virtual List<TimesheetSummaryProject> Projects { get; set; }
    }

    internal class UserEntry
    {
        public virtual string Name { get; set; }
        public virtual string Level { get; set; }
        public virtual string Type { get; set; }
        public virtual List<TimesheetSummaryProject> Projects { get; set; }
    }
}