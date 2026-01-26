#region Usings

using TCR.Lib.BL;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider
{
    public interface ITRiZHubProvider : IProviderBase<PrivilegeType>
    {
        DataContext DataContext { get; }
        ICurrentUser CurrentUser { get; set; }

        void BeginTransaction();
        void CommitTransaction();
        bool IsTransactionActive();
    }
}