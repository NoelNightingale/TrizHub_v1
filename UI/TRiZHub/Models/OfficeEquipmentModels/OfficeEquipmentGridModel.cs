#region Usings

using System;

#endregion

namespace TRiZHub.Models.OfficeEquipmentModels
{
    public class OfficeEquipmentGridModel
    {
        public Guid? Id { get; set; }

        public Guid UserAccountId { get; set; }

        public string Account { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Type { get; set; }

        public string Model { get; set; }

        public string supplierName { get; set; }

        public string SerialNumber { get; set; }

        public decimal Cost { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime? AssignedDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public string AssetRegister { get; set; }

        public string Notes { get; set; }

        public Boolean IsAccountingItem { get; set; }
    }
}