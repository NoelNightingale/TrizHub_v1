#region Usings

using System;

#endregion

namespace TRiZHub.Models.ProjectModels
{
    public class ProjectModel
    {
        public Guid? Id { get; set; }

        public Guid ClientId { get; set; }

        public Guid? ProjectLeadId { get; set; }
        public Guid? ProjectTypeId { get; set; }
        public bool AllowSubProjectAlternativeType { get; set; }
        public bool HasSubprojects { get; set; }

        public string ClientName { get; set; }

        public string ProjectLeadName { get; set; }

        public string ProjectName { get; set; }

        public string ProjectNumber { get; set; }

        public string ProjectDescription { get; set; }

        public bool Billable { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }

        public bool ExcludeTimeCapture { get; set; }
    }
}