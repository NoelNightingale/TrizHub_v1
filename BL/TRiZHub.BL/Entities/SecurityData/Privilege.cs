#region Usings

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.SecurityData
{
    [Table("Privilege")]
    public class Privilege : DbEntity
    {
        [Index("IDX_PrivilegeSecurity", IsUnique = true)]
        public virtual PrivilegeType Security { get; set; }

        [Required]
        [MaxLength(200)]
        public virtual string Description { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}