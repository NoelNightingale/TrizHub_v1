#region Usings

using System;
using TCR.Lib.BL;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Email;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Test.Mocs
{
    public class FakeEmailProvider : IEmailProvider
    {
        public IAuditDBContext<PrivilegeType> AuditDbContext { get; set; }

        public ICurrentUser CurrentUser { get; set; }
        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public bool IsTransactionActive()
        {
            throw new NotImplementedException();
        }

        public DataContext DataContext { get; set; }

        public IUserContext<PrivilegeType> LoggedInUser { get; set; }

        public int DataContextSaveChanges()
        {
            return 0;
        }

      
        public void SendCriticalError(Exception exception)
        {
        }

        public void SendError(Exception exception)
        {
        }

        public void SendInformation(string message)
        {
        }

        public void SendPasswordEmailToUser(UserIdentity user, string password)
        {
        } 

        public void SendWarning(string infoMessage)
        {
        }

        public bool UserIsAllowed(PrivilegeType privelege)
        {
            return true;
        }
    }
}