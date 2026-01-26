#region Usings

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;

#endregion

namespace TRiZHub.BL.Entities.ScorecardTemplateData
{
    [Table("ScorecardTemplatePeriod")]
    public class ScorecardTemplatePeriod : DbEntity
    {
        public virtual Guid ScorecardTemplateId { get; set; }

        [ForeignKey("ScorecardTemplateId")]
        public virtual ScorecardTemplate ScorecardTemplate { get; set; }

        [Required]
        public virtual DateTime StartDate { get; set; }

        [Required]
        public virtual DateTime EndDate { get; set; }

        [Required]
        public virtual string Description { get; set; }

        [Required]
        public virtual int ReviewYear { get; set; }

        [Required]
        public virtual bool IsVariable { get; set; }

        [Required]
        [DefaultValue(1)]
        public virtual int ReportSortOrder { get; set; }
    }
}