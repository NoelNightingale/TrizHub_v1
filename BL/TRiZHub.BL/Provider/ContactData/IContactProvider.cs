#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.ContactData;

#endregion

namespace TRiZHub.BL.Provider.ContactData
{
    public interface IContactProvider : ITRiZHubProvider
    {
        EmergancyContact SaveEmergancyContact(Guid? id, Guid userAccountId, string name,
            string surname, string relationship, string cellphoneNumber, string landLineNumber);

        EmergancyContact GetEmergancyContact(Guid id);

        void DeleteEmergencyContact(Guid id);

        IQueryable<EmergancyContact> EmergencyContactFilterList(Guid userAccountId);
    }
}