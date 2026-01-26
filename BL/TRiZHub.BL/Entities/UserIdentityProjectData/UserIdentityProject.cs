#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.ProjectData;

#endregion Usings

namespace TRiZHub.BL.Entities.UserIdentityProject
{
    [Table("UserIdentityProject")]
    public class UserIdentityProject : DbEntity
    {
        [Index("IDX_UserIdentityProject", Order = 0)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        public virtual Guid? ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public virtual Guid? SubProjectId { get; set; }

        [ForeignKey("SubProjectId")]
        public virtual SubProject SubProject { get; set; }
    }
}