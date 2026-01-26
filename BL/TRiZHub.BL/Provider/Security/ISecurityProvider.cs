#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Provider.Security
{
    public interface ISecurityProvider : ITRiZHubProvider
    {
        ICurrentUser UserLogin(string userName);
        ICurrentUser UserIdentityToCurrentUser(UserIdentity user, bool isUserProfileComplete = true);
        ICurrentUser GetCurrentUser(string userName);

        #region UserMaintenance

        UserIdentity SignUp(string accountName);

        UserIdentity SignUp(string accountName, string firstName, string surname, byte[] imageData,
            bool checkSubscriberRegister = true);

        UserIdentity UpdateProfilePic(byte[] imageData);

        UserIdentity SaveUser(Guid id, string accountName, string firstName, string surname, 
            List<Guid> allowedRoles, bool updateRoles = true);

        ICollection<Role> GetUserRoles(Guid userId);

        IQueryable<UserIdentity> GetUserList();

        IQueryable<UserAccount> GetUserAccountList();

        UserIdentity EditProfile(string firstName, string surname, string email, string imageFileName, byte[] imageData);

        UserIdentity GetMyProfile();

        UserIdentity ActivateAccount(Guid userId);

        UserIdentity DeactivateAccount(Guid userId);

        #endregion

        #region Roles

        Role SaveRole(Guid? id, string name, string description, StatusType status, List<PrivilegeType> privileges, bool isActive);

        Role GetRole(Guid id);

        IQueryable<Role> GetRoles();

        List<Privilege> GetRolePrivilegeList(Guid roleId);

        IQueryable<Privilege> GetPrivilegeList();

        #endregion
    }
}