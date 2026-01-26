#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.UserIdentityClient;

#endregion

namespace TRiZHub.BL.Provider.ClientEntityData
{
    public interface IClientProvider : ITRiZHubProvider
    {
        IQueryable<ClientEntity> ClientEntityList();
        IQueryable<ClientEntity> ClientEntityListForClientReporter();
        IQueryable<UserIdentityClient> GetUserAllocatedClients(Guid userID);
        void SaveUserAlllocation(Guid userId, List<UserIdentityClient> clients);

        int DeleteClient(Guid id);

        #region Customer

        ClientEntity SaveClientEntity(Guid? id, string customerName, bool isActive);

        ClientEntity GetClientEntity(Guid id);

        IQueryable<UserIdentity> GetClientReporters(Guid id);

        void AddClientReporter(Guid clientiId, Guid userId);

        void RemoveClientReporter(Guid clientid, Guid userId);


        #endregion

    }
}