#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    /// <summary>
    /// An explicit grant or denial for one membership. No row means the role default applies.
    /// </summary>
    [Table("WorkTeamMemberPrivilege")]
    public class WorkTeamMemberPrivilege : DbEntity
    {
        [Index("UIDX_WorkTeamMemberPrivilege", IsUnique = true, Order = 0)]
        public virtual Guid WorkTeamMemberId { get; set; }

        [ForeignKey("WorkTeamMemberId")]
        public virtual WorkTeamMember WorkTeamMember { get; set; }

        [Index("UIDX_WorkTeamMemberPrivilege", IsUnique = true, Order = 1)]
        public virtual WorkTeamPrivilege Privilege { get; set; }

        public virtual bool IsGranted { get; set; }
    }
}
