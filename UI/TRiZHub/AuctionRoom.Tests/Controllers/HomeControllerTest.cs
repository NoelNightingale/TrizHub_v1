#region Usings

using System.Diagnostics.CodeAnalysis;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Test;
using TRiZHub.BL.Test.DataConnections;
using TRiZHub.BL.Test.Mocs;
using TRiZHub.Controllers;
using TRiZHub.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;

#endregion

namespace TRiZHub.Tests.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HomeControllerTest
    {
        private IAppSettings _AppSettings;
        private ITestDataConnection _TestDataConnection;
        private ICurrentUser _TestUser;

        [TestMethod]
        [TestCategory("Controller.Home")]
        public void Index()
        {
            using (var controller = CreateController())
            {
                var result = controller.Index();
                result.ShouldNotBeNull();
            }
        }

        #region Setup

        [TestInitialize]
        public void TestInitialize()
        {
            _AppSettings = new FakeAppSettings();
            _TestDataConnection = new SQLDataConnection();
            _TestUser = SeedData.CreateAdmin(_TestDataConnection.Context, "testuser@mail.com", "password@1");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _TestDataConnection.Dispose();
        }

        private HomeController CreateController()
        {
            var parms = new object[]
            {
                _AppSettings
            };
            return Moqer.CreateController<HomeController>(parms, _TestUser.UserName);
        }

        #endregion
    }
}