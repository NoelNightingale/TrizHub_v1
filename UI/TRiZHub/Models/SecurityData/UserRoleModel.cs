#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.SecurityData
{
    public class UserRoleModel
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }

        [Display(Name = "Selected")]
        public bool Selected { get; set; }
    }
}