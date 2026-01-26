#region Usings

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.SecurityData
{
    [Table("Role")]
    public class Role : DbEntity
    {
        [Required]
        [MaxLength(100)]
        [Index("UIDX_RoleRoleName", IsUnique = true, Order = 0)]
        public virtual string RoleName { get; set; }

        public virtual string Description { get; set; }
        public virtual StatusType Status { get; set; }
        public virtual ICollection<Privilege> Privileges { get; set; }
        public virtual ICollection<UserAccount> AdminUsers { get; set; }
        public virtual bool isActive { get; set; }
    }
}