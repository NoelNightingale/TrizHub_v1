#region Usings

using System;
using System.Collections.Generic;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Provider.Security
{
    public class LoggedInUser : ICurrentUser
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public List<PrivilegeType> AllowedPrivileges { get; set; }

        public bool IsSystemAdmin { get; set; }

        public bool IsUserApproved { get; set; }

        public bool IsUserProfileComplete { get; set; }
    }
}