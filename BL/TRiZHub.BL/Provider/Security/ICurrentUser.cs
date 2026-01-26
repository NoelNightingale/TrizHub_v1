#region Usings

using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Provider.Security
{
    public interface ICurrentUser : IUserContext<PrivilegeType>
    {
    }
}