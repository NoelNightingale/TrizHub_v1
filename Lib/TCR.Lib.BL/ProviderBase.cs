#region Usings

using System;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using TCR.Lib.SysLog;
using System.Collections.Generic;



#endregion

namespace TCR.Lib.BL
{
    public class ProviderBase<TPrivilegeTypeEnum> : IProviderBase<TPrivilegeTypeEnum>
    {
        public ProviderBase(IAuditDBContext<TPrivilegeTypeEnum> context)
            : this(context, null)
        {
            //String timeout = ConfigurationSettings.AppSettings["SqlCommandTimeout"].ToString();
            //((IObjectContextAdapter)context).ObjectContext.CommandTimeout = Int32.Parse(timeout);
        }

        public ProviderBase(IAuditDBContext<TPrivilegeTypeEnum> context, IUserContext<TPrivilegeTypeEnum> userContext)
        {
            //String timeout = ConfigurationSettings.AppSettings["SqlCommandTimeout"].ToString();
            //((IObjectContextAdapter)context).ObjectContext.CommandTimeout = Int32.Parse(timeout);
            AuditDbContext = context;
            LoggedInUser = userContext;
        }

        public IAuditDBContext<TPrivilegeTypeEnum> AuditDbContext { get; set; }
        public IUserContext<TPrivilegeTypeEnum> LoggedInUser { get; protected set; }

        public int DataContextSaveChanges()
        {
            try
            {
                if (LoggedInUser != null)
                    return AuditDbContext.SaveChanges(LoggedInUser);
                return AuditDbContext.SaveChanges();
            }
            catch (DbEntityValidationException error)
            {
                SendError(error);
                throw error;
            }
            catch (DbUpdateException err)
            {
                SendError(err);
                throw err;
            }
        }

        public bool UserIsAllowed(TPrivilegeTypeEnum privelege)
        {
            if (LoggedInUser == null)
                return false;

            if (LoggedInUser.IsSystemAdmin)
                return true;

            if (LoggedInUser.AllowedPrivileges == null)
                return false;

            if (LoggedInUser.AllowedPrivileges.Count == 0)
                return false;

            return LoggedInUser.AllowedPrivileges.Contains(privelege);
        }

        public void Authenticate(TPrivilegeTypeEnum privelege)
        {
            if (!UserIsAllowed(privelege))
                throw new GenericSecurityException("Not Allowed!");
        }

        public void AuthenticateList(IList<TPrivilegeTypeEnum> priveleges)
        {
            bool allowed = false;
            foreach (var privelege in priveleges)
            {
                allowed = allowed || UserIsAllowed(privelege);
            }
            if (!allowed)
                throw new GenericSecurityException("Not Allowed!");
        }

        #region Logging

        public void SendInformation(string message)
        {
            AuditDbContext.AddSystemLogEntry(this, Guid.NewGuid(), LoggedInUser == null ? (Guid?) null : LoggedInUser.Id,
                LogEventType.Information, message);
            SyslogSender.SendInformation(this, message);
        }

        public void SendWarning(string warningMessage)
        {
            AuditDbContext.AddSystemLogEntry(this, Guid.NewGuid(), LoggedInUser == null ? (Guid?) null : LoggedInUser.Id,
                LogEventType.Warning, warningMessage);
            SyslogSender.SendWarning(this, warningMessage);
        }

        public void SendCriticalError(Exception exception)
        {
            AuditDbContext.AddSystemLogEntry(this, Guid.NewGuid(), LoggedInUser == null ? (Guid?) null : LoggedInUser.Id,
                LogEventType.CriticalException,
                exception.Message,
                exception.StackTrace,
                exception.InnerException != null ? exception.InnerException.Message : string.Empty,
                exception.InnerException != null ? exception.InnerException.StackTrace : string.Empty
                );
            SyslogSender.SendCriticalError(this, exception);
        }

        public void SendError(Exception exception)
        {
            AuditDbContext.AddSystemLogEntry(this, Guid.NewGuid(), LoggedInUser == null ? (Guid?) null : LoggedInUser.Id,
                LogEventType.Error,
                exception.Message,
                exception.StackTrace,
                exception.InnerException != null ? exception.InnerException.Message : string.Empty,
                exception.InnerException != null ? exception.InnerException.StackTrace : string.Empty
                );
            SyslogSender.SendError(this, exception);
        }

        #endregion
    }
}