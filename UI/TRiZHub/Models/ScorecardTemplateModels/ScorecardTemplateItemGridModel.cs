#region Usings

using System;

#endregion

namespace TRiZHub.Models.ScorecardTemplateModels
{
    public class ScorecardTemplateItemGridModel
    {
        public Guid Id { get; set; }

        public string Order { get; set; }

        public string Description { get; set; }

        public decimal Weight { get; set; }
    }
}