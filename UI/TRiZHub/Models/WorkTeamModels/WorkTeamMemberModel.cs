#region Usings

using System;
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
    }
}
