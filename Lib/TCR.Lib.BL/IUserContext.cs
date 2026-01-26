#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TCR.Lib.BL
{
    public interface IUserContext<TPrivilegeTypeEnum>
    {
        #region User Details

        Guid Id { get; }

        string UserName { get; }

        string DisplayName { get; }

        List<TPrivilegeTypeEnum> AllowedPrivileges { get; }

        bool IsSystemAdmin { get; }

        bool IsUserApproved { get; }

        bool IsUserProfileComplete { get; }

        #endregion
    }
}