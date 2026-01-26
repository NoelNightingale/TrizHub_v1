#region Usings

using System;

#endregion

namespace TRiZHub.Models.ScorecardModels
{
    public class ScorecardGridModel
    {
        public Guid ScorecardId { get; set; }
        public Guid ScorecardPeriodId { get; set; }

        public string ScorecardName { get; set; }

        public string ScorecardCode { get; set; }

        public string EvaluatorName { get; set; }

        public string EmployeeName { get; set; }

        public string ScorecardPeriod { get; set; }

        public DateTime DateCreated { get; set; }

        public bool Completed { get; set; }

        public bool locked { get; set; }
    }
}