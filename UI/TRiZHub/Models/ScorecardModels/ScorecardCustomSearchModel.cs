#region Usings

using System;

#endregion

namespace TRiZHub.Models.ScorecardModels
{
    public class ScorecardCustomSearchModel
    {
        public string EmployeeName { get; set; }
        public string ScoreCardName { get; set; }
        public string EvaluatorName { get; set; }
        public bool Locked { get; set; }
        public bool Submitted { get; set; }
        public int Year { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public Guid? PeriodId { get; set; }
        public bool VariablePeriod { get; set; }
    }
}