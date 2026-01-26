#region Usings

using System;

#endregion

namespace TRiZHub.Models.ScorecardTemplateModels
{
    public class ScorecardTemplateDropdownModel
    {
        public Guid Id { get; set; }

        public string Description
        {
            get { return string.Format("{0}", ScorecardName); }
        }

        public string ScorecardCode { get; set; }

        public string ScorecardName { get; set; }

        public bool Active { get; set; }
    }
}