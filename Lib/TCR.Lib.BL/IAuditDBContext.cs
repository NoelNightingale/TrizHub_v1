#region Usings

using System;
using System.Data.Entity;

#endregion

namespace TCR.Lib.BL
{
    public interface IAuditDBContext<TPrivilegeTypeEnum>
    {
        int SaveChanges(IUserContext<TPrivilegeTypeEnum> currentUser);
        int SaveChanges();

        void AddSystemLogEntry(object sender, Guid guid, Guid? currentUserId, LogEventType logEventType,
            string message, string stackTrace = null, string innerExceptionMessage = null,
            string innerExceptionStackTrace = null);

        DbContextTransaction ContextTransaction { get; set; }
        void BeginTransaction();
        void CommitTransaction();
        bool IsTransactionActive();
    }
}