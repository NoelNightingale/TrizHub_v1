#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.TravelInformationData;

#endregion

namespace TRiZHub.BL.Provider.TravelInformationData
{
    public interface ITravelInfrormationProvider : ITRiZHubProvider
    {
        IQueryable<TravelInformation> TravelInformationFilterList(Guid userAccountId);

        TravelInformation SaveTravelInformation(Guid? id, Guid userAccountId, string documentType, string number,
            DateTime expiryDate);

        TravelInformation GetTravelInformation(Guid id);

        void DeleteTravelInformation(Guid id);
    }
}