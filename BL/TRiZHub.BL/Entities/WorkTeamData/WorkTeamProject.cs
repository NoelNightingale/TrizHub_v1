#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ProjectData;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    [Table("WorkTeamProject")]
    public class WorkTeamProject : DbEntity
    {
        [Index("IDX_WorkTeamProjectTeam")]
        public virtual Guid WorkTeamId { get; set; }

        [ForeignKey("WorkTeamId")]
        public virtual WorkTeam WorkTeam { get; set; }

        [Index("IDX_WorkTeamProjectProject")]
        public virtual Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        /// <summary>Null means the whole project.</summary>
        public virtual Guid? SubProjectId { get; set; }

        [ForeignKey("SubProjectId")]
        public virtual SubProject SubProject { get; set; }
    }
}
