#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    [Table("WorkTeamMemberCapability")]
    public class WorkTeamMemberCapability : DbEntity
    {
        [Index("UIDX_WorkTeamMemberCapability", IsUnique = true, Order = 0)]
        public virtual Guid WorkTeamMemberId { get; set; }

        [ForeignKey("WorkTeamMemberId")]
        public virtual WorkTeamMember WorkTeamMember { get; set; }

        [Index("UIDX_WorkTeamMemberCapability", IsUnique = true, Order = 1)]
        public virtual WorkAssignmentCapability Capability { get; set; }

        public virtual bool IsAllowed { get; set; }
    }
}
