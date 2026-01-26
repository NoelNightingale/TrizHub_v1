#region Usings

using System.Collections.Generic;

#endregion

namespace TRiZHub.Models.ScorecardModels
{
    public class ScorecardCollectionModel
    {
        public ScorecardModel ScorecardModel { get; set; }

        public List<ScorecardRecordModel> ScorecardRecordModels { get; set; }
    }
}