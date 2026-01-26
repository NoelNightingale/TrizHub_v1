#region Usings

using System;
using TCR.Lib.BL;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider
{
    public class TRiZHubProvider : ProviderBase<PrivilegeType>, ITRiZHubProvider
    {
        protected Guid? _SubscriberPackageId; //remeber and save some DB queries on the provider

        public TRiZHubProvider(DataContext context)
            : this(context, null)
        {
        }

        public TRiZHubProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        public ICurrentUser CurrentUser
        {
            get { return LoggedInUser as ICurrentUser; }
            set { LoggedInUser = value; }
        }

        public DataContext DataContext
        {
            get { return AuditDbContext as DataContext; }
        }

        protected DateTime DateEndOfDay(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, DateTimeKind.Utc);
        }

        protected DateTime DateOnly(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        public void BeginTransaction()
        {
            DataContext.BeginTransaction();
        }

        public void CommitTransaction()
        {
            DataContext.CommitTransaction();
        }

        public bool IsTransactionActive()
        {
            return DataContext.IsTransactionActive();
        }
    }
}