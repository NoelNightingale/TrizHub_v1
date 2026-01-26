#region Usings

using System;
using System.Collections.Generic;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.ScorecardTemplateModels
{
    public class ScorecardTemplateItemModel
    {
        public Guid Id { get; set; }
        public Guid ScorecardTemplateId { get; set; }

        public string Order { get; set; }

        public string Description { get; set; }

        public string Definition { get; set; }

        public decimal Weight { get; set; }

        public int ScorecardScoring { get; set; }

        public decimal? Minimum { get; set; }

        public decimal? Maximum { get; set; }

        public string ManualDefinition { get; set; }

        public string ExcellentDefinition { get; set; }

        public string AdequateDefinition { get; set; }

        public string InadequateDefinition { get; set; }

        //public List<ScorecardTemplateItemScoreModel> ScorecardScoreRecords { get; set; }
    }
}