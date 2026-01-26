#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.OfficeEquipmentData
{
    public class OfficeEquipment : DbEntity
    {
        [Index("IDX_OfficeEquipmentUserAccount", Order = 0)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Type { get; set; }

        [MaxLength(500)]
        public virtual string Model { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string SupplierName { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string SerialNumber { get; set; }

        [Required]
        public virtual decimal Cost { get; set; }

        [Required]
        public virtual DateTime PurchaseDate { get; set; }

        [Required]
        public virtual string InvoiceNumber { get; set; }

        public virtual DateTime? AssignedDate { get; set; }

        public virtual DateTime? ReturnDate { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string AssetRegister { get; set; }

        public virtual string Notes { get; set; }

        public virtual Boolean IsAccountingItem { get; set; }
    }
}