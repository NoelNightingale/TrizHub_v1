#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.Models.Account
{
    public class CurrentUserModel : ICurrentUser
    {
        public CurrentUserModel(ICurrentUser source)
        {
            Id = source.Id;
            UserName = source.UserName;
            DisplayName = source.DisplayName;
            AllowedPrivileges = source.AllowedPrivileges == null
                ? new List<PrivilegeType>()
                : source.AllowedPrivileges.ToList();
            IsSystemAdmin = source.IsSystemAdmin;
            IsUserApproved = source.IsUserApproved;
            IsUserProfileComplete = source.IsUserProfileComplete;
        }

        public Guid Id { get; }

        public string UserName { get; }

        public string DisplayName { get; }

        public List<PrivilegeType> AllowedPrivileges { get; }

        public bool IsSystemAdmin { get; }

        public bool IsUserApproved { get; }

        public bool IsUserProfileComplete { get; }
    }
}