#region Usings

using System;

#endregion

namespace TRiZHub.Models.ScorecardTemplateModels
{
    public class ScorecardTemplatePeriodGridModel
    {
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public int ReviewYear { get; set; }

        public bool IsVariable { get; set; }

        public int ReportSortOrder { get; set; }
    }
}