#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.ContactData
{
    [Table("EmergancyContact")]
    public class EmergancyContact : DbEntity
    {
        [Index("IDX_EmergancyContactUserAccount", Order = 0)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Surname { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Relationship { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string CellphoneNumber { get; set; }

        [MaxLength(500)]
        public virtual string LandLineNumber { get; set; }
    }
}