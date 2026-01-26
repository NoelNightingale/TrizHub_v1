#region Usings

using System;

#endregion

namespace TCR.Lib.BL
{
    public interface IProviderBase<TPrivilegeTypeEnum>
    {
        IAuditDBContext<TPrivilegeTypeEnum> AuditDbContext { get; set; }
        IUserContext<TPrivilegeTypeEnum> LoggedInUser { get; }
        bool UserIsAllowed(TPrivilegeTypeEnum privelege);
        int DataContextSaveChanges();
        void SendInformation(string message);
        void SendWarning(string infoMessage);
        void SendCriticalError(Exception exception);
        void SendError(Exception exception);
    }
}