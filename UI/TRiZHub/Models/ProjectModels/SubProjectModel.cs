#region Usings

using System;

#endregion

namespace TRiZHub.Models.ProjectModels
{
    public class SubProjectModel
    {
        public Guid? Id { get; set; }

        public Guid ProjectId { get; set; }

        public Guid? SubProjectTypeId { get; set; }

        public string ParentProjectName { get; set; }
        public Guid? ParentProjectTypeId { get; set; }

        public bool ParentAllowSubProjectAlternativeType { get; set; }

        public string subProjectNumber { get; set; }

        public string ProjectName { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }
    }
}