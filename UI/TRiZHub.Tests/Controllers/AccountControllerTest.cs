#region Usings

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TRiZHub.BL.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TRiZHub.Controllers.Security;

#endregion

namespace TRiZHub.Tests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AccountControllerTest : ApiControllerTest<AccountController>
    {
        [TestMethod]
        [TestCategory("Controller.Account")]
        public void GetCurrentUser()
        {
            var adminUser = SeedData.CreateAdmin(DataContext);

            using (var controller = CreateController(adminUser))
            {
                var result = controller.GetCurrentUser();
                result.Id.ShouldEqual(adminUser.Id);
            }
        }

        [TestMethod]
        [TestCategory("Controller.Account")]
        public void GetMyProfile()
        {
            var adminUser = SeedData.CreateAdmin(DataContext, "jan@mail.com", "123123123");
            using (var controller = CreateController(adminUser))
            {
                var result = controller.GetMyProfile();
                var checkUser = DataContext.UserIdentitySet.Single(a => a.Id == adminUser.Id);
                checkUser.AccountName.ShouldEqual(result.EmailAddress);
                checkUser.FirstName.ShouldEqual(result.FirstName);
            }
        }
    }
}