#region Usings

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ScorecardData;

#endregion

namespace TRiZHub.BL.Entities.ScorecardTemplateData
{
    [Table("ScorecardTemplate")]
    public class ScorecardTemplate : DbEntity
    {
        [Required]
        [MaxLength(500)]
        public virtual string ScorecardName { get; set; }

        [MaxLength(500)]
        public virtual string ScorecardCode { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual decimal ExcellentWeight { get; set; }

        public virtual decimal AdequateWeight { get; set; }

        public virtual decimal InadequateWeight { get; set; }

        #region Navigation

        public virtual ICollection<ScorecardTemplateItem> ScorecardTemplateItems { get; set; }
        public virtual ICollection<Scorecard> Scorecards { get; set; }
        public virtual ICollection<ScorecardTemplatePeriod> ScorecardTemplatePeriods { get; set; }

        #endregion
    }
}