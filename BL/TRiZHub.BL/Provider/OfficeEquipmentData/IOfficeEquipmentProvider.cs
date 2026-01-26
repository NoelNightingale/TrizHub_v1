#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.OfficeEquipmentData;

#endregion

namespace TRiZHub.BL.Provider.OfficeEquipmentData
{
    public interface IOfficeEquipmentProvider : ITRiZHubProvider
    {
        IQueryable<OfficeEquipment> OfficeEquipementFilterList(Guid userAccountId);

        OfficeEquipment SaveOfficeEquipemnt(Guid? id, Guid userAccountId, string type, string model, string supllierName, string serialNumber,
            decimal cost, DateTime purchaseDate, string invoiceNumber, DateTime? assignedDate, DateTime? returnDate,
            string assetRegister, string notes, Boolean isAccountingItem);

        OfficeEquipment GetOfficeEquipment(Guid id);

        void DeleteOfficeEquipemnt(Guid id);
    }
}