#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.OfficeEquipmentModels
{
    public class OfficeEquipmentEditModel
    {
        public Guid? Id { get; set; }

        public Guid UserAccountId { get; set; }

        public string Account { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        [Required]
        public string Type { get; set; }

        public string Model { get; set; }

        [Required]
        public string SupplierName { get; set; }

        [Required]
        public string SerialNumber { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        public DateTime? AssignedDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required]
        public string AssetRegister { get; set; }

        public string Notes { get; set; }

        public Boolean IsAccountingItem { get; set; }
    }
}