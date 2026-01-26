using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRiZHub.BL.Scripts.TimesheetReportProcedure
{
    public class TimesheetReportProcedureModel
    {
        public bool Billable { get; set; }
        public string Client { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string SubProjectId { get; set; }
        public string SubProjectType { get; set; }
        public string PhaseName { get; set; }
        public string UserAccountId { get; set; }
        public string Person { get; set; }
        public string CurrentClientName { get; set; }
        public decimal Hours { get; set; }
        public decimal Cost { get; set; }
        public Guid ProjectIdGuid { get { return new Guid(ProjectId); } }
        public Guid UserAccountIdGuid { get { return new Guid(UserAccountId); } }
    }

    public class TimesheetReportDetailModel
    {
        public Guid UserAccountId { get; set; }
        public string Contact { get; set; }
        public DateTime DateEntry { get; set; }
        public string Client { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string ProjectBillableType { get; set; }
        public bool Billable { get; set; }
        public string SubProjectNumber { get; set; }
        public string SubProjectName { get; set; }
        public string SubProjectBillableType { get; set; }
        public Guid EmployerId { get; set; }
        public string EmployerName { get; set; }
        public string TeamName { get; set; }
        public string ActivityName { get; set; }
        public string Comments { get; set; }
        public decimal Hours { get; set; }
        public decimal? Rate { get; set; }
    }

    public class BillableHoursReportModel
    {
        public string Phase { get; set; }
        public string Contact { get; set; }
        public string TeamName { get; set; }
        public string EmployerId { get; set; }
        public string Employer { get; set; }
        public string ActivityName { get; set; }
        public decimal Hours { get; set; }
        public decimal? Rate { get; set; }
    }
}