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
using TRiZHub.BL.Entities.EmployerData;
using TRiZHub.BL.Entities.TeamJobDesignationData;

#endregion Usings

namespace TRiZHub.BL.Provider.ClientEntityData
{
    public class EmployerProvider : TRiZHubProvider, IEmployerProvider
    {
        #region Constructor

        private IList<PrivilegeType> getTokens;

        public EmployerProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            getTokens = new List<PrivilegeType>();
            getTokens.Add(PrivilegeType.EmployerMaintenance);
            getTokens.Add(PrivilegeType.UserMaintenance);
        }

        #endregion Constructor

        public IQueryable<Employer> EmployerList()
        {
            return DataContext.EmployerSet;
        }

        public Dictionary<Guid, string> GetUserEmployer(List<Guid> userIds)
        {
            AuthenticateList(getTokens);

            using (var ctx = new DataContext())
            {
                List<TeamJobDesignation> userTeams = new List<TeamJobDesignation>();
                foreach (var item in userIds)
                {
                    userTeams.Add(ctx.TeamJobDesignationSet.Where(t => t.UserAccountId == item && ((t.StartDate <= DateTime.Now && t.EndDate > DateTime.Now) || (t.StartDate <= DateTime.Now && t.EndDate == null))).FirstOrDefault());
                }

                Dictionary<Guid, string> userEmployers = new Dictionary<Guid, string>();
                foreach (var item in userTeams)
                {
                    if (item != null)
                    {
                        userEmployers.Add(item.UserAccountId, ctx.EmployerSet.Find(item.EmployerId).Name);
                    }
                }

                return userEmployers;
            }
        }

        public Employer GetEmployer(Guid id)
        {
            AuthenticateList(getTokens);
            return DataContext.EmployerSet.FirstOrDefault(a => a.Id == id);
        }

        public Employer SaveEmployer(Employer model)
        {
            //Authenticate(PrivilegeType.ProjectMaintenance);

            var existing =
                DataContext.EmployerSet.FirstOrDefault(
                    a => a.Name == model.Name && a.Id != model.Id);
            if (existing != null)
                throw new Exception("A Employer with the name: " + model.Name + " already exists.");

            var record = DataContext.EmployerSet.FirstOrDefault(a => a.Id == model.Id);
            if (record == null)
            {
                record = new Employer
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    DateCreated = DateTime.UtcNow,
                    IsActive = model.IsActive,
                    IsDeleted = false
                };
                DataContext.EmployerSet.Add(record);
            }
            else
            {
                record.Name = model.Name;
                record.IsActive = model.IsActive;
            }

            DataContextSaveChanges();

            return record;
        }

        public int Activate(Guid id)
        {
            AuthenticateList(getTokens);

            var record = DataContext.EmployerSet.FirstOrDefault(a => a.Id == id);

            record.IsActive = true;
            DataContextSaveChanges();
            return 1;
        }

        public int Deactivate(Guid id)
        {
            AuthenticateList(getTokens);

            var record = DataContext.EmployerSet.FirstOrDefault(a => a.Id == id);

            record.IsActive = false;
            DataContextSaveChanges();
            return 1;
        }

        public int Delete(Guid id)
        {
            AuthenticateList(getTokens);

            // Check that there are no records linked
            var linkedCount = DataContext.TeamJobDesignationSet.Where(tjd => tjd.EmployerId == id).Count();

            if (linkedCount > 0)
                throw new Exception("This employer has Team and Job Designations assigned to it, could not delete.");

            var record = DataContext.EmployerSet.FirstOrDefault(a => a.Id == id);

            record.IsActive = false;
            record.IsDeleted = true;
            DataContextSaveChanges();
            return 1;
        }

        //public IQueryable<ClientEntity> ClientEntityListForClientReporter()
        //{
        //    AuthenticateList(getTokens);
        //    return DataContext.ClientReporterSet.Where(u => u.UserAccountId == CurrentUser.Id).Select(a => a.Client);
        //}

        //public ClientEntity GetClientEntity(Guid id)
        //{
        //    AuthenticateList(getTokens);
        //    return DataContext.ClientEntitySet.FirstOrDefault(a => a.Id == id);
        //}

        //public ClientEntity SaveClientEntity(Guid? id, string customerName, bool isActive)
        //{
        //    Authenticate(PrivilegeType.ClientMaintenance);

        //    var existing = DataContext.ClientEntitySet.FirstOrDefault(a => a.EntityName == customerName && a.Id != id);
        //    if (existing != null)
        //        throw new ClientException("A client with the Customer name: " + customerName + " already exists.");

        //    var record = DataContext.ClientEntitySet.FirstOrDefault(a => a.Id == id);
        //    if (record == null)
        //    {
        //        record = new ClientEntity
        //        {
        //            DateCreated = DateTime.UtcNow,
        //            IsActive = isActive,
        //            EntityName = customerName

        //        };
        //        DataContext.ClientEntitySet.Add(record);
        //    }

        //    record.DateCreated = DateTime.UtcNow;
        //    record.IsActive = isActive;
        //    record.EntityName = customerName;

        //    DataContextSaveChanges();

        //    return record;
        //}

        //public IQueryable<UserIdentity> GetClientReporters(Guid id)
        //{
        //    AuthenticateList(getTokens);
        //    return DataContext.ClientReporterSet.Where(u => u.ClientId == id).Select(a => a.UserIdentity);
        //}

        //public void AddClientReporter(Guid clientId, Guid userId)
        //{
        //    Authenticate(PrivilegeType.ClientMaintenance);
        //    var clientReporter = DataContext.ClientReporterSet
        //        .Where(c => c.ClientId == clientId)
        //        .Where(c => c.UserAccountId == userId)
        //        .FirstOrDefault();

        //    if (clientReporter == null)
        //    {
        //        clientReporter = new ClientReporter();
        //        clientReporter.ClientId = clientId;
        //        clientReporter.UserAccountId = userId;
        //        clientReporter.Id = Guid.NewGuid();
        //        DataContext.ClientReporterSet.Add(clientReporter);
        //        DataContextSaveChanges();
        //    }

        //}

        //public void RemoveClientReporter(Guid clientId, Guid userId)
        //{
        //    Authenticate(PrivilegeType.ClientMaintenance);
        //    var clientReporter = DataContext.ClientReporterSet
        //        .Where(c => c.ClientId == clientId)
        //        .Where(c => c.UserAccountId == userId)
        //        .FirstOrDefault();

        //    if (clientReporter != null)
        //    {
        //        DataContext.ClientReporterSet.Remove(clientReporter);
        //        DataContextSaveChanges();
        //    }
        //}

        //public int DeleteClient(Guid id)
        //{
        //    Authenticate(PrivilegeType.ClientMaintenance);

        //    // Check if the client is associated with a Project
        //    var projectCount = DataContext.ProjectSet.Count(p => p.ClientId == id);

        //    if (projectCount > 0)
        //    {
        //        return 0;
        //    }

        //    // Delete Client
        //    var client = DataContext.ClientEntitySet.FirstOrDefault(c => c.Id == id);
        //    client.IsDeleted = true;

        //    DataContextSaveChanges();

        //    return 1;
        //}
    }
}