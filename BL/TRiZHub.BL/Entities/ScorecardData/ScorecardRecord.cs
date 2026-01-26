#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ScorecardTemplateData;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.ScorecardData
{
    [Table("ScorecardRecord")]
    public class ScorecardRecord : DbEntity
    {
        public virtual Guid ScorecardId { get; set; }

        [ForeignKey("ScorecardId")]
        public virtual Scorecard Scorecard { get; set; }

        public virtual Guid ScorecardTemplateItemId { get; set; }

        [ForeignKey("ScorecardTemplateItemId")]
        public virtual ScorecardTemplateItem ScorecardTemplateItem { get; set; }

        public virtual ScorecardScoreType? Rating { get; set; }

        public virtual decimal? Value { get; set; }

        public virtual DateTime? LastUpdated { get; set; }

        public virtual bool Completed { get; set; }

        public virtual string EvaluatorHtmlComment { get; set; }

        public virtual string EmployeeHtmlComment { get; set; }
    }
}