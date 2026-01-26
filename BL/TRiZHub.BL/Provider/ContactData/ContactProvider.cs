#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ContactData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.ContactData
{
    public class ContactProvider : TRiZHubProvider, IContactProvider
    {
        #region Constructor

        public ContactProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        public EmergancyContact SaveEmergancyContact(Guid? id, Guid userAccountId, string name,
            string surname, string relationship, string cellphoneNumber,
            string landLineNumber)
        {
            Authenticate(PrivilegeType.UserEmergencyContactMaintenance);

            if (id == Guid.Empty)
                id = null;

            var existing = DataContext.EmergancyContactSet.FirstOrDefault(u => u.UserAccountId != userAccountId &&
                                                                               u.Name == name &&
                                                                               u.Surname == surname &&
                                                                               u.Relationship == relationship &&
                                                                               u.CellphoneNumber == cellphoneNumber &&
                                                                               u.LandLineNumber == landLineNumber
                                                                               && u.Id != id);

            if (existing != null)
            {
                throw new ContactException("A emregency contact entry already exists");
            }

            var record = DataContext.EmergancyContactSet.FirstOrDefault(a => a.Id == id);

            if (record == null)
            {
                record = new EmergancyContact
                {
                    UserAccountId = userAccountId
                };

                DataContext.EmergancyContactSet.Add(record);
            }

            record.UserAccountId = userAccountId;
            record.Name = name;
            record.Surname = surname;
            record.Relationship = relationship;
            record.CellphoneNumber = cellphoneNumber;
            record.LandLineNumber = landLineNumber;

            DataContextSaveChanges();

            return record;
        }

        public EmergancyContact GetEmergancyContact(Guid id)
        {
            Authenticate(PrivilegeType.UserEmergencyContactMaintenance);
            return DataContext.EmergancyContactSet.FirstOrDefault(a => a.Id == id);
        }

        public void DeleteEmergencyContact(Guid id)
        {
            Authenticate(PrivilegeType.UserEmergencyContactMaintenance);
            var record = GetEmergancyContact(id);

            if (record != null)
            {
                DataContext.EmergancyContactSet.Remove(record);
                DataContextSaveChanges();
            }
        }

        public IQueryable<EmergancyContact> EmergencyContactFilterList(Guid userAccountId)
        {
            Authenticate(PrivilegeType.UserEmergencyContactMaintenance);
            return DataContext.EmergancyContactSet.Where(a => a.UserAccountId == userAccountId);
        }
    }
}