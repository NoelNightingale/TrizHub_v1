#region Usings

using System;

#endregion

namespace TRiZHub.Models.ProjectModels
{
    public class UserProjectGridModel
    {
        public Guid Id { get; set; }

        public string Description
        {
            get { return string.Format("{0}", ((SubProjectName == null || SubProjectName.Equals("")) ?
                "[" + ProjectNumber + "] " + ProjectName : 
                "[" + ProjectNumber + "-" + SubProjectNumber + "] " + ProjectName + " [" + SubProjectName + "]")); }
        }

        public Guid? ProjectId { get; set; }

        public Guid? SubProjectId { get; set; }

        public string ProjectName { get; set; }

        public string SubProjectName { get; set; }



        public string ProjectNumber { get; set; }

        public string SubProjectNumber { get; set; }

        public bool IsActive { get; set; }

        public bool IsBillable { get; set; }

        public string ClientName { get; set; }
        public Guid? ClientId { get; set; }
    }
}