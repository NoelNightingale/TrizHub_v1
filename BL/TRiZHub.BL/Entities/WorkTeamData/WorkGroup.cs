#region Usings

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;

#endregion

namespace TRiZHub.BL.Entities.WorkTeamData
{
    [Table("WorkGroup")]
    public class WorkGroup : DbEntity
    {
        [Required]
        [MaxLength(500)]
        public virtual string Name { get; set; }

        [MaxLength(2000)]
        public virtual string Description { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual ICollection<WorkGroupClient> Clients { get; set; }

        public virtual ICollection<WorkGroupManager> Managers { get; set; }

        public virtual ICollection<WorkTeam> Teams { get; set; }
    }
}
