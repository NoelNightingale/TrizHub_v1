#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.WorkTeamModels
{
    public class WorkTeamMemberModel
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid WorkTeamId { get; set; }

        [Required]
        public Guid UserAccountId { get; set; }

        public string UserName { get; set; }

        [Required]
        public WorkTeamRoleType Role { get; set; }

        public string RoleName
        {
            get { return Role.ToString(); }
        }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsCurrent { get; set; }

        public List<WorkTeamMemberPrivilegeModel> Privileges { get; set; }
    }

    public class WorkTeamMemberPrivilegeModel
    {
        public WorkTeamPrivilege Privilege { get; set; }

        public string Name { get; set; }

        /// <summary>What this role gets when the membership has no override.</summary>
        public bool RoleDefault { get; set; }

        /// <summary>Null means use the role default.</summary>
        public bool? Override { get; set; }
    }

    public class WorkTeamRolePrivilegeModel
    {
        public WorkTeamRoleType Role { get; set; }

        public WorkTeamPrivilege Privilege { get; set; }

        public string Name { get; set; }

        public bool IsGranted { get; set; }
    }
}
