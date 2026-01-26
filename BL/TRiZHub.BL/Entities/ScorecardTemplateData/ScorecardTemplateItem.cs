#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.ScorecardTemplateData
{
    [Table("ScorecardTemplateItem")]
    public class ScorecardTemplateItem : DbEntity
    {
        public virtual Guid ScorecardTemplateId { get; set; }

        [ForeignKey("ScorecardTemplateId")]
        public virtual ScorecardTemplate ScorecardTemplate { get; set; }

        public virtual string Order { get; set; }

        [Required]
        public virtual string Description { get; set; }

        public virtual string Definition { get; set; }

        [Required]
        public virtual decimal Weight { get; set; }

        public virtual decimal? Minimum { get; set; }

        public virtual decimal? Maximum { get; set; }

        public virtual int ScorecardScoring { get; set; }

        public virtual string ManualDefinition { get; set; }

        public virtual string ExcellentDefinition { get; set; }

        public virtual string AdequateDefinition { get; set; }

        public virtual string InadequateDefinition { get; set; }
    }
}