#region Usings

using System;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.ScorecardTemplateModels
{
    public class ScorecardTemplateItemScoreModel
    {
        public Guid? Id { get; set; }
        public Guid? ScorecardTemplateItemId { get; set; }

        public ScorecardScoreType ScorecardType { get; set; }

        public string ScorecardTypeString
        {
            get { return ScorecardType.ToString(); }
        }

        public decimal? Score { get; set; }

        public string Definition { get; set; }

        
    }
}