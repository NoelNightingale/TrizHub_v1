#region Usings

using System;
using System.Collections.Generic;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.ScorecardModels
{
    public class ScorecardRecordModel
    {

        public Guid? ScorecardRecordId { get; set; }
        public Guid ScorecardTemplateItemId { get; set; }

        public string Order { get; set; }

        public string Description { get; set; }

        public string Definition { get; set; }

        public decimal Weight { get; set; }

        public ScorecardScoreType? ScoreType { get; set; }

        public int ScorecardScoring { get; set; }

        public decimal? Minimum { get; set; }

        public decimal? Maximum { get; set; }

        public string ManualDefinition { get; set; }

        public string EDefinition { get; set; }

        public string ADefinition { get; set; }

        public string IDefinition { get; set; }

        public string EvaluatorHtmlComment { get; set; }

        public string EmployeeHtmlComment { get; set; }

        public decimal? Value { get; set; }

        public List<ScorecardRecordScoreDefinitionModel> ScoretypeDefinitions { get; set; }
    }
}