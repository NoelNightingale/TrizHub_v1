#region Usings

using System;

#endregion

namespace TRiZHub.Models.ScorecardModels
{
    public class ScorecardRecordScoreModel
    {
        public Guid? Id { get; set; }
        public Guid ScorecardTemplateItemScoreId { get; set; }

        public decimal? Score { get; set; }

        public string Definition { get; set; }
    }
}