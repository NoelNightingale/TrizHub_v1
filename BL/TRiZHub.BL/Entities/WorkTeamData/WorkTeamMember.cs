#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    [Table("WorkTeamMember")]
    public class WorkTeamMember : DbEntity
    {
        [Index("IDX_WorkTeamMemberTeam")]
        public virtual Guid WorkTeamId { get; set; }

        [ForeignKey("WorkTeamId")]
        public virtual WorkTeam WorkTeam { get; set; }

        [Index("IDX_WorkTeamMemberUser")]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        public virtual WorkTeamRoleType Role { get; set; }

        [Required]
        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }
    }
}
