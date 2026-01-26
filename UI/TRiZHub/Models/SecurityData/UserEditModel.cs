#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.SecurityData
{
    public class UserEditModel
    {
        public Guid? Id { get; set; }

        [Required]
        public string Account { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        public List<UserRoleModel> RoleList { get; set; }

        public DateTime? LockedOut { get; set; }

    }
}