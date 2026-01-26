#region Usings

using System;
using System.Collections.Generic;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.ScorecardModels
{
    public class ScorecardRecordScoreDefinitionModel
    {
        public decimal Score { get; set; }
        public ScorecardScoreType Scoretype { get; set; }

        public string ScoretypeString { get { return Scoretype.ToString(); } }

        public string Definition { get; set; }

    }
}