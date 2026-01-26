#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TRiZHub.BL.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TRiZHub.Controllers.Security;
using TRiZHub.Models;
using TRiZHub.Models.SecurityData;

#endregion

namespace TRiZHub.Tests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UserControllerTest : ApiControllerTest<UserController>
    {
        [TestMethod]
        [TestCategory("Controller.User")]
        public void UserSave()
        {
            var adminUser = SeedData.CreateAdmin(DataContext, "jan@mail.com", "123123123");
            var aRole = SeedData.CreateRole(DataContext, "test");
            var testUser = SeedData.CreateAdminUser(DataContext, "test@tester.com");

            using (var controller = CreateController(adminUser))
            {
                var result = controller.UserSave(new UserEditModel
                {
                    Id = testUser.Id,
                    Account = "test@tester.com",
                    FirstName = "bob",
                    RoleList = new List<UserRoleModel>
                    {
                        new UserRoleModel
                        {
                            RoleId = aRole.Id,
                            RoleName = aRole.RoleName,
                            Selected = true
                        }
                    }
                });

                result.Id.ShouldNotBeNull();
                result.Id.ShouldEqual(testUser.Id);
            }
        }

        [TestMethod]
        [TestCategory("Controller.User")]
        public void UserGrid()
        {
            var adminUser = SeedData.CreateAdmin(DataContext, "jan@mail.com", "123123123");
            var testUser = SeedData.CreateAdminUser(DataContext, "test@tester.com");
            using (var controller = CreateController(adminUser))
            {
                var roleList = controller.UserGrid(new GridModel
                {
                    CurrentPage = 0,
                    RecordsPerPage = 500
                });

                roleList.RecordCount.ShouldEqual(DataContext.UserIdentitySet.Count());
            }
        }

        [TestMethod]
        [TestCategory("Controller.User")]
        public void UserGet()
        {
            var adminUser = SeedData.CreateAdmin(DataContext, "jan@mail.com", "123123123");
            var testUser = SeedData.CreateAdminUser(DataContext, "test@tester.com");
            using (var controller = CreateController(adminUser))
            {
                var result = controller.UserGet(testUser.Id);
                result.Id.ShouldEqual(testUser.Id);
            }
        }
    }
}