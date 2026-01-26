#region Usings

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using TCR.Lib.Utility;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.MasterData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Resources;
using TRiZHub.BL.Util;

#endregion

namespace TRiZHub.BL.Test
{
    public static class SeedData
    {
        #region Security

        public static ICurrentUser CreateAdmin(DataContext context, string emailAddress = "Admin@mail.com", string password = "password@1")
        {
            var result = CreateAdminUser(context, emailAddress, password);
            var role = CreateRole(context);
            var currentUser = result.Roles.SingleOrDefault(a => a.RoleName == role.RoleName);

            if (currentUser == null)
            {
                result.Roles.Add(role);
                context.SaveChanges();
            }

            var qPrivs = from userP in context.UserAccountSet
                from r in userP.Roles
                from privileges in r.Privileges
                where userP.Id == result.Id
                select privileges.Security;

            result.AllowedPrivileges = qPrivs.Distinct().ToList();

            var provider = new SecurityProvider(context);

            return provider.UserIdentityToCurrentUser(result);
        }

        public static ICurrentUser CreateSubscriber(DataContext context, string emailAddress = "testsubscriber@mail.com", string password = "password@1")
        {
            var subscriber = CreateSubscriberUser(context, emailAddress, password);

            var provider = new SecurityProvider(context);
            return provider.UserIdentityToCurrentUser(subscriber);
        }

        public static Role CreateRole(DataContext context, string roleName = "admin", bool hasAllPrivileged = true)
        {
            var role = context.RoleSet.Include(a => a.Privileges).SingleOrDefault(a => a.RoleName == roleName);

            if (role == null)
            {
                role = new Role
                {
                    RoleName = roleName,
                    Description = "My Role Description",
                    Privileges = new List<Privilege>()
                };
                context.RoleSet.Add(role);
            }

            if (hasAllPrivileged)
            {
                foreach (PrivilegeType p in Enum.GetValues(typeof (PrivilegeType)))
                {
                    var priv = context.PrivilegeSet.SingleOrDefault(a => a.Security == p);
                    if (priv == null)
                    {
                        priv = new Privilege
                        {
                            Description = NameSplitting.SplitCamelCase(p),
                            Security = p
                        };
                        context.PrivilegeSet.Add(priv);
                    }

                    var curr = role.Privileges.SingleOrDefault(a => a.Security == priv.Security);
                    if (curr == null)
                        role.Privileges.Add(priv);
                }
            }

            context.SaveChanges();

            return role;
        }

        public static UserAccount CreateAdminUser(DataContext context, string email = "james@jamesbond.com",
            string password = "password@1", string surname = "Bond", string firstName = "James")
        {
            try
            {
                var currentUser = context.UserAccountSet.Include(a => a.Roles).SingleOrDefault(a => a.AccountName == email);

                if (currentUser == null)
                {
                    currentUser = new UserAccount();
                    currentUser.Roles = new List<Role>();
                    context.UserAccountSet.Add(currentUser);

                    currentUser.AccountName = email;
                    currentUser.FirstName = firstName;
                    currentUser.Surname = surname;
                    currentUser.Active = true;
                    currentUser.Registered = DateTime.UtcNow;
                    currentUser.ProfileImageData = new ImageData
                    {
                        FileData = ImageData.CreateGenericImage(ResourceManager.DefaultProfileIcon()),
                        FileName = "Test"
                    };

                    context.SaveChanges();
                }

                return currentUser;
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        public static UserAccount CreateSubscriberUser(DataContext context, string email = "johan@greef.co.za",
            string password = "password@1", string surname = "Greef", string firstName = "Johan")
        {
            try
            {
                var subscriberUser = context.UserAccountSet.SingleOrDefault(a => a.AccountName == email);
     

                if (subscriberUser == null)
                {
                    subscriberUser = new UserAccount();
                    subscriberUser.AccountName = email;
                    subscriberUser.FirstName = firstName;
                    subscriberUser.Surname = surname;
                    subscriberUser.Active = true;
                    subscriberUser.Registered = DateTime.UtcNow;
                    subscriberUser.ProfileComplete = true;

                    subscriberUser.ProfileImageData = new ImageData
                    {
                        FileData = ImageData.CreateGenericImage(ResourceManager.DefaultProfileIcon()),
                        FileName = "Test"
                    };

                    context.UserAccountSet.Add(subscriberUser);

                    context.SaveChanges();
                }

                return subscriberUser;
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        public static List<Privilege> GetPrivileges(DataContext context)
        {
            var privileges = context.PrivilegeSet.OrderBy(p => p.Id).Take(2);

            return privileges.ToList();
        }

        public static List<Guid> GetRoles(DataContext context)
        {
            var roles = context.RoleSet.OrderBy(r => r.Id).Select(s => s.Id).Take(2);

            return roles.ToList();
        }

        #endregion
    }
}