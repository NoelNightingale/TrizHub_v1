#region Usings

using System;

#endregion

namespace TRiZHub.Models.ProjectModels
{
    public class ProjectGridModel
    {
        public Guid Id { get; set; }

        public string ClientName { get; set; }

        public string ProjectLeadName { get; set; }

        public string ProjectName { get; set; }

        public string ProjectNumber { get; set; }

        public string ProjectDescription { get; set; }

        public bool Billable { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }
    }
}