#region Usings

using System;

#endregion

namespace TRiZHub.Models.ProjectModels
{
    public class SubProjectGridModel
    {
        public Guid Id { get; set; }

        public string ParentProjectNumber { get; set; }

        public string ParentProjectName { get; set; }

        public string ProjectName { get; set; }

        public string SubProjectNumber { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }
    }
}