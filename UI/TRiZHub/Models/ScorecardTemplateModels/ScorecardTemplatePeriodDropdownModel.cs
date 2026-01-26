#region Usings

using System;

#endregion

namespace TRiZHub.Models.ScorecardTemplateModels
{
    public class ScorecardTemplatePeriodDropdownModel
    {
        public Guid Id { get; set; }

        public string Description
        {
            get
            {
                if (IsVariable)
                {
                    return "(Variable) " + Name;
                }
                else {
                    return string.Format("({0} - {1}) {2}", StartDate.ToString(@"yyyy\/MM\/dd"), EndDate.ToString(@"yyyy\/MM\/dd"), Name);
                }
            }
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Name { get; set; }

        public string ScorecardName { get; set; }
        public bool IsVariable { get; set; }
        public int ReportSortOrder { get; set; }
    }
}