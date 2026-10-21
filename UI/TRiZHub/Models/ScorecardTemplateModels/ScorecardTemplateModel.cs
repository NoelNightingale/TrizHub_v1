#region Usings

using System;

#endregion

namespace TRiZHub.Models.ScorecardTemplateModels
{
    public class ScorecardTemplateModel
    {
        public Guid? Id { get; set; }

        public string ScorecardName { get; set; }

        public string ScorecardCode { get; set; }

        public bool IsActive { get; set; }

        public decimal ExcellentWeight { get; set; }

        public decimal AdequateWeight { get; set; }

        public decimal InadequateWeight { get; set; }

        public decimal TotalAvailableWeight { get; set; }

        public int ItemCount { get; set; }

        public decimal TotalWeight { get; set; }

        public int PeriodCount { get; set; }

        public int ScorecardCount { get; set; }
    }
}