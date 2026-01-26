#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.SecurityData
{
    public class RoleViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string RoleName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public StatusType StatusType { get; set; }

        public string Status
        {
            get { return StatusType.ToString(); }

            set { Status.ToString(); }
        }

        public List<PermissionViewModel> Permissions { get; set; }

        public bool IsActive { get; set; }
    }
}