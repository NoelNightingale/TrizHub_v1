#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.ClientReporterData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using System.Collections.Generic;
using TRiZHub.BL.Entities.UserIdentityClient;


using System.Data.Entity;

#endregion

namespace TRiZHub.BL.Provider.ClientEntityData
{
    public class ClientProvider : TRiZHubProvider, IClientProvider
    {
        #region Constructor
        IList<PrivilegeType> getTokens;

        public ClientProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            getTokens = new List<PrivilegeType>();
            getTokens.Add(PrivilegeType.ClientMaintenance);
            getTokens.Add(PrivilegeType.ReportGenerationTimesheet);
            getTokens.Add(PrivilegeType.CustomerReportAccess);
            getTokens.Add(PrivilegeType.TimesheetCaptureForOtherAccounts);
            getTokens.Add(PrivilegeType.TimesheetCapture);
        }


        #endregion

        public IQueryable<ClientEntity> ClientEntityList()
        {
            //AuthenticateList(getTokens);
            return DataContext.ClientEntitySet;
        }


        public IQueryable<ClientEntity> ClientEntityListForClientReporter()
        {
            AuthenticateList(getTokens);
            return DataContext.ClientReporterSet.Where(u => u.UserAccountId == CurrentUser.Id).Select(a => a.Client);
        }


        public ClientEntity GetClientEntity(Guid id)
        {
            AuthenticateList(getTokens);
            return DataContext.ClientEntitySet.FirstOrDefault(a => a.Id == id);
        }

        public ClientEntity SaveClientEntity(Guid? id, string customerName, bool isActive)
        {
            Authenticate(PrivilegeType.ClientMaintenance);

            var existing = DataContext.ClientEntitySet.FirstOrDefault(a => a.EntityName == customerName && a.Id != id);
            if (existing != null)
                throw new ClientException("A client with the Customer name: " + customerName + " already exists.");

            var record = DataContext.ClientEntitySet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new ClientEntity
                {
                    DateCreated = DateTime.UtcNow,
                    IsActive = isActive,
                    EntityName = customerName

                };
                DataContext.ClientEntitySet.Add(record);
            }


            record.DateCreated = DateTime.UtcNow;
            record.IsActive = isActive;
            record.EntityName = customerName;

            DataContextSaveChanges();

            return record;
        }


        public IQueryable<UserIdentity> GetClientReporters(Guid id)
        {
            AuthenticateList(getTokens);
            return DataContext.ClientReporterSet.Where(u => u.ClientId == id).Select(a => a.UserIdentity);
        }

        public void AddClientReporter(Guid clientId, Guid userId)
        {
            Authenticate(PrivilegeType.ClientMaintenance);
            var clientReporter = DataContext.ClientReporterSet
                .Where(c => c.ClientId == clientId)
                .Where(c => c.UserAccountId == userId)
                .FirstOrDefault();

            if (clientReporter == null)
            {
                clientReporter = new ClientReporter();
                clientReporter.ClientId = clientId;
                clientReporter.UserAccountId = userId;
                clientReporter.Id = Guid.NewGuid();
                DataContext.ClientReporterSet.Add(clientReporter);
                DataContextSaveChanges();
            }

        }

        public void RemoveClientReporter(Guid clientId, Guid userId)
        {
            Authenticate(PrivilegeType.ClientMaintenance);
            var clientReporter = DataContext.ClientReporterSet
                .Where(c => c.ClientId == clientId)
                .Where(c => c.UserAccountId == userId)
                .FirstOrDefault();

            if (clientReporter != null)
            {
                DataContext.ClientReporterSet.Remove(clientReporter);
                DataContextSaveChanges();
            }
        }

        public int DeleteClient(Guid id)
        {
            Authenticate(PrivilegeType.ClientMaintenance);

            // Check if the client is associated with a Project
            var projectCount = DataContext.ProjectSet.Count(p => p.ClientId == id);

            if (projectCount > 0)
            {
                return 0;
            }

            // Delete Client
            var client = DataContext.ClientEntitySet.FirstOrDefault(c => c.Id == id);
            client.IsDeleted = true;

            DataContextSaveChanges();

            return 1;
        }


        public IQueryable<UserIdentityClient> GetUserAllocatedClients(Guid userID)
        {
            return DataContext.UserIdentityClientSet.Where(u => u.UserAccountId == userID)
                .Include(c => c.Client);
        }

        public void SaveUserAlllocation(Guid userId, List<UserIdentityClient> clients)
        {
            Authenticate(PrivilegeType.UserProjectMaintenance);

            // Remove old
            var toDelete = DataContext.UserIdentityClientSet.Where(x => x.UserAccountId == userId);
            DataContext.UserIdentityClientSet.RemoveRange(toDelete);
            DataContext.UserIdentityClientSet.AddRange(clients);
            DataContextSaveChanges();

        }


    }
}