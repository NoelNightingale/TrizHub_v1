#region Usings

using System;

#endregion

namespace TRiZHub.Models.ProjectModels
{
    public class ProjectTypeDropdownModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool AllowSubProjectAlternativeType { get; set; }

        public bool AllowSubProjectBillable { get; set; }

        public int SortOrder { get; set; }
    }
}