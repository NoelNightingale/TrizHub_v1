#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.OfficeEquipmentData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.BillingRatesData;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.OfficeEquipmentData
{
    public class OfficeEquipmentProvider : TRiZHubProvider, IOfficeEquipmentProvider
    {
        #region Constructor

        public OfficeEquipmentProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        #region Office Equipment

        public IQueryable<OfficeEquipment> OfficeEquipementFilterList(Guid userAccountId)
        {
            Authenticate(PrivilegeType.UserAssetRegisterMaintenance);
            return DataContext.OfficeEquipmentSet.Where(a => a.UserAccountId == userAccountId);
        }

        public void DeleteOfficeEquipemnt(Guid id)
        {
            Authenticate(PrivilegeType.UserAssetRegisterMaintenance);
            var record = GetOfficeEquipment(id);

            if (record != null)
            {
                DataContext.OfficeEquipmentSet.Remove(record);
                DataContext.SaveChanges();
            }
        }

        public OfficeEquipment GetOfficeEquipment(Guid id)
        {
            Authenticate(PrivilegeType.UserAssetRegisterMaintenance);
            return DataContext.OfficeEquipmentSet.FirstOrDefault(a => a.Id == id);
        }

        public OfficeEquipment SaveOfficeEquipemnt(Guid? id, Guid userAccountId, string type, string model, string supplierName, string serialNumber,
            decimal cost, DateTime purchaseDate,
            string invoiceNumber, DateTime? assignedDate, DateTime? returnDate, string assetRegister, string notes, Boolean isAccoutingItem)
        {
            Authenticate(PrivilegeType.UserAssetRegisterMaintenance);

            if (id == Guid.Empty)
                id = null;

          

            if ((assignedDate != null && assignedDate.HasValue) && (assignedDate.Value.Date < purchaseDate.Date))
            {
                throw new OfficeEquipmentException("Assigned Date is before Purchase Date!");
            }

            //if(returnDate.Date < purchaseDate.Date || returnDate.Date < assignedDate.Date)
            //{
            //    throw new OfficeEquipmentException("Return Date is before or on Purchase or Assigned Date!");
            //}

            var existing = DataContext.OfficeEquipmentSet.FirstOrDefault(a => a.Id != id &&
                                                                              a.UserAccountId == userAccountId &&
                                                                              a.Type == type &&
                                                                              a.Model == model &&
                                                                              a.SerialNumber == serialNumber &&
                                                                              a.Cost == cost &&
                                                                              a.PurchaseDate == purchaseDate &&
                                                                              a.InvoiceNumber == invoiceNumber &&
                                                                              a.AssignedDate == assignedDate &&
                                                                              a.ReturnDate == returnDate &&
                                                                              a.SupplierName == supplierName &&
                                                                              a.AssetRegister == assetRegister &&
                                                                              a.Notes == notes &&
                                                                              a.IsAccountingItem == isAccoutingItem);
            if (existing != null)
                throw new OfficeEquipmentException("An Asset Register entry already exists.");

            var record = DataContext.OfficeEquipmentSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new OfficeEquipment
                {
                    UserAccountId = userAccountId
                };
                DataContext.OfficeEquipmentSet.Add(record);
            }

            


            record.Type = type;
            record.Model = model;
            record.SupplierName = supplierName;
            record.SerialNumber = serialNumber;
            record.Cost = cost;
            record.PurchaseDate = purchaseDate;
            record.InvoiceNumber = invoiceNumber;
            record.AssignedDate = assignedDate;
            record.ReturnDate = returnDate;
            record.AssetRegister = assetRegister;
            record.Notes = notes;
            record.IsAccountingItem = isAccoutingItem;

            DataContextSaveChanges();

            return record;
        }


        public void DropOfficeEquipment()
        {
            Authenticate(PrivilegeType.UserAssetRegisterMaintenance);

            DataContext.OfficeEquipmentSet.RemoveRange(DataContext.OfficeEquipmentSet.Where(oe => oe.UserAccount.Active));

            DataContext.SaveChanges();
        }



    }

    

    #endregion
}