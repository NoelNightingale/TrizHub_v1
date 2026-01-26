#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ScorecardTemplateData;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.ScorecardData
{
    [Table("Scorecard")]
    public class Scorecard : DbEntity
    {
        public virtual Guid ScorecardTemplateId { get; set; }

        [ForeignKey("ScorecardTemplateId")]
        public virtual ScorecardTemplate ScorecardTemplate { get; set; }

        public virtual Guid ScorecardTemplatePeriodId { get; set; }

        [ForeignKey("ScorecardTemplatePeriodId")]
        public virtual ScorecardTemplatePeriod ScorecardTemplatePeriod { get; set; }

        public virtual Guid EvaluatorId { get; set; }

        [ForeignKey("EvaluatorId")]
        [InverseProperty("EvaluatorsScorecards")]
        public virtual UserAccount Evaluator { get; set; }

        public virtual Guid EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeesScorecards")]
        public virtual UserAccount Employee { get; set; }

        public virtual bool Rated { get; set; }

        public virtual bool Completed { get; set; }

        public virtual Guid CreatedBy { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual string EvaluatorMessage { get; set; }

        public virtual string EmployeeMessage { get; set; }

        public virtual ICollection<ScorecardRecord> ScorecardRecords { get; set; }

        public virtual bool locked { get; set; }

        public virtual DateTime? VariableStart { get; set; }
        public virtual DateTime? VariableEnd { get; set; }
        public virtual int? VariableYear { get; set; }
    }
}