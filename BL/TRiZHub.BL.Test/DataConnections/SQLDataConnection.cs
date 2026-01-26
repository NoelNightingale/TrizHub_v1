#region Usings

using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using TRiZHub.BL.Context;

#endregion

namespace TRiZHub.BL.Test.DataConnections
{
    [ExcludeFromCodeCoverage]
    public class SQLDataConnection : ITestDataConnection
    {
        private string dbRandom = "";

        public SQLDataConnection(bool teardownDatabase = true)
        {
            var ensureDllIsCopied = SqlProviderServices.Instance;

            dbRandom = Environment.TickCount.ToString();

            SetupNewDB(teardownDatabase);

            DataBase = new SqlConnection(DbConnectionString);
            Context = new DataContext(DataBase);
        }

        private void SetupNewDB(bool teardownDatabase)
        {
            if (teardownDatabase)
                TearDownDatabase();
            //pooling=true;max pool size=1;
            DbConnectionString =
                string.Format(
                    "Data Source={0};Initial Catalog={1};Persist Security Info=False;User ID={2};Password={3};{4}",
                    SqlServerName,
                    DatabaseName,
                    TestUserName,
                    TestPassword,
                    "Connection Timeout=180");
            using (DbConnection setupCon = new SqlConnection(DbConnectionString))
            {
                DataContext.Setup(setupCon, false,false);
                setupCon.Close();
            }
            SqlConnection.ClearAllPools();
        }

        private string DatabaseName
        {
            get
            {
                var conString = GetConnectionString();
                var builder = new SqlConnectionStringBuilder(conString);
                return builder.InitialCatalog;// + dbRandom;
            }
        }

        private string SqlServerName
        {
            get
            {
                var conString = GetConnectionString();
                var builder = new SqlConnectionStringBuilder(conString);
                return builder.DataSource;
            }
        }

        private string TestUserName
        {
            get
            {
                var conString = GetConnectionString();
                var builder = new SqlConnectionStringBuilder(conString);
                return builder.UserID;
            }
        }

        private string TestPassword
        {
            get
            {
                var conString = GetConnectionString();
                var builder = new SqlConnectionStringBuilder(conString);
                return builder.Password;
            }
        }

        public DbConnection DataBase { get; private set; }

        public string SQLServerName
        {
            get { return SqlServerName; }
        }

        public string DbConnectionString { get; private set; } = "";

        public void Dispose()
        {
            Context.Dispose();
            DataBase.Close();
            DataBase.Dispose();
            SqlConnection.ClearAllPools();
        //    TearDownDatabase();
        }

        public DataContext Context { get; set; }

        public void TearDownDatabase()
        {
            var conStr =
                string.Format(
                    "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}",
                    SQLServerName, "master",
                    TestUserName,
                    TestPassword);
            using (DbConnection setupCon = new SqlConnection(conStr))
            {
                setupCon.Open();
                //try force single connection mode
                try
                {
                    using (var dbCommand = setupCon.CreateCommand())
                    {
                        dbCommand.CommandText = "ALTER DATABASE [" + DatabaseName +
                                                "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                        dbCommand.ExecuteNonQuery();
                    }
                }
                catch
                {
                }

                try
                {
                    using (var dbCommand = setupCon.CreateCommand())
                    {
                        dbCommand.CommandText = "USE master DROP DATABASE [" + DatabaseName + "]";
                        dbCommand.ExecuteNonQuery();
                    }
                }
                catch
                {
                }

                try
                {
                    using (var dbCommand = setupCon.CreateCommand())
                    {
                        dbCommand.CommandText = "USE master CREATE DATABASE [" + DatabaseName + "]";
                        dbCommand.ExecuteNonQuery();
                    }
                }
                catch
                {
                }
            }
        }

        private static string GetConnectionString()
        {
            var conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            var conString = conStr == null
                ? @"Data Source=.;Initial Catalog=TRiZHub_TEST;Persist Security Info=True;User ID=TRiZHub;Password=TRiZHub@1"
                : conStr.ConnectionString;
            return conString;
        }
    }
}