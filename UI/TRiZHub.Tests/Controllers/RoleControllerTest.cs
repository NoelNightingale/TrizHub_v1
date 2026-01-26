#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TRiZHub.BL.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TRiZHub.BL.Entities.Types;
using TRiZHub.Controllers.Security;
using TRiZHub.Models;
using TRiZHub.Models.SecurityData;

#endregion

namespace TRiZHub.Tests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RoleControllerTest : ApiControllerTest<RoleController>
    {
        [TestMethod]
        [TestCategory("Controller.Role")]
        public void SaveRole()
        {
            var adminUser = SeedData.CreateAdmin(DataContext, "jan@mail.com", "123123123");
            using (var controller = CreateController(adminUser))
            {
                var result = controller.RoleSave(new RoleViewModel
                {
                    Description = "New test role",
                    RoleName = "New Test role",
                    StatusType = StatusType.Active,
                    Permissions = new List<PermissionViewModel>
                    {
                        new PermissionViewModel
                        {
                            Privilege = PrivilegeType.RoleMaintenance,
                            Selected = true
                        },
                        new PermissionViewModel
                        {
                            Privilege = PrivilegeType.UserMaintenance,
                            Selected = false
                        }
                    }
                });

                result.Id.ShouldNotBeNull();
                result.Permissions.Count().ShouldEqual(1);
            }
        }

        [TestMethod]
        [TestCategory("Controller.Role")]
        public void RoleList()
        {
            var adminUser = SeedData.CreateAdmin(DataContext, "jan@mail.com", "123123123");
            var testRole = SeedData.CreateRole(DataContext, "test", true);
            var testRole2 = SeedData.CreateRole(DataContext, "test2", true);
            using (var controller = CreateController(adminUser))
            {
                var roleList = controller.RoleList();
                roleList.Count.ShouldEqual(DataContext.RoleSet.Count());
            }
        }

        [TestMethod]
        [TestCategory("Controller.Role")]
        public void RoleGrid()
        {
            var adminUser = SeedData.CreateAdmin(DataContext, "jan@mail.com", "123123123");
            var testRole = SeedData.CreateRole(DataContext, "test", true);
            var testRole2 = SeedData.CreateRole(DataContext, "test2", true);
            using (var controller = CreateController(adminUser))
            {
                var roleList = controller.RoleGrid(new GridModel
                {
                    CurrentPage = 0,
                    RecordsPerPage = 500
                });

                roleList.RecordCount.ShouldEqual(DataContext.RoleSet.Count());
            }
        }

        [TestMethod]
        [TestCategory("Controller.Role")]
        public void RoleGet()
        {
            var adminUser = SeedData.CreateAdmin(DataContext, "jan@mail.com", "123123123");
            var testRole = SeedData.CreateRole(DataContext, "test", true);
            using (var controller = CreateController(adminUser))
            {
                var result = controller.RoleGet(testRole.Id);
                result.Id.ShouldEqual(testRole.Id);
            }
        }
    }
}