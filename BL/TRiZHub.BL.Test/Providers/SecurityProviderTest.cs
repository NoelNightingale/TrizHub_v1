#region Usings

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Test.Providers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SecurityProviderTest : ProviderTestBase
    {
        [TestMethod]
        [TestCategory("Provider.Security")]
        public void UserLogin()
        {
            ISecurityProvider provider = new SecurityProvider(Context);
            var testUser = SeedData.CreateAdminUser(Context, "testUser");

            var user = provider.UserLogin(testUser.AccountName);

            user.ShouldNotBeNull();
            user.UserName.ShouldEqual(testUser.AccountName);
            user.AllowedPrivileges.ShouldNotBeNull();
            provider.CurrentUser.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("Provider.Security")]
        public void UserLoginFailed()
        {
            ISecurityProvider provider = new SecurityProvider(Context);

            var testUser = SeedData.CreateAdminUser(Context, "test@mail.com", "password@1", "Bond", "James");

            var user = provider.UserLogin(testUser.AccountName);
            user.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("Provider.Security")]
        public void SignUp()
        {
            ISecurityProvider provider = new SecurityProvider(Context);
      
            var user = provider.SignUp("test@mail.com");

            user.ShouldNotBeNull();
            user.Id.ShouldNotBeNull();
        }

        //[TestMethod]
        //[TestCategory("Provider.Security")]
        //public void SaveRole()
        //{
        //    var role = SeedData.CreateAdmin(Context);
        //    ISecurityProvider provider = new SecurityProvider(Context, role);
        //    var privileges = SeedData.GetPrivileges(Context).Select(a => a.Security).ToList();
        //    var createRole = provider.SaveRole(null, "James", "MVP-role test", 0, privileges);

        //    createRole.RoleName.ShouldEqual("James");
        //    createRole.Privileges.Count().ShouldBeGreaterThan(0);
        //}

        //[TestMethod]
        //[TestCategory("Provider.Security")]
        //public void UpdateRole()
        //{
        //    var role = SeedData.CreateAdmin(Context);
        //    ISecurityProvider provider = new SecurityProvider(Context, role);
        //    var privileges = SeedData.GetPrivileges(Context).Select(a => a.Security).ToList();
        //    var createRole = provider.SaveRole(null, "James", "MVP-role test", 0, privileges);

        //    createRole.RoleName.ShouldEqual("James");
        //    createRole.Privileges.Count().ShouldBeGreaterThan(0);

        //    var updateExisting = provider.SaveRole(createRole.Id, "Test2", "MVP-role test", 0, privileges);
        //    updateExisting.RoleName.ShouldEqual("Test2");
        //    updateExisting.Privileges.Count().ShouldBeGreaterThan(0);
        //}

        [TestMethod]
        [TestCategory("Provider.Security")]
        public void GetRole()
        {
            var admin = SeedData.CreateAdmin(Context);
            ISecurityProvider provider = new SecurityProvider(Context, admin);
            var role = SeedData.CreateRole(Context, "Code Auditor");

            var getRole = provider.GetRole(role.Id);

            getRole.Id.ShouldNotBeNull();
            getRole.RoleName.ShouldEqual("Code Auditor");
        }

        [TestMethod]
        [TestCategory("Provider.Security")]
        public void GetRoles()
        {
            var admin = SeedData.CreateAdmin(Context);
            ISecurityProvider provider = new SecurityProvider(Context, admin);
            var role = SeedData.CreateRole(Context, "Code Auditor");

            var getRole = provider.GetRoles().Count();

            getRole.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        [TestCategory("Provider.Security")]
        public void LockUnlockProviderAccount()
        {
            var user = SeedData.CreateAdmin(Context);
         
            ISecurityProvider provider = new SecurityProvider(Context, user);

            var newUser = provider.SignUp("test@mail.com", "James", "Bond", null);
            newUser.Active.ShouldBeTrue();

            var updated = provider.DeactivateAccount(newUser.Id);
            updated.Active.ShouldBeFalse();

            updated = provider.ActivateAccount(updated.Id);
            updated.Active.ShouldBeTrue();
        }
    }
}