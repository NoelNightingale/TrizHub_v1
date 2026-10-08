#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TRiZHub.Models.ProjectModels;

#endregion

namespace TRiZHub.Models.WorkTeamModels
{
    public class WorkTeamAllocationSaveModel
    {
        [Required]
        public Guid WorkTeamId { get; set; }

        /// <summary>Selected tree nodes: no ProjectId means the whole client, no SubProjectId means the whole project.</summary>
        public List<UserIdentityProjectModel> Selection { get; set; }
    }
}
