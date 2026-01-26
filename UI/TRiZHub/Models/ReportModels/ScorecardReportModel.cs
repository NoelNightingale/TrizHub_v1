#region Usings

using System;

#endregion

namespace TRiZHub.Models.ReportModels
{
    public class ScorecardReportModel : GridModel
    {
        //public Guid ScorecardTemplateId { get; set; }
        //public Guid[] ScorecardTemplatePeriodsIds { get; set; }
        //public string ScorecardTemplatePeriods { get; set; }
        //public Guid EmployeeId { get; set; }
        //public Int32 ScoreCardStatus { get; set; }

        public bool SearchAllYears { get; set; }
        public string ReviewYearsString { get; set; }
        public string[] ReviewYears { get; set; }
        public bool SearchAllPeriods { get; set; }
        public Guid[] ReviewPeriods { get; set; }
        public string ReviewPeriodIds { get; set; }
        public int DetailLevel { get; set; }

        public int Submitted { get; set; }
        public int Locked { get; set; }
        public int EmployeeHasScorecard { get; set; }

        public Guid[] Employees { get; set; }
        public string EmployeeIds { get; set; }

        public Guid[] Clients { get; set; }
        public string ClientIds { get; set; }

        public Guid[] LineManagers { get; set; }
        public string LineManagerIds { get; set; }

        public Guid[] Evaluators { get; set; }
        public string EvaluatorIds { get; set; }

        public Guid[] Scorecards { get; set; }
        public string ScorecardIds { get; set; }
    }
}