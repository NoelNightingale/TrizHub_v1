#region Usings

using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using TCR.Lib.BL;
using TCR.Lib.Utility;
using TRiZHub.BL.Entities.ActivityData;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.ContactData;
using TRiZHub.BL.Entities.EmailData;
using TRiZHub.BL.Entities.Logging;
using TRiZHub.BL.Entities.MasterData;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.ScorecardData;
using TRiZHub.BL.Entities.ScorecardTemplateData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.SettingsData;
using TRiZHub.BL.Entities.TeamData;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.BillingCycleData;
using TRiZHub.BL.Entities.BillingRatesData;
using TRiZHub.BL.Entities.OfficeEquipmentData;
using TRiZHub.BL.Entities.TravelInformationData;
using TRiZHub.BL.Entities.PersonalInformationData;
using TRiZHub.BL.Entities.TeamJobDesignationData;
using TRiZHub.BL.Entities.ClientReporterData;
using TRiZHub.BL.Entities.UserIdentityProject;
using TRiZHub.BL.Entities.UserIdentityClient;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using TCR.Lib.SQL;
using TRiZHub.BL.Scripts.TimesheetReportProcedure;
using TRiZHub.BL.Scripts.ScorecardProcedure;
using TRiZHub.BL.Entities.EmployerData;
using System.Data.Entity.Migrations;

#endregion Usings

// enable-migrations
// Add-Migration {NameOfMigration} -verbose
// Update-Database
// Update-Database  -TargetMigration {NameOfMigration}

namespace TRiZHub.BL.Context
{
    public class DataContext : DbContext, IAuditDBContext<PrivilegeType>
    {
        public DataContext()
            : base("DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        public DataContext(DbConnection connection)
            : base(connection, true)
        {
        }

        public DbSet<Role> RoleSet { get; set; }
        public DbSet<Privilege> PrivilegeSet { get; set; }
        public DbSet<UserIdentity> UserIdentitySet { get; set; }
        public DbSet<UserAccount> UserAccountSet { get; set; }
        public DbSet<SystemLog> SystemLogSet { get; set; }
        public DbSet<AuditLog> AuditLogSet { get; set; }
        public DbSet<SystemParameter> SystemParameterSet { get; set; }
        public DbSet<ImageData> ImageDataSet { get; set; }
        public DbSet<EmailQueue> EmailQueueSet { get; set; }
        public DbSet<EmergancyContact> EmergancyContactSet { get; set; }
        public DbSet<TravelInformation> TravelInformationSet { get; set; }
        public DbSet<TeamJobDesignation> TeamJobDesignationSet { get; set; }
        


        // Clients
        public DbSet<ClientEntity> ClientEntitySet { get; set; }

        public DbSet<ClientReporter> ClientReporterSet { get; set; }

        // Project
        public DbSet<Project> ProjectSet { get; set; }

        public DbSet<SubProject> SubProjectSet { get; set; }

        public DbSet<Activity> ActivitySet { get; set; }
        public DbSet<Team> TeamSet { get; set; }

        public DbSet<ProjectType> ProjectTypeSet { get; set; }
        public DbSet<UserIdentityProject> UserIdentityProjectSet { get; set; }
        
        public DbSet<UserIdentityClient> UserIdentityClientSet { get; set; }

        // Timesheet
        public DbSet<TimesheetEntry> TimesheetEntrySet { get; set; }
        public DbSet<TimesheetTemplate> TimesheetTemplateSet { get; set; }
        public DbSet<TimesheetTemplateItem> TimesheetTemplateItemSet { get; set; }

        // Score Card Template
        public DbSet<ScorecardTemplate> ScorecardTemplateSet { get; set; }

        public DbSet<ScorecardTemplateItem> ScorecardTemplateItemSet { get; set; }

        //public DbSet<ScorecardTemplateItemScore> ScorecardTemplateItemScoreSet { get; set; }
        public DbSet<ScorecardTemplatePeriod> ScorecardTemplatePeriodSet { get; set; }

        // Score Card
        public DbSet<Scorecard> ScorecardSet { get; set; }

        //public DbSet<ScorecardPeriod> ScorecardPeriodSet { get; set; }
        public DbSet<ScorecardRecord> ScorecardRecordSet { get; set; }

        //Billing Cycle
        public DbSet<BillingCycleEntry> BillingCycleEntrySet { get; set; }

        //Billing Rates
        public DbSet<BillingRates> BillingRatesSet { get; set; }

        //Office Equipment
        public DbSet<OfficeEquipment> OfficeEquipmentSet { get; set; }

        // Personal Information
        public DbSet<PersonalInformation> PersonalInformationSet { get; set; }

        // Employer
        public DbSet<Employer> EmployerSet { get; set; }

        public int SaveChanges(IUserContext<PrivilegeType> currentUser)
        {
            if (currentUser != null)
                return AuditHandler.SaveChanges(this, AuditLogSet, currentUser.UserName, currentUser.Id);
            return base.SaveChanges();
        }

        public void AddSystemLogEntry(object sender, Guid guid, Guid? currentUserId, LogEventType logEventType,
            string message, string stackTrace = null, string innerExceptionMessage = null,
            string innerExceptionStackTrace = null)
        {
            try
            {
                //create a fresh context because the current context is in an error state
                using (var dbContext = new DataContext())
                {
                    var syslogEntry = new SystemLog();
                    dbContext.SystemLogSet.Add(syslogEntry);

                    syslogEntry.EventTime = DateTime.UtcNow;
                    syslogEntry.Sender = sender.ToString();
                    syslogEntry.Id = guid;
                    syslogEntry.UserIdentityId = currentUserId;
                    syslogEntry.EventType = logEventType;
                    syslogEntry.Message = message;
                    syslogEntry.StackTrace = stackTrace;
                    syslogEntry.InnerException = innerExceptionMessage;
                    syslogEntry.InnerExceptionStackTrace = innerExceptionStackTrace;

                    dbContext.SaveChanges();
                }
            }
            catch
            {
                //empty exception never allowed!
                //in this case we are unable to hit the db to log an exception
                //therefore we do not want to change the actual error that happend in the handler

                //perform housekeeping on the systemlog
                using (var dbContext = new DataContext())
                {
                    dbContext.Database.ExecuteSqlCommand("delete from SystemLogs where EventTime < GetDate() - 100");
                }
            }
        }

        public static void Setup()
        {
            var connection = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            using (DbConnection setupCon = new SqlConnection(connection.ConnectionString))
            {
                Setup(setupCon);
            }
        }

        public static void Setup(DbConnection dbConnection, bool initDefaultData = true, bool useMigrations = true)
        {
            //if (useMigrations)
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<DataContext, Migrations.Configuration>());
            //else
            //    Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DataContext>());
            using (var context = new DataContext(dbConnection))
            {
                try
                {
                    context.Database.Initialize(true);
                }
                catch (InvalidOperationException ex)
                {
                    throw ex;
                }
                catch (ModelValidationException ex)
                {
                    throw ex;
                }

                SystemParameter.LoadDefault(context);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Role>()
                .HasMany(x => x.Privileges)
                .WithMany(a => a.Roles)
                .Map(x =>
                {
                    x.ToTable("RolePrivilege");
                    x.MapLeftKey("RoleId");
                    x.MapRightKey("PrivilegeId");
                });

            modelBuilder.Entity<UserAccount>()
                .HasMany(x => x.Roles)
                .WithMany(a => a.AdminUsers)
                .Map(x =>
                {
                    x.ToTable("AdminUserRole");
                    x.MapLeftKey("AdminUserId");
                    x.MapRightKey("RoleId");
                });
        }

        #region House Keeping / Maintenenace

        public void EmailHouseKeeping()
        {
            //Delete history
            Database.ExecuteSqlCommand("delete from EmailQueue where Processed <= (GetDate() - 100)");
            //Retry failed emails
            Database.ExecuteSqlCommand(
                "update EmailQueue set Status = 0 where Status = 3 and Processed < DATEADD(minute, -5, GetDate()) and SendAttempts < 3");
            //Reshedule processing emails
            Database.ExecuteSqlCommand(
                "update EmailQueue set Status = 0 where Status = 1 and Processed < DATEADD(minute, -10, GetDate())");
        }

        public List<BillableHoursReportModel> ExecuteBillableHoursProcedure(DateTime startDateTime,
                                                             DateTime endDateTime,
                                                             String projectID)
        {
            //employers = employers.Replace(",", "','");

            //if (employers != "All") employers = "'" + employers + "'";

            var query =
                Database.SqlQuery<BillableHoursReportModel>("exec lsp_getProjectBillableHours @StartDate,@EndDate,@projectID",
                new SqlParameter("@StartDate", startDateTime),
                new SqlParameter("@EndDate", endDateTime),
                new SqlParameter("@projectID", projectID)
                //new SqlParameter("@employerIDs", employers)
                );
            Database.CommandTimeout = 600;

            return query.ToList();
        }

        public List<TimesheetReportDetailModel> ExecuteTimesheetDetailProcedure(DateTime startDateTime,
                                                                    DateTime endDateTime,
                                                                    String userAccountIDs,
                                                                    String clientAccountIDs,
                                                                    String projectIDs,
                                                                    bool billable)
        {
            clientAccountIDs = clientAccountIDs.Replace(",", "','");
            projectIDs = projectIDs.Replace(",", "','");
            userAccountIDs = userAccountIDs.Replace(",", "','");

            if (clientAccountIDs != "All") clientAccountIDs = "'" + clientAccountIDs + "'";
            if (projectIDs != "All") projectIDs = "'" + projectIDs + "'";
            if (userAccountIDs != "All") userAccountIDs = "'" + userAccountIDs + "'";

            var query =
                Database.SqlQuery<TimesheetReportDetailModel>("exec lsp_getTimesheetReportDetail @StartDate,@EndDate,@userAccountIDs,@clientsAccountID,@projectIDs,@billable",
                new SqlParameter("@StartDate", startDateTime),
                new SqlParameter("@EndDate", endDateTime),
                new SqlParameter("@userAccountIDs", userAccountIDs),
                new SqlParameter("@clientsAccountID", clientAccountIDs),
                new SqlParameter("@projectIDs", projectIDs),
                new SqlParameter("@billable", billable)
                );
            Database.CommandTimeout = 600;

            return query.ToList();
        }

        public List<TimesheetReportProcedureModel> ExecuteTimesheetReportProcedure(DateTime startDateTime,
                                                                    DateTime endDateTime,
                                                                    bool showPhases,
                                                                    bool showOnlyBillbale,
                                                                    String userAccountIDs,
                                                                    String clientAccountIDs,
                                                                    String projectIDs,
                                                                    String projectWildCardSearch)
        {
            clientAccountIDs = clientAccountIDs.Replace(",", "','");
            projectIDs = projectIDs.Replace(",", "','");
            userAccountIDs = userAccountIDs.Replace(",", "','");

            if (clientAccountIDs != "All") clientAccountIDs = "'" + clientAccountIDs + "'";
            if (projectIDs != "All") projectIDs = "'" + projectIDs + "'";
            if (userAccountIDs != "All") userAccountIDs = "'" + userAccountIDs + "'";

            var query =
                Database.SqlQuery<TimesheetReportProcedureModel>("exec lsp_getTimesheetReportData @StartDate,@EndDate,@ShowPhases,@OnlyBillable,@userAccountIDs,@clientsAccountID,@projectIDs,@projectWildCardSearch",
                new SqlParameter("@StartDate", startDateTime),
                new SqlParameter("@EndDate", endDateTime),
                new SqlParameter("@ShowPhases", showPhases),
                new SqlParameter("@OnlyBillable", showOnlyBillbale),
                new SqlParameter("@userAccountIDs", userAccountIDs),
                new SqlParameter("@clientsAccountID", clientAccountIDs),
                new SqlParameter("@projectIDs", projectIDs),
                new SqlParameter("@projectWildCardSearch", projectWildCardSearch)
            );
            Database.CommandTimeout = 600;
            return query.ToList();
        }

        public List<ScoreCardSummaryModel> ExecuteScorecardSummaryProcedure(string reviewYears,
            string reviewPeriods, int submitted, int locked,
            int employeeHasScorecard, string employees, string clients, string lineManagers,
            string evaluators, string scorecards)
        {
            reviewPeriods = reviewPeriods.Replace(",", "','");
            employees = employees.Replace(",", "','");
            clients = clients.Replace(",", "','");
            lineManagers = lineManagers.Replace(",", "','");
            evaluators = evaluators.Replace(",", "','");
            scorecards = scorecards.Replace(",", "','");

            if (reviewPeriods != "All") reviewPeriods = "'" + reviewPeriods + "'";
            if (employees != "All") employees = "'" + employees + "'";
            if (clients != "All") clients = "'" + clients + "'";
            if (lineManagers != "All") lineManagers = "'" + lineManagers + "'";
            if (evaluators != "All") evaluators = "'" + evaluators + "'";
            if (scorecards != "All") scorecards = "'" + scorecards + "'";

            var query =
                Database.SqlQuery<ScoreCardSummaryModel>("exec lsp_getScoreCardSummary @reviewYears, @reviewPeriods,@submitted,@locked,@employeeHasScorecard,@employees,@clientsAccountID,@lineManagers,@evaluators,@scorecards",
                new SqlParameter("@reviewYears", reviewYears),
                new SqlParameter("@reviewPeriods", reviewPeriods),
                new SqlParameter("@submitted", submitted),
                new SqlParameter("@locked", locked),
                new SqlParameter("@employeeHasScorecard", employeeHasScorecard),
                new SqlParameter("@employees", employees),
                new SqlParameter("@clientsAccountID", clients),
                new SqlParameter("@lineManagers", lineManagers),
                new SqlParameter("@evaluators", evaluators),
                new SqlParameter("@scorecards", scorecards)
            );
            Database.CommandTimeout = 600;

            return query.ToList();
        }

        #endregion House Keeping / Maintenenace

        #region Transaction

        public DbContextTransaction ContextTransaction { get; set; }

        public void BeginTransaction()
        {
            if (ContextTransaction == null)
                ContextTransaction = Database.BeginTransaction();
            else
                throw new GenericSecurityException("Another Transaction was already created");
        }

        public void CommitTransaction()
        {
            if (IsTransactionActive())
            {
                try
                {
                    ContextTransaction.Commit();
                    ContextTransaction.Dispose();
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }
                finally
                {
                    ContextTransaction.Dispose();
                    ContextTransaction = null;
                }
            }
            else
            {
                throw new GenericSecurityException("No transaction is started...");
            }
        }

        public bool IsTransactionActive()
        {
            return ContextTransaction != null;
        }

        #endregion Transaction
    }
}