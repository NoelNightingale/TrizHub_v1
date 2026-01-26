#region Usings

using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRiZHub.BL.Test.DataConnections;
using TRiZHub.BL.Test.Mocs;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Email;
using TRiZHub.BL.Provider.Settings;

#endregion

namespace TRiZHub.BL.Test
{
    public abstract class ProviderTestBase
    {
        private ITestDataConnection _testDataConnection;

        protected IAppSettings AppSettings
        {
            get { return new FakeAppSettings(); }
        }

        protected IEmailProvider EmailProvider
        {
            get { return new FakeEmailProvider(); }
        }

        protected DataContext Context
        {
            get { return _testDataConnection.Context; }
        }

        public virtual bool TearDownDb
        {
            get { return true; }
        }

        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public string ReportFolder
        {
            get
            {
                var x = AssemblyDirectory;
                if (!x.EndsWith("\\"))
                    x = x + "\\";
                return x + @"Report\";
            }
        }

        public IAppSettings FakeSettings
        {
            get { return new FakeAppSettings(); }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _testDataConnection = new SQLDataConnection(TearDownDb);
            InitTest();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //_testDataConnection.TearDownDatabase();
            _testDataConnection.Dispose();
        }

        protected virtual void InitTest()
        {
        }
    }
}