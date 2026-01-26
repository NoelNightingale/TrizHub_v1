using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRiZHub.BL.Scripts.ScorecardProcedure
{
    public class ScoreCardSummaryModel
    {
        public Guid? EmployeeAccountId { get; set; }
		public string EmployeeName { get; set; }
        public string EmployeeIsActive { get; set; }
        public Guid? ScorecardID { get; set; }
        public string ScorecardName { get; set; }
        public DateTime? ScorecardVariableStart { get; set; }
        public DateTime? ScorecardVariableEnd { get; set; }
        public int? ScorecardVariableYear { get; set; }
        public int? ReviewYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LineManagerName { get; set; }
        public Guid? EvaluatorId { get; set; }
        public string EvaluatorFirstName { get; set; }
        public string EvaluatorSurname { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Submitted { get; set; }
        public string locked { get; set; }
    }
}
