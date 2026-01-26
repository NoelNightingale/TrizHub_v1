#region Usings

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.MasterData;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.BillingCycleData;

#endregion

namespace TRiZHub.DbPopulate.DbGen
{
    internal class TRiZHubDummyDbGenerator : IDisposable
    {
        private static TRiZHubDummyDbGenerator _instance;
        private DataContext Db { get; set; }

        public void Dispose()
        {
            if (Db == null)
                return;
            Db.Dispose();
            Db = null;
        }

        public static TRiZHubDummyDbGenerator Instance()
        {
            return _instance ?? (_instance = new TRiZHubDummyDbGenerator());
        }

        private void SetupDatabase(DataContext db)
        {
            ShowMessage("Starting Database Reload...");
            ShowMessage("");
            if (db.Database.Exists())
            {
                ShowMessage("Deleting Database...");
                Database.Delete(db.Database.Connection);
                // TODO: Implement action to close all existing connections...
                // set the database to SINGLE_USER so it can be dropped
                /*db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
                    "ALTER DATABASE [" + db.Database.Connection.Database +
                    "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                // drop the database
                db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
                    "USE master DROP DATABASE [" + db.Database.Connection.Database + "]");*/
            }
            ShowMessage("Creating Database...");
            DataContext.Setup();
            ShowMessage("Done...");
            ShowMessage("---");
            Db = db;
        }

        public void CreateDatabase(DataContext db)
        {
            SetupDatabase(db);
            // Finalize
            ShowMessage("---");
            ShowMessage("Finished Creating Database...");
        }

        public void CreateDummyDatabase(DataContext db)
        {
            CreateDatabase(db);

            // Create Objects into Database
            CreateRoles();
            CreateUsers();
            //CreateCustomers();
            //CreateProjects();
            //CreateSubProjects();
            //CreateTimesheetInfo();
            //CreateBillingCycle();
            //CreateScorecardTempaltes();
            //CreateScorecardTempaltePeriods();
            //CreateScorecardTempalteItems();
            //CreateScorecardTempalteItemScores();

            // Finalize
            ShowMessage("---");
            ShowMessage("Finished setting up the Database...");
            db.Dispose();
            Thread.Sleep(1000);
        }

        public void CreateRoles()
        {
            ShowMessage("CreateRoles...");

            Db.RoleSet.Add(new Role
            {
                RoleName = "Admin",
                Description = "Administrator Roles",
                Privileges = Db.PrivilegeSet.ToList(),
                Status = StatusType.Active
            });

            Db.RoleSet.Add(new Role
            {
                RoleName = "User Management",
                Description = "User control",
                Privileges = new List<Privilege>
                {
                    Db.PrivilegeSet.First(a => a.Security == PrivilegeType.UserMaintenance),
                    Db.PrivilegeSet.First(a => a.Security == PrivilegeType.RoleMaintenance)
                },
                Status = StatusType.Active
            });

            Db.SaveChanges();
        }

        public void CreateUsers()
        {
            ShowMessage("CreateUsers...");

            var s7fsIcon = new ImageData
            {
                FileName = "default.jpg",
                FileData = ImageData.CreateGenericImage(File.ReadAllBytes(@"Images\7FsIcon.jpg"))
            };
            Db.ImageDataSet.Add(s7fsIcon);

            var raezorIcon = new ImageData
            {
                FileName = "raezor.jpg",
                FileData = ImageData.CreateGenericImage(File.ReadAllBytes(@"Images\RAEZOR.jpg"))
            };
            Db.ImageDataSet.Add(raezorIcon);

            Db.UserAccountSet.Add(new UserAccount
            {
                AccountName = @"7FS\user1",
                Active = true,
                FirstName = "Andrew",
                IsSystemAdmin = true,
                ProfileComplete = true,
                Registered = DateTime.UtcNow,
                Surname = "Scott",
                ProfileImageData = s7fsIcon
            });

            Db.UserAccountSet.Add(new UserAccount
            {
                AccountName = @"7FS\franche",
                Active = true,
                FirstName = "Admin",
                IsSystemAdmin = true,
                Registered = DateTime.UtcNow,
                ProfileImageData = raezorIcon
            });

            Db.UserAccountSet.Add(new UserAccount
            {
                AccountName = @"RAEZOR_W10\CA2",
                Active = true,
                FirstName = "John",
                IsSystemAdmin = true,
                ProfileComplete = true,
                Registered = DateTime.UtcNow,
                Surname = "Snow",
                ProfileImageData = raezorIcon
            });

            Db.UserAccountSet.Add(new UserAccount
            {
                AccountName = @"AzureAD\RAEZOR",
                Active = true,
                FirstName = "Franche",
                IsSystemAdmin = true,
                ProfileComplete = true,
                Registered = DateTime.UtcNow,
                Surname = "van den Berg",
                ProfileImageData = raezorIcon
            });

            Db.UserAccountSet.Add(new UserAccount
            {
                AccountName = @"DESKTOP-2SP9CAC\George",
                Active = true,
                FirstName = "Tian",
                IsSystemAdmin = true,
                ProfileComplete = true,
                Registered = DateTime.UtcNow,
                Surname = "Engelbregh",
                ProfileImageData = s7fsIcon
            });

            Db.UserAccountSet.Add(new UserAccount
            {
                AccountName = @"7fs\User4",
                Active = true,
                FirstName = "Clover",
                IsSystemAdmin = false,
                ProfileComplete = true,
                Registered = DateTime.UtcNow,
                Surname = "Ranch",
                ProfileImageData = s7fsIcon
            });

            Db.SaveChanges();
        }

        private void CreateCustomers()
        {
            ShowMessage("CreateCustomers...");

            Db.ClientEntitySet.Add(new ClientEntity()
            {
                DateCreated = DateTime.UtcNow,
                EntityName = "Xavier",
                IsActive = true,
            });


            Db.ClientEntitySet.Add(new ClientEntity()
            {
                DateCreated = DateTime.UtcNow,
                EntityName = "Jack",
                IsActive = true,
            });

            Db.SaveChanges();
        }

        private void CreateProjects()
        {
            ShowMessage("CreateProjects...");

            Db.ProjectSet.Add(new Project
            {
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                Billable = true,
                Client = Db.ClientEntitySet.First(a => a.EntityName == "Xavier"),
                ProjectLead = Db.UserAccountSet.First(a => a.AccountName == @"7FS\user1"),
                ProjectName = "Autocar"
            });

            Db.ProjectSet.Add(new Project
            {
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                Billable = true,
                Client = Db.ClientEntitySet.First(a => a.EntityName == "Xavier"),
                ProjectLead = Db.UserAccountSet.First(a => a.AccountName == @"RAEZOR_W10\CA2"),
                ProjectName = "Phoenix"
            });

            Db.ProjectSet.Add(new Project
            {
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                Billable = false,
                Client = Db.ClientEntitySet.First(a => a.EntityName == "Jack"),
                ProjectLead = Db.UserAccountSet.First(a => a.AccountName == @"AzureAD\RAEZOR"),
                ProjectName = "Project X"
            });

            Db.SaveChanges();
        }

        private void CreateSubProjects()
        {
            ShowMessage("CreateSubProjects...");

            Db.SubProjectSet.Add(new SubProject
            {
                Project = Db.ProjectSet.First(a => a.ProjectName == "Autocar"),
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                ProjectName = "Project 01"
            });
            Db.SubProjectSet.Add(new SubProject
            {
                Project = Db.ProjectSet.First(a => a.ProjectName == "Autocar"),
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                ProjectName = "Project 02"
            });
            Db.SubProjectSet.Add(new SubProject
            {
                Project = Db.ProjectSet.First(a => a.ProjectName == "Autocar"),
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                ProjectName = "Project 03"
            });
            Db.SubProjectSet.Add(new SubProject
            {
                Project = Db.ProjectSet.First(a => a.ProjectName == "Autocar"),
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                ProjectName = "Project 04"
            });
            Db.SubProjectSet.Add(new SubProject
            {
                Project = Db.ProjectSet.First(a => a.ProjectName == "Autocar"),
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                ProjectName = "Project 05"
            });

            Db.SubProjectSet.Add(new SubProject
            {
                Project = Db.ProjectSet.First(a => a.ProjectName == "Phoenix"),
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                ProjectName = "Auto Fuel System"
            });
            Db.SubProjectSet.Add(new SubProject
            {
                Project = Db.ProjectSet.First(a => a.ProjectName == "Phoenix"),
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                ProjectName = "Space Cerp Hub"
            });
            Db.SubProjectSet.Add(new SubProject
            {
                Project = Db.ProjectSet.First(a => a.ProjectName == "Phoenix"),
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                ProjectName = "Underground Services"
            });

            Db.SaveChanges();
        }

        private void CreateBillingCycle()
        {
            ShowMessage("CreateBillingCycle");

            Db.BillingCycleEntrySet.Add(new BillingCycleEntry
            {
                Cycle = 1,
                Year = 2016,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Weekdays = 0,
                PublicHolidays = 0,
                WorkDays = 0,
                CreatedByAccountId = Db.UserAccountSet.First(a => a.IsSystemAdmin).Id,
                DateCreated = DateTime.UtcNow,
                IsClosed = false,
                IsActive = true
            });

            Db.BillingCycleEntrySet.Add(new BillingCycleEntry
            {
                Cycle = 2,
                Year = 2016,
                StartDate = DateTime.UtcNow.AddMonths(1).AddDays(1),
                EndDate = DateTime.UtcNow.AddMonths(3),
                Weekdays = 0,
                PublicHolidays = 0,
                WorkDays = 0,
                CreatedByAccountId = Db.UserAccountSet.First(a => a.IsSystemAdmin).Id,
                DateCreated = DateTime.UtcNow,
                IsClosed = false,
                IsActive = true
            });

            Db.BillingCycleEntrySet.Add(new BillingCycleEntry
            {
                Cycle = 3,
                Year = 2016,
                StartDate = DateTime.UtcNow.AddMonths(3).AddDays(1),
                EndDate = DateTime.UtcNow.AddMonths(4),
                Weekdays = 0,
                PublicHolidays = 0,
                WorkDays = 0,
                CreatedByAccountId = Db.UserAccountSet.First(a => a.IsSystemAdmin).Id,
                DateCreated = DateTime.UtcNow,
                IsClosed = false,
                IsActive = true
            });

            Db.SaveChanges();
        }

        private void CreateTimesheetInfo()
        {
            ShowMessage("CreateTimesheetInfo...");

            var projects = Db.ProjectSet.Select(a => a.ProjectName).ToList();
            var subProjects = Db.SubProjectSet.Select(a => a.ProjectName).ToList();
            var activities = Db.ActivitySet.Select(a => a.ActivityName).ToList();
            var teams = Db.TeamSet.Select(a => a.TeamName).ToList();
            var accounts =
                Db.UserAccountSet.Where(a => a.Active && a.ProfileComplete).Select(a => a.AccountName).ToList();

            var rnd = new Random(Environment.TickCount);
            var proR = rnd.Next(0, projects.Count);
            var subpR = rnd.Next(0, subProjects.Count);
            var actR = rnd.Next(0, activities.Count);
            var temR = rnd.Next(0, teams.Count);
            var accR = rnd.Next(0, accounts.Count);
            var days = rnd.Next(-10, 10);
            var hours = rnd.Next(1, 8);
            var totalRecords = rnd.Next(50, 150);
            for (var i = 0; i < totalRecords; i++)
            {
                proR = rnd.Next(0, projects.Count);
                subpR = rnd.Next(0, subProjects.Count);
                actR = rnd.Next(0, activities.Count);
                temR = rnd.Next(0, teams.Count);
                accR = rnd.Next(0, accounts.Count);
                days = rnd.Next(-10, 10);
                hours = rnd.Next(1, 8);

                var aa = projects[proR];
                var bb = subProjects[subpR];
                var cc = activities[actR];
                var dd = teams[temR];
                var ff = accounts[accR];
                var date = DateTime.UtcNow.AddDays(days);

                var timeSheetEntry = new TimesheetEntry
                {
                    CreatedByAccountId = Db.UserAccountSet.First(a => a.IsSystemAdmin).Id,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                    Comments = "Dummy Data Test",
                    ProjectId = Db.ProjectSet.First(a => a.ProjectName == aa).Id,
                    SubProjectId = Db.SubProjectSet.FirstOrDefault(a => a.ProjectName == bb && a.Project.ProjectName == aa) != null ?
                        Db.SubProjectSet.FirstOrDefault(a => a.ProjectName == bb && a.Project.ProjectName == aa).Id : (Guid?)null,
                    ActivityId = Db.ActivitySet.First(a => a.ActivityName == cc).Id,
                    TeamId = Db.TeamSet.First(a => a.TeamName == dd).Id,
                    UserAccountId = Db.UserAccountSet.First(a => a.AccountName == ff).Id,
                    DateEntry = new DateTime(date.Year, date.Month, date.Day),
                    Hours = hours,
                };

                TimesheetEntry existing;
                if (timeSheetEntry.SubProject != null)
                {
                    existing =
                        Db.TimesheetEntrySet.FirstOrDefault(a => a.UserAccountId == timeSheetEntry.UserAccountId &&
                                                                 a.ProjectId == timeSheetEntry.ProjectId &&
                                                                 a.SubProjectId == timeSheetEntry.SubProjectId &&
                                                                 a.TeamId == timeSheetEntry.TeamId &&
                                                                 a.ActivityId == timeSheetEntry.ActivityId &&
                                                                 a.DateEntry.Day == timeSheetEntry.DateEntry.Day &&
                                                                 a.DateEntry.Month == timeSheetEntry.DateEntry.Month &&
                                                                 a.DateEntry.Year == timeSheetEntry.DateEntry.Year);
                }
                else
                {
                    existing =
                        Db.TimesheetEntrySet.FirstOrDefault(a => a.UserAccountId == timeSheetEntry.UserAccountId &&
                                                                 a.ProjectId == timeSheetEntry.ProjectId &&
                                                                 a.TeamId == timeSheetEntry.TeamId &&
                                                                 a.ActivityId == timeSheetEntry.ActivityId &&
                                                                 a.DateEntry.Day == timeSheetEntry.DateEntry.Day &&
                                                                 a.DateEntry.Month == timeSheetEntry.DateEntry.Month &&
                                                                 a.DateEntry.Year == timeSheetEntry.DateEntry.Year);

                }

                if (existing == null)
                {
                    Db.TimesheetEntrySet.Add(timeSheetEntry);
                    Db.SaveChanges();
                }
                else
                    i--;
            }

        }

        private void CreateScorecardTempaltes()
        {
            ShowMessage("CreateScorecardTempaltes...");

            Db.ScorecardTemplateSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplate
            {
                IsActive = true,
                ScorecardCode = "GSC01",
                ScorecardName = "Management performance",
            });

            Db.ScorecardTemplateSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplate
            {
                IsActive = true,
                ScorecardCode = "AWE01",
                ScorecardName = "Advance work experiance",
            });

            Db.SaveChanges();
        }

        private void CreateScorecardTempaltePeriods()
        {
            ShowMessage("CreateScorecardTempaltePeriods...");

            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a=> a.ScorecardCode == "GSC01"),
                Description = "Jan/Feb",
                StartDate = new DateTime(2016,01,01),
                EndDate = new DateTime(2016,02,29),
            });
            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
                Description = "Mar/Apr",
                StartDate = new DateTime(2016, 03, 01),
                EndDate = new DateTime(2016, 04, 30),
            });
            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
                Description = "May/June",
                StartDate = new DateTime(2016, 05, 01),
                EndDate = new DateTime(2016, 06, 30),
            });
            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
                Description = "July/Aug",
                StartDate = new DateTime(2016, 07, 01),
                EndDate = new DateTime(2016, 08, 31),
            });
            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
                Description = "Sep/Oct",
                StartDate = new DateTime(2016, 09, 01),
                EndDate = new DateTime(2016, 10, 30),
            });
            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
                Description = "Nov/Dec",
                StartDate = new DateTime(2016, 11, 01),
                EndDate = new DateTime(2016, 12, 31),
            });

            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "AWE01"),
                Description = "Q1",
                StartDate = new DateTime(2016, 01, 01),
                EndDate = new DateTime(2016, 03, 31),
            });
            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "AWE01"),
                Description = "Q2",
                StartDate = new DateTime(2016, 04, 01),
                EndDate = new DateTime(2016, 06, 30),
            });
            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "AWE01"),
                Description = "Q3",
                StartDate = new DateTime(2016, 07, 01),
                EndDate = new DateTime(2016, 09, 30),
            });
            Db.ScorecardTemplatePeriodSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplatePeriod
            {
                ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "AWE01"),
                Description = "Q4",
                StartDate = new DateTime(2016, 10, 01),
                EndDate = new DateTime(2016, 12, 31),
            });

            Db.SaveChanges();
        }

        //private void CreateScorecardTempalteItems()
        //{
        //    ShowMessage("CreateScorecardTempalteItems...");

        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
        //        Code = "T",
        //        Description = "Tasks on time",
        //        Weight = 15,
        //    });
        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
        //        Code = "Q",
        //        Description = "First Time Right",
        //        Weight = 15,
        //    });
        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
        //        Code = "C",
        //        Description = "Efficiency",
        //        Weight = 15,
        //    });
        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
        //        Code = "A",
        //        Description = "Attitude and work ethics",
        //        Weight = 15,
        //    });
        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
        //        Code = "CS",
        //        Description = "Customer Service",
        //        Weight = 15,
        //    });
        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
        //        Code = "T",
        //        Description = "Timesheet on time and correct first time. A or I only",
        //        Weight = 5,
        //    });
        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "GSC01"),
        //        Code = "TS",
        //        Description = "Timesheet Score",
        //        Weight = 20,
        //    });

        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "AWE01"),
        //        Code = "L",
        //        Description = "Looks",
        //        Weight = 60,
        //    });
        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "AWE01"),
        //        Code = "G",
        //        Description = "Gender",
        //        Weight = 35,
        //    });
        //    Db.ScorecardTemplateItemSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItem
        //    {
        //        ScorecardTemplate = Db.ScorecardTemplateSet.First(a => a.ScorecardCode == "AWE01"),
        //        Code = "E",
        //        Description = "Experience",
        //        Weight = 5,
        //    });
            

        //    Db.SaveChanges();
        //}

        //private void CreateScorecardTempalteItemScores()
        //{
        //    ShowMessage("CreateScorecardTempalteItemScores...");

        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a=> a.Description == "Tasks on time"),
        //        ScoreType = ScorecardScoreType.E,
        //        Score = (decimal) 1.5,
        //        Definition = "All tasks always on time",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Tasks on time"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "80% - 90%",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Tasks on time"),
        //        ScoreType = ScorecardScoreType.I,
        //        Score = (decimal)0.5,
        //        Definition = "<80%",
        //    });

        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "First Time Right"),
        //        ScoreType = ScorecardScoreType.E,
        //        Score = (decimal)1.5,
        //        Definition = "All tasks always right first time",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "First Time Right"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "80% - 90%",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "First Time Right"),
        //        ScoreType = ScorecardScoreType.I,
        //        Score = (decimal)0.5,
        //        Definition = "<80%",
        //    });

        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Efficiency"),
        //        ScoreType = ScorecardScoreType.E,
        //        Score = (decimal)1.5,
        //        Definition = "Time taken (at acceptable quality) compared to average expectation: All work done faster than average at acceptable quality",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Efficiency"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "Work done at expected rate and quality",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Efficiency"),
        //        ScoreType = ScorecardScoreType.I,
        //        Score = (decimal)0.5,
        //        Definition = "Work done at slower rate than expected but at acceptable quality",
        //    });

        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Attitude and work ethics"),
        //        ScoreType = ScorecardScoreType.E,
        //        Score = (decimal)1.5,
        //        Definition = "Always:\n" +
        //                     "- Being on time for meetings \n" +
        //                     "- Taking ownership/responsibility for own work and staying until the job is done correctly \n" +
        //                     "- Billing ethically \n" +
        //                     "- Be trustworthy to take action and/or give feedback on assigned tasks",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Attitude and work ethics"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "80% - 90%",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Attitude and work ethics"),
        //        ScoreType = ScorecardScoreType.I,
        //        Score = (decimal)0.5,
        //        Definition = "<80%",
        //    });

        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Customer Service"),
        //        ScoreType = ScorecardScoreType.E,
        //        Score = (decimal)1.5,
        //        Definition = "Always: \n" +
        //                     "- Communicating (phone, emails) professionally, timely and with courtesy \n" +
        //                     "- Being on time and prepaired for meetings",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Customer Service"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "80% - 90%",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Customer Service"),
        //        ScoreType = ScorecardScoreType.I,
        //        Score = (decimal)0.5,
        //        Definition = "<80%",
        //    });

        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Timesheet on time and correct first time. A or I only"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "Based on monthly review of timesheets: No issues with timesheet completion (o time and correct)",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Timesheet on time and correct first time. A or I only"),
        //        ScoreType = ScorecardScoreType.I,
        //        Score = (decimal)0.5,
        //        Definition = ">= 1 mistakes in timesheet or late",
        //    });


        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Timesheet Score"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "8.5 Average logged hours = 100%",
        //    });

        //    //

        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Looks"),
        //        ScoreType = ScorecardScoreType.E,
        //        Score = (decimal)1.5,
        //        Definition = "Outrageously Beautiful",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Looks"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "Good looking",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Looks"),
        //        ScoreType = ScorecardScoreType.I,
        //        Score = (decimal)0.5,
        //        Definition = "Less than oky...",
        //    });

        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Gender"),
        //        ScoreType = ScorecardScoreType.E,
        //        Score = (decimal)1.5,
        //        Definition = "Female",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Gender"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "Male",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Gender"),
        //        ScoreType = ScorecardScoreType.I,
        //        Score = (decimal)0.5,
        //        Definition = "Other",
        //    });

        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Experience"),
        //        ScoreType = ScorecardScoreType.E,
        //        Score = (decimal)1.5,
        //        Definition = "20 Years+",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Experience"),
        //        ScoreType = ScorecardScoreType.A,
        //        Score = (decimal)1,
        //        Definition = "10 Years+",
        //    });
        //    Db.ScorecardTemplateItemScoreSet.Add(new BL.Entities.ScorecardTemplateData.ScorecardTemplateItemScore
        //    {
        //        ScorecardTemplateItem = Db.ScorecardTemplateItemSet.First(a => a.Description == "Experience"),
        //        ScoreType = ScorecardScoreType.I,
        //        Score = (decimal)0.5,
        //        Definition = "Less than 10 Years",
        //    });

        //    Db.SaveChanges();
        //}



        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}