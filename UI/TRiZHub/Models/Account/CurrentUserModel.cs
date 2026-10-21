#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.WorkTeamData;

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

        /// <summary>Only filled by <c>Account/GetCurrentUser</c>; null on per-request instances.</summary>
        public WorkTeamCapabilitySummary TeamCapabilities { get; set; }

        /// <summary>True on a Debug build answering a localhost request.</summary>
        public bool CanImpersonate { get; set; }

        /// <summary>True when this request is running as the account named by the local impersonation cookie.</summary>
        public bool Impersonating { get; set; }
    }
}