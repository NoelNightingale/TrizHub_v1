#region Usings

using System;
using System.Collections.Generic;
#endregion

namespace TRiZHub.Models.ScorecardModels
{
    public class ScorecardModel
    {
        public Guid? ScorecardId { get; set; }
        public Guid ScorecardTemplateId { get; set; }
        public Guid ScorecardTemplatePeriodId { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid EmployeeId { get; set; }

        public string ScorecardName { get; set; }

        public string ScorecardCode { get; set; }

        public string EvaluatorName { get; set; }

        public string EmployeeName { get; set; }

        public string ScorecardPeriodName { get; set; }

        public DateTime DateCreated { get; set; }

        public Guid createdBy { get; set; }

        public bool Completed { get; set; }

        public bool rated { get; set; }

        public string EvaluatorMessage { get; set; }

        public string EmployeeMessage { get; set; }

        public bool locked { get; set; }

        //        public System.Collections.List<ScorecardRecordModel> ScorecardRecords { get; set; }

        public DateTime? VariableStart { get; set; }
        public DateTime? VariableEnd { get; set; }
        public int? VariableYear { get; set; }
    }
}