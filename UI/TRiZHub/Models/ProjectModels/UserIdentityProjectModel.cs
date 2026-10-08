#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TRiZHub.Models.ProjectModels
{
    //ClientName
    //ListOfProjects<UserIdentityProjectModel>

    //public class ClientProjects {
    //    public string Name { get; set; }
    //    public List<UserIdentityProjectModel> ListOfProjects { get; set; }
    //}


    public class UserIdentityProjectModel
    {
        public Guid Id { get; set; }

        public Guid ClientId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? SubProjectId { get; set; }

        public string Action { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool isActive { get; set; }

        public bool Selected { get; set; }

        /// <summary>Names of the teams this item is inherited through, for read-only display; null when not inherited.</summary>
        public string InheritedFrom { get; set; }

        public List<UserIdentityProjectModel> ListOfProjects { get; set; }

    }

    public class UserIdentityProjectSaveModel
    {
        public Guid UserId { get; set; }

        public List<UserIdentityProjectModel> Projects { get; set; }
    }
}