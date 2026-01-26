#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.EmployerData;

#endregion Usings

namespace TRiZHub.BL.Entities.TeamJobDesignationData
{
    [Table("TeamJobDesignation")]
    public class TeamJobDesignation : DbEntity
    {
        [Index("IDX_TeamJobDesignationUserAccount", Order = 0)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        [Required]
        public virtual Guid ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual ClientEntity Client { get; set; }

        public virtual Guid? LineLeaderId { get; set; }

        [ForeignKey("LineLeaderId")]
        public virtual UserAccount LineLeader { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string JobDesignation { get; set; }

        [Required]
        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Location { get; set; }

        public virtual Guid? EmployerId { get; set; }

        [ForeignKey("EmployerId")]
        public virtual Employer Employer { get; set; }
    }
}