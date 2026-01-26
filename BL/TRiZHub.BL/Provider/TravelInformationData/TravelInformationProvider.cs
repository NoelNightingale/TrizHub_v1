#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.TravelInformationData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.TravelInformationData
{
    public class TravelInformationProvider : TRiZHubProvider, ITravelInfrormationProvider
    {
        #region Constructor

        public TravelInformationProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        #region travel Information

        public TravelInformation GetTravelInformation(Guid id)
        {
            Authenticate(PrivilegeType.UserTravelInformationMaintenance);
            return DataContext.TravelInformationSet.FirstOrDefault(a => a.Id == id);
        }

        public void DeleteTravelInformation(Guid id)
        {
            Authenticate(PrivilegeType.UserTravelInformationMaintenance);
            var record = GetTravelInformation(id);

            if (record != null)
            {
                DataContext.TravelInformationSet.Remove(record);
                DataContext.SaveChanges();
            }
        }


        public TravelInformation SaveTravelInformation(Guid? id, Guid userAccountId, string documentType, string number,
            DateTime expiryDate)
        {
            Authenticate(PrivilegeType.UserTravelInformationMaintenance);

            if (id == Guid.Empty)
                id = null;

            var existing =
                DataContext.TravelInformationSet.FirstOrDefault(u => u.UserAccountId == userAccountId &&
                                                                     u.DocumentType == documentType &&
                                                                     u.Number == number &&
                                                                     u.ExpiryDate == expiryDate &&
                                                                     u.Id != id);

            if (existing != null)
            {
                throw new TravelInfromationException("A Travel Information entry already exists.");
            }

            var record = DataContext.TravelInformationSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new TravelInformation
                {
                    UserAccountId = userAccountId
                };
                DataContext.TravelInformationSet.Add(record);
            }


            record.DocumentType = documentType;
            record.Number = number;
            record.ExpiryDate = expiryDate;

            DataContextSaveChanges();

            return record;
        }

        public IQueryable<TravelInformation> TravelInformationFilterList(Guid userAccountId)
        {
            Authenticate(PrivilegeType.UserTravelInformationMaintenance);
            return DataContext.TravelInformationSet.Where(a => a.UserAccountId == userAccountId);
        }

        #endregion
    }
}