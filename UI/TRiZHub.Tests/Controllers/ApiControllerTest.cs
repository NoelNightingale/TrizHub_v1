#region Usings

using System.Web.Http;
using TRiZHub.BL.Test.DataConnections;
using TRiZHub.BL.Test.Mocs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Tests.Utility;

#endregion

namespace TRiZHub.Tests.Controllers
{
    public class ApiControllerTest<T> where T : ApiController
    {
        private IAppSettings _AppSettings;
        private ITestDataConnection _TestDataConnection;

        #region Setup

        [TestInitialize]
        public void TestInitialize()
        {
            _AppSettings = new FakeAppSettings();
            _TestDataConnection = new SQLDataConnection();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _TestDataConnection.Dispose();
        }

        protected DataContext DataContext
        {
            get { return _TestDataConnection.Context; }
        }

        protected T CreateController(ICurrentUser user = null)
        {
            var parms = new object[]
            {
                _AppSettings,
                DataContext,
                user
            };
            return Moqer.CreateAPIController<T>(parms);
        }

        #endregion
    }
}