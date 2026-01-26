#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TRiZHub.Models.ScorecardTemplateModels
{
    public class ScorecardTemplatePeriodSearchModel
    {
        public List<Guid> ScorecardTemplateItemIds { get; set; }
        public int[] ReviewYears { get; set; }
    }
}