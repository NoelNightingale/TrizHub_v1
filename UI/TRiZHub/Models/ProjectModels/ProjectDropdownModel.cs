#region Usings

using System;

#endregion

namespace TRiZHub.Models.ProjectModels
{
    public class ProjectDropdownModel
    {
        public Guid Id
        {
            get { return SubProjectId != null ? SubProjectId.Value : ProjectId; }
        }

        public string Description
        {
            get { return string.Format("{0} {1}", ProjectName, ((SubProjectName == null || SubProjectName.Equals("")) ? "" : " [" + SubProjectName + "]")); }
        }

        public Guid ProjectId { get; set; }

        public Guid? SubProjectId { get; set; }

        public string ProjectName { get; set; }

        public string SubProjectName { get; set; }

        public bool IsActive { get; set; }

        public bool IsBillable { get; set; }

        public string ClientName { get; set; }
        public Guid ClientId { get; set; }
    }
}