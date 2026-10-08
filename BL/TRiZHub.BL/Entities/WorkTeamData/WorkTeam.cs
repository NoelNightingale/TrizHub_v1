#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.TeamData;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    [Table("WorkTeam")]
    public class WorkTeam : DbEntity
    {
        [Required]
        [MaxLength(500)]
        public virtual string Name { get; set; }

        [MaxLength(2000)]
        public virtual string Description { get; set; }

        public virtual bool IsActive { get; set; }

        [Index("IDX_WorkTeamTeamType")]
        public virtual Guid TeamTypeId { get; set; }

        [ForeignKey("TeamTypeId")]
        public virtual Team TeamType { get; set; }

        public virtual bool AllowTeamTimesheets { get; set; }

        public virtual bool AllowTeamScorecards { get; set; }

        public virtual bool LeadsManageAllocations { get; set; }

        public virtual bool LeadsManageRates { get; set; }

        public virtual ICollection<WorkTeamMember> Members { get; set; }

        public virtual ICollection<WorkTeamClient> Clients { get; set; }

        public virtual ICollection<WorkTeamProject> Projects { get; set; }
    }
}
