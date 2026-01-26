#region Usings

using System;

#endregion

namespace TRiZHub.Models.ScorecardTemplateModels
{
    public class ScorecardTemplateGridModel
    {
        public Guid Id { get; set; }

        public string ScorecardName { get; set; }

        public string ScorecardCode { get; set; }

        public bool IsActive { get; set; }
    }
}