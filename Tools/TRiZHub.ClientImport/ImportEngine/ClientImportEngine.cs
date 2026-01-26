#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.MasterData;

#endregion

namespace TRiZHub.ClientImport.ImportEngine
{
    internal class ClientImportEngine : IDisposable
    {
        private static ClientImportEngine _instance;

        private Guid _profileImageGuid;
        private DataContext Db { get; set; }

        public void Dispose()
        {
            if (Db == null)
                return;
            Db.Dispose();
            Db = null;
        }

        public static ClientImportEngine Instance()
        {
            return _instance ?? (_instance = new ClientImportEngine());
        }

        private void SetupDatabase(DataContext db)
        {
            ShowMessage("Starting Database Reload...");
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

        public void ImportClientData(DataContext db)
        {
            CreateDatabase(db);

            // Create Objects into Database
            ImportActivity();
            ImportClientEntity();
            ImportProfileImage();
            ImportUsers();
            ImportUserAccounts();
            ImportTeams();
            ImportProjects();
            ImportTimesheetEntries();

            // Finalize
            ShowMessage("---");
            ShowMessage("Finished setting up the Database...");
            db.Dispose();
            Thread.Sleep(1000);
        }

        private void ImportProfileImage()
        {
            var s7fsIcon = new ImageData
            {
                FileName = "default.jpg",
                FileData = ImageData.CreateGenericImage(File.ReadAllBytes(@"Images\7FsIcon.jpg"))
            };
            Db.ImageDataSet.Add(s7fsIcon);
            Db.SaveChanges();
            _profileImageGuid = s7fsIcon.Id;
        }

        private void InsertIntoTable(string insertStatement, string values, ref int passed, ref int total)
        {
            try
            {
                Db.Database.ExecuteSqlCommand(string.Format("{0} VALUES ({1})", insertStatement, values));
                passed++;
            }
            catch (Exception)
            {
            }
            total++;
        }

        private void LoadFile(string file, string insertStatement, ref int passed, ref int total, List<KeyValuePair<string,string>> overides)
        {
            using (var sr = new StreamReader(file))
                while (sr.Peek() >= 0)
                    if (overides == null)
                        InsertIntoTable(insertStatement, sr.ReadLine(), ref passed, ref total);
                    else
                    {
                        var line = overides.Aggregate(sr.ReadLine(), (current, keyValuePair) => current.Replace(keyValuePair.Key, keyValuePair.Value));
                        InsertIntoTable(insertStatement, line, ref passed, ref total);
                    }
        }

        private void ImportActivity()
        {
            ShowMessage("ImportActivity...",false);
            var total = 0;
            var passed = 0;
            LoadFile(@"Files\ImportActivity.txt", "INSERT INTO Activity ([Id],[ActivityName],[IsActive])", ref passed, ref total, null);
            ShowMessage(string.Format("({0}/{1})", passed, total), false);
            ShowMessage("");
        }

        private void ImportClientEntity()
        {
            ShowMessage("ImportClientEntity...", false);
            var total = 0;
            var passed = 0;
            LoadFile(@"Files\ImportClientEntity.txt",
                "INSERT INTO [ClientEntity] ([Id],[DateCreated],[IsActive],[EntityName])", ref passed,
                ref total, null);
            ShowMessage(string.Format("({0}/{1})", passed, total), false);
            ShowMessage("");
        }

        private void ImportUsers()
        {
            ShowMessage("ImportUsers...", false);
            var total = 0;
            var passed = 0;
            LoadFile(@"Files\ImportUsers.txt",
                "INSERT INTO [UserIdentity] ([Id],[AccountName],[IsSystemAdmin],[FirstName],[Surname],[ProfileImageDataId],[Registered],[Active])",
                ref passed, ref total, new List<KeyValuePair<string, string>>(new[] {new KeyValuePair<string, string>("PROFIleimageGuid",_profileImageGuid.ToString())}));
            ShowMessage(string.Format("({0}/{1})", passed, total), false);
            ShowMessage("");
        }

        private void ImportUserAccounts()
        {
            ShowMessage("ImportUserAccounts...", false);
            var total = 0;
            var passed = 0;
            LoadFile(@"Files\ImportUserAccounts.txt", "INSERT INTO [UserAccount] ([Id],[ProfileComplete])", ref passed,
                ref total, null);
            ShowMessage(string.Format("({0}/{1})", passed, total), false);
            ShowMessage("");
        }

        private void ImportTeams()
        {
            ShowMessage("ImportTeams...", false);
            var total = 0;
            var passed = 0;
            LoadFile(@"Files\ImportTeams.txt", "INSERT INTO [Team] ([Id],[TeamName],IsActive)", ref passed, ref total, null);
            ShowMessage(string.Format("({0}/{1})", passed, total), false);
            ShowMessage("");
        }

        private void ImportProjects()
        {
            ShowMessage("ImportProjects...", false);
            var total = 0;
            var passed = 0;
            LoadFile(@"Files\ImportProjects.txt",
                "INSERT INTO [Project] ([Id],[ClientId],[ProjectLeadId],[ProjectName],[Billable],[DateCreated],[IsActive])",
                ref passed, ref total, null);
            ShowMessage(string.Format("({0}/{1})", passed, total), false);
            ShowMessage("");
        }

        private void ImportTimesheetEntries()
        {
            var counterPassed = 0;
            var counterTotal = 0;

            Console.WriteLine("Press any key to proceed with loading Timesheet Records... (WARNING, LONG ACTION!!)");
            Console.ReadKey();
            var combinedLine = "";
            var insertAction =
                "INSERT INTO [dbo].[TimesheetEntry] ([Id],[UserAccountId],[ProjectId],[SubProjectId],[TeamId],[ActivityId],[CreatedByAccountId],[Comments],[Hours],[DateEntry],[DateCreated],[IsActive],[ClientEntity_Id]) VALUES (";
            using (var sr = new StreamReader(@"Files\TimesheetEntries.txt"))
            {
                while (sr.Peek() >= 0)
                {
                    counterTotal++;
                    var line = sr.ReadLine();
                    try
                    {
                        if (line.Contains("NULL"))
                        {
                            if (combinedLine.Length > 0)
                            {
                                try
                                {
                                    Db.Database.ExecuteSqlCommand(insertAction +
                                                                  combinedLine + ")");
                                    counterPassed++;
                                }
                                catch (Exception)
                                {
                                    combinedLine = "";
                                }
                            }
                            Db.Database.ExecuteSqlCommand(insertAction + line + ")");
                            combinedLine = "";
                            counterPassed++;
                        }
                        else
                            combinedLine += line;
                    }
                    catch (Exception)
                    {
                        combinedLine += line;
                    }
                    Console.Clear();
                    Console.WriteLine("Total: " + counterTotal);
                    Console.WriteLine("Passed: " + counterPassed);
                }
            }
        }


        private static void ShowMessage(string message, bool newLine = true)
        {
            if (newLine)
                Console.WriteLine(message);
            else
                Console.Write(message);
        }
    }
}