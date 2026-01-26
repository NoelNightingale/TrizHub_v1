#region Usings

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Resources;

#endregion

namespace TRiZHub.BL.Provider.Security
{
    public class SecurityProvider : TRiZHubProvider, ISecurityProvider
    {
        public ICurrentUser UserLogin(string accountName)
        {
            var result = DataContext.UserIdentitySet.SingleOrDefault(x => x.AccountName == accountName);

            if (result == null)
            {
                //New User to be created...
                try
                {
                    result = SignUp(accountName);
                }
                catch (Exception d)
                {
                    var a = d;
                }
            }

            if (result.Active == false)
                throw new SecurityException("Account Deactivated!");

            if (result.IsSystemAdmin)
            {
                var adminUser = result as UserAccount;

                var qPrivs = from userP in DataContext.UserAccountSet
                             from role in userP.Roles
                             from privileges in role.Privileges
                             where userP.Id == result.Id
                             select privileges.Security;

                adminUser.AllowedPrivileges = qPrivs.Distinct().ToList();

                if (result is UserAccount && result.IsSystemAdmin)
                    adminUser.AllowedPrivileges = DataContext.PrivilegeSet.Select(a => a.Security).ToList();
            }

            if (result is UserAccount)
                CurrentUser = UserIdentityToCurrentUser(result);

            return CurrentUser;
        }

        public ICurrentUser UserIdentityToCurrentUser(UserIdentity user, bool isUserProfileComplete = true)
        {
            return new LoggedInUser
            {
                Id = user.Id,
                UserName = user.AccountName,
                DisplayName = user.FirstName + " " + user.Surname,
                AllowedPrivileges = (user as UserAccount).AllowedPrivileges,
                IsSystemAdmin = (user as UserAccount).IsSystemAdmin,
                IsUserProfileComplete = isUserProfileComplete
            };
        }

        public ICurrentUser GetCurrentUser(string userName)
        {
            var result =
                DataContext.UserIdentitySet.SingleOrDefault(x => x.AccountName == userName);

            if (result == null)
                throw new SecurityException("User not logged in!");

            if (result.Active == false)
                throw new SecurityException("Account Deactivated!");


            var adminUser = result as UserAccount;

            var qPrivs = from userP in DataContext.UserAccountSet
                         from role in userP.Roles
                         from privileges in role.Privileges
                         where userP.Id == result.Id
                         select privileges.Security;

            adminUser.AllowedPrivileges = qPrivs.Distinct().ToList();

            if (result is UserAccount && result.IsSystemAdmin)
                adminUser.AllowedPrivileges = DataContext.PrivilegeSet.Select(a => a.Security).ToList();

            if (result is UserAccount)
                CurrentUser = UserIdentityToCurrentUser(result, (result as UserAccount).ProfileComplete);

            return CurrentUser;
        }

        #region Constructor

        public SecurityProvider(DataContext context)
            : this(context, null)
        {
        }

        public SecurityProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        #region UserMaintenance

        public UserIdentity SignUp(string accountName, string firstName,
            string surname, byte[] imageData,
            bool checkSubscriberRegister = true)
        {
            var current =
                DataContext.UserIdentitySet.SingleOrDefault(
                    a => a.AccountName == accountName);

            if (current != null)
                throw new SecurityException("User already exists");

            var image = new Entities.MasterData.ImageData
            {
                FileName = "default.jpg",
                FileData =
                    imageData ?? Entities.MasterData.ImageData.CreateGenericImage(ResourceManager.DefaultProfileIcon())
            };

            var user = new UserAccount
            {
                AccountName = accountName,
                FirstName = firstName,
                Surname = surname,
                Active = true,
                Registered = DateTime.UtcNow,
                ProfileImageData = image,
                ProfileComplete = true
            };

            DataContext.UserAccountSet.Add(user);

            DataContextSaveChanges();

            return user;
        }

        public UserIdentity UpdateProfilePic(byte[] imageData)
        {
            var currentUser = DataContext.UserIdentitySet.First(a => a.Id == CurrentUser.Id);
            if (currentUser.ProfileImageData != null)
                DataContext.ImageDataSet.Remove(currentUser.ProfileImageData);
            currentUser.ProfileImageData = new Entities.MasterData.ImageData
            {
                FileData = imageData,
                FileName = "newImage.jpg"
            };
            DataContextSaveChanges();
            return currentUser;
        }

        public UserIdentity SignUp(string accountName)
        {
            var current = DataContext.UserAccountSet.SingleOrDefault(a => a.AccountName == accountName);

            if (current != null)
                throw new SecurityException("User already exists");

            var image = new Entities.MasterData.ImageData
            {
                FileName = "default.jpg",
                FileData = Entities.MasterData.ImageData.CreateGenericImage(ResourceManager.DefaultProfileIcon())
            };


            var user = new UserAccount
            {
                AccountName = accountName,
                Active = true,
                Registered = DateTime.UtcNow,
                ProfileImageData = image
            };

            DataContext.UserAccountSet.Add(user);

            DataContextSaveChanges();

            return user;
        }


        public UserIdentity SaveUser(Guid id, string accountName, string firstName, string surname, 
            List<Guid> allowedRoles, bool updateRoles = true)
        {
            Authenticate(PrivilegeType.UserMaintenance);
            var user = DataContext.UserIdentitySet.SingleOrDefault(x => x.AccountName == accountName);
            if (user == null)
                user = SignUp(accountName);

            user.AccountName = accountName;
            user.FirstName = firstName;
            user.Surname = surname;
            (user as UserAccount).ProfileComplete = true;

            if (updateRoles)
            {
                var roles = DataContext.RoleSet.Where(a => allowedRoles.Contains(a.Id)).ToList();

                if ((user as UserAccount).Roles != null)
                {
                    (user as UserAccount).Roles.Clear();
                    foreach (var r in roles)

                        if (r.isActive == false)
                        {
                            throw new SecurityException("Role " + r.RoleName + " is InActive");

                        }
                        else
                        {
                            (user as UserAccount).Roles.Add(r);
                        }
                }
                else
                {
                    (user as UserAccount).Roles = roles;
                }
            }

            DataContextSaveChanges();

            return user;
        }

        public IQueryable<UserIdentity> GetUserList()
        {
            Authenticate(PrivilegeType.UserMaintenance);
            var userList = from u in DataContext.UserAccountSet.Include(a => a.PersonalInformation)
                           orderby u.Active, u.FirstName ascending
                           select u;
            return userList;
        }

        public UserIdentity EditProfile(string firstName, string surname, string email, string imageFileName,
            byte[] imageData)
        {
            var myProfile = DataContext.UserIdentitySet.Single(u => u.Id == CurrentUser.Id);

            myProfile.FirstName = firstName;
            myProfile.AccountName = email;
            myProfile.Surname = surname;

            if (!string.IsNullOrWhiteSpace(imageFileName) && imageData != null)
            {
                if (myProfile.ProfileImageData != null)
                    DataContext.ImageDataSet.Remove(myProfile.ProfileImageData);

                myProfile.ProfileImageData = new Entities.MasterData.ImageData
                {
                    FileName = imageFileName,
                    FileData = Entities.MasterData.ImageData.CreateGenericImage(imageData)
                };
                myProfile.ProfileImageDataId = myProfile.ProfileImageData.Id;
            }

            if (myProfile.ProfileImageData == null) //still no image - load a default one
            {
                myProfile.ProfileImageData = new Entities.MasterData.ImageData
                {
                    FileName = "default.jpg",
                    FileData = Entities.MasterData.ImageData.CreateGenericImage(ResourceManager.DefaultProfileIcon())
                };
            }

            (myProfile as UserAccount).ProfileComplete = true;

            DataContext.SaveChanges();

            return myProfile;
        }

        public UserIdentity GetMyProfile()
        {
            var myProfile = DataContext.UserIdentitySet.Single(u => u.Id == CurrentUser.Id);

            return myProfile;
        }

        public UserIdentity DeactivateAccount(Guid userId)
        {
            Authenticate(PrivilegeType.UserMaintenance);

            var thisUser = DataContext.UserIdentitySet.Single(u => u.Id == userId);

            thisUser.Active = false;

            DataContextSaveChanges();

            return thisUser;
        }

        public UserIdentity ActivateAccount(Guid userId)
        {
            Authenticate(PrivilegeType.UserMaintenance);

            var thisUser = DataContext.UserIdentitySet.Single(u => u.Id == userId);

            thisUser.Active = true;

            DataContextSaveChanges();

            return thisUser;
        }

        #endregion

        #region Roles

        public Role SaveRole(Guid? roleId, string name, string description, StatusType status,
            List<PrivilegeType> privileges, bool isActive)
        {
            Authenticate(PrivilegeType.RoleMaintenance);

            var roleList = privileges.ToArray();

            var role = DataContext.RoleSet.SingleOrDefault(r => r.RoleName == name);

            if (role != null && role.Id != roleId)
            {
                throw new SecurityException(name + " role already exists.");
            }
            if (roleId != null)
            {
                role = DataContext.RoleSet.Single(r => r.Id == roleId);
            }
            else
            {
                role = new Role();
                DataContext.RoleSet.Add(role);
            }

            role.Status = status;
            role.RoleName = name;
            role.Description = description;
            role.isActive = isActive;

            var security = DataContext.PrivilegeSet.Where(s => roleList.Contains(s.Security)).ToList();
            if (role.Privileges != null)
            {
                role.Privileges.Clear();
                foreach (var i in security)
                {
                    role.Privileges.Add(i);
                }
            }
            else
            {
                role.Privileges = security;
            }
            DataContextSaveChanges();

            return role;
        }

        public Role GetRole(Guid id)
        {
            Authenticate(PrivilegeType.RoleMaintenance);

            var role = DataContext.RoleSet.Single(r => r.Id == id);
            return role;
        }

        public IQueryable<Role> GetRoles()
        {
            Authenticate(PrivilegeType.RoleMaintenance);

            var objQuery = from r in DataContext.RoleSet
                           orderby r.RoleName
                           select r;
            return objQuery;
        }

        public ICollection<Role> GetUserRoles(Guid userId)
        {
            var user = DataContext.UserAccountSet.Include(a => a.Roles).Where(a => a.Id == userId).SingleOrDefault();
            if (user != null)
                return user.Roles;
            return new List<Role>(); //this user type does not have roles so return empty set.
        }

        public IQueryable<UserAccount> GetUserAccountList()
        {
            return DataContext.UserAccountSet.OrderBy(a => a.FirstName);
        }

        public List<Privilege> GetRolePrivilegeList(Guid roleId)
        {
            var role = DataContext.RoleSet.Where(r => r.Id == roleId).FirstOrDefault();
            return role.Privileges.ToList();
        }

        public IQueryable<Privilege> GetPrivilegeList()
        {
            return DataContext.PrivilegeSet.OrderBy(a => a.Description);
        }

        #endregion
    }
}