#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using TRiZHub.BL.Provider.WorkTeamData;

#endregion

namespace TRiZHub.Models.WorkTeamModels
{
    public class WorkTeamEditModel
    {
        public Guid? Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public Guid? TeamTypeId { get; set; }

        public string TeamTypeName { get; set; }

        public bool AllowTeamTimesheets { get; set; }

        public bool AllowTeamScorecards { get; set; }

        public bool LeadsManageAllocations { get; set; }

        public bool LeadsManageRates { get; set; }

        /// <summary>Output only: what the current user may do on this team.</summary>
        public WorkTeamPermissions Permissions { get; set; }
    }
}
