#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ActivityData;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.ScorecardTemplateData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.TeamData;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Entities.ScorecardData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.ReportData.ReportAttributes;
using TRiZHub.BL.Provider.ReportData.ReportModels.ScorecardSummary;
using TRiZHub.BL.Provider.ReportData.ReportModels.TimesheetSummary;
using TRiZHub.BL.Scripts.TimesheetReportProcedure;
using TRiZHub.BL.Provider.ReportData;
using TRiZHub.BL.Entities.OfficeEquipmentData;
using TRiZHub.BL.Provider.Security;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ClientEntityData;
using System.Web;
using TRiZHub.BL.Entities.TeamJobDesignationData;
using TRiZHub.BL.Entities.EmployerData;
using TRiZHub.BL.Entities.BillingCycleData;
using TRiZHub.BL.Entities.UserIdentityProject;
using TRiZHub.BL.Provider.ClientEntityData;
using TRiZHub.BL.Provider.ProjectData;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Runtime.Remoting.Contexts;
using TRiZHub.BL.Provider.ReportData.ReportModels.ProjectAllocationModel;
using System.IO;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using TRiZHub.BL.Migrations;
using System.Data.SqlClient;
using TRiZHub.BL.Models.Reports;

#endregion Usings

namespace TRiZHub.BL.Provider.ReportData
{
    public class ReportProvider : TRiZHubProvider, IReportProvider
    {
        public const int USER_COL_OFFSET = 6;
        public const int USER_COL_OFFSET_SUMMARY = 6;
        private const int LOG_EFFICIENCY_THRESHOLD = 8;

        private const string PERCENTAGE_FORMAT = "0.000%";

        #region Constructor

        public ReportProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion Constructor

        #region User Asset Register

        public byte[] GenerateUserAssetRegisterSummary()
        {
            Authenticate(PrivilegeType.UserAssetRegisterMaintenance);

            using (var pck = new ExcelPackage())
            {
                List<UserAccount> userAccount = new List<UserAccount>();

                userAccount = DataContext.UserAccountSet.Include(a => a.TeamJobDesignation).Where(a => a.Active).OrderBy(a => a.FirstName).ToList();

                //Create the worksheet
                var sheets = new List<ExcelWorksheet>();
                var sheetMain = pck.Workbook.Worksheets.Add("User Asset Register Summary");
                sheets.Add(sheetMain);

                short lineCount = 0;

                sheetMain.Column(1).Width = 12;

                //Header Section
                lineCount++;
                //sheetMain.Cells[lineCount, 1].Value = "User Asset Register Summary";
                //sheetMain.Cells[lineCount, 1, lineCount, 7].Merge = true;
                //sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Font.Bold = true;
                //sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Font.Size = 14;

                //lineCount++;

                sheetMain.Cells[lineCount, 1].Value = "User Id (Fixed)";
                sheetMain.Cells[lineCount, 2].Value = "User Name (Fixed)";
                sheetMain.Cells[lineCount, 3].Value = "Type (Required)";
                sheetMain.Cells[lineCount, 4].Value = "Model";
                sheetMain.Cells[lineCount, 5].Value = "Supplier Name (Required)";
                sheetMain.Cells[lineCount, 6].Value = "Serial Number (Required)";
                sheetMain.Cells[lineCount, 7].Value = "Cost (Required)";
                sheetMain.Cells[lineCount, 8].Value = "Purchase Date (Required)";
                sheetMain.Cells[lineCount, 9].Value = "Invoice Number (Required)";
                sheetMain.Cells[lineCount, 10].Value = "Assigned Date (Required)";
                sheetMain.Cells[lineCount, 11].Value = "Return Date";
                sheetMain.Cells[lineCount, 12].Value = "Asset Register (Required)";
                sheetMain.Cells[lineCount, 13].Value = "Is Accounting Item";
                sheetMain.Cells[lineCount, 14].Value = "Notes";

                foreach (UserAccount account in userAccount)
                {
                    if (account.OfficeEquipemnt.Count == 0)
                    {
                        lineCount++;

                        sheetMain.Cells[lineCount, 1].Value = account.Id;
                        sheetMain.Cells[lineCount, 2].Value = account.Fullname;
                    }

                    foreach (OfficeEquipment item in account.OfficeEquipemnt)
                    {
                        lineCount++;
                        sheetMain.Cells[lineCount, 1].Value = account.Id;
                        sheetMain.Cells[lineCount, 2].Value = account.Fullname;
                        sheetMain.Cells[lineCount, 3].Value = item.Type;
                        sheetMain.Cells[lineCount, 4].Value = item.Model;
                        sheetMain.Cells[lineCount, 5].Value = item.SupplierName;
                        sheetMain.Cells[lineCount, 6].Value = item.SerialNumber;
                        sheetMain.Cells[lineCount, 7].Value = item.Cost;
                        sheetMain.Cells[lineCount, 8].Value = item.PurchaseDate.ToShortDateString();
                        sheetMain.Cells[lineCount, 9].Value = item.InvoiceNumber;
                        sheetMain.Cells[lineCount, 10].Value = item.AssignedDate.Value.ToShortDateString();
                        sheetMain.Cells[lineCount, 11].Value = (item.ReturnDate == null) ? "" : item.ReturnDate.Value.ToShortDateString();
                        sheetMain.Cells[lineCount, 12].Value = item.AssetRegister;
                        sheetMain.Cells[lineCount, 13].Value = (item.IsAccountingItem) ? "Yes" : "No";
                        sheetMain.Cells[lineCount, 14].Value = item.Notes;
                    }

                    //lineCount++;
                    //lineCount++;
                }

                AutoWidthColumns(ref sheetMain);
                return pck.GetAsByteArray();
            }
        }

        #endregion User Asset Register

        #region User

        public byte[] GenrateUserSummary(Guid? userID, bool showInactive)
        {
            Authenticate(PrivilegeType.ReportGenerationUserSummary);

            if (userID == Guid.Empty)
            {
                using (var pck = new ExcelPackage())
                {
                    List<UserAccount> userAccount = new List<UserAccount>();

                    if (showInactive)
                    {
                        userAccount = DataContext.UserAccountSet.OrderBy(a => a.FirstName).ToList();
                    }
                    else
                    {
                        userAccount = DataContext.UserAccountSet.Where(a => a.Active == true).OrderBy(a => a.FirstName).ToList();
                    }

                    //Create the worksheet
                    var sheets = new List<ExcelWorksheet>();
                    var sheetMain = pck.Workbook.Worksheets.Add("User Summary");
                    sheets.Add(sheetMain);

                    short lineCount = 0;

                    sheetMain.Column(1).Width = 22;

                    //Header Section
                    lineCount++;
                    sheetMain.Cells[lineCount, 1].Value = "User Summary Report";
                    sheetMain.Cells[lineCount, 1, lineCount, 7].Merge = true;
                    sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Font.Bold = true;
                    sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Font.Size = 18;

                    lineCount++;

                    foreach (var account in userAccount)
                    {
                        lineCount++;

                        sheetMain.Cells[lineCount, 1].Value = "Name: " + account.FirstName;
                        lineCount++;
                        sheetMain.Cells[lineCount, 1].Value = "Surname: " + account.Surname;
                        sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Border.Bottom.Color.SetColor(Color.Black);

                        //Body Section

                        // Personal Information
                        lineCount++;
                        lineCount++;

                        // add main heading
                        sheetMain.Cells[lineCount, 1].Value = "Personal Information";
                        sheetMain.Cells[lineCount, 1, lineCount, 22].Merge = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(79,
                                129,
                                189)); //Set color to dark blue
                        sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Font.Color.SetColor(Color.White);

                        // add sub headings
                        lineCount++;

                        sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(
                            192, 192,
                            192)); //Set color to light grey

                        sheetMain.Cells[lineCount, 1].Value = "Full Names";
                        sheetMain.Cells[lineCount, 2].Value = "Surname";
                        sheetMain.Cells[lineCount, 3].Value = "Title";
                        sheetMain.Cells[lineCount, 4].Value = "Id Number";
                        sheetMain.Cells[lineCount, 5].Value = "DOB";
                        sheetMain.Cells[lineCount, 6].Value = "Spouse Name";
                        sheetMain.Cells[lineCount, 7].Value = "Children";
                        sheetMain.Cells[lineCount, 8].Value = "Company";
                        sheetMain.Cells[lineCount, 9].Value = "Work Experience Start Date";
                        sheetMain.Cells[lineCount, 10].Value = "Employment Start Date";
                        sheetMain.Cells[lineCount, 11].Value = "Employment End Date";
                        sheetMain.Cells[lineCount, 12].Value = "Race";
                        sheetMain.Cells[lineCount, 13].Value = "Gender";
                        sheetMain.Cells[lineCount, 14].Value = "Door Tag #";
                        sheetMain.Cells[lineCount, 15].Value = "CellPhone Number";
                        sheetMain.Cells[lineCount, 16].Value = "Phone Extension";
                        sheetMain.Cells[lineCount, 17].Value = "Land line Number";
                        sheetMain.Cells[lineCount, 18].Value = "Company Email";
                        sheetMain.Cells[lineCount, 19].Value = "Other Email";
                        sheetMain.Cells[lineCount, 20].Value = "Medical Aid Scheme";
                        sheetMain.Cells[lineCount, 21].Value = "Medical Aid Scheme option";
                        sheetMain.Cells[lineCount, 22].Value = "Medical Aid Number";

                        foreach (var item in account.PersonalInformation)
                        {
                            lineCount++;

                            sheetMain.Cells[lineCount, 1].Value = item.FullNames;
                            sheetMain.Cells[lineCount, 2].Value = item.Surname;
                            sheetMain.Cells[lineCount, 3].Value = item.Title;
                            sheetMain.Cells[lineCount, 4].Value = item.IdNumber;
                            if (item.Dob != null) sheetMain.Cells[lineCount, 5].Value = item.Dob.ToShortDateString();
                            sheetMain.Cells[lineCount, 6].Value = item.SpouseName;
                            sheetMain.Cells[lineCount, 7].Value = item.Children;
                            sheetMain.Cells[lineCount, 8].Value = item.Company;
                            if (item.WorkExperienceStartDate != null) sheetMain.Cells[lineCount, 9].Value = item.WorkExperienceStartDate.ToShortDateString();
                            if (item.EmploymentStartDate != null) sheetMain.Cells[lineCount, 10].Value = item.EmploymentStartDate.ToShortDateString();
                            sheetMain.Cells[lineCount, 11].Value = item.EmploymentEndDate;
                            sheetMain.Cells[lineCount, 12].Value = item.Race;
                            sheetMain.Cells[lineCount, 13].Value = item.Gender;
                            sheetMain.Cells[lineCount, 14].Value = item.DoorTagNumber;
                            sheetMain.Cells[lineCount, 15].Value = item.CellPhone;
                            sheetMain.Cells[lineCount, 16].Value = item.PhoneExtension;
                            sheetMain.Cells[lineCount, 17].Value = item.LandLinePhone;
                            sheetMain.Cells[lineCount, 18].Value = item.CompanyEmail;
                            sheetMain.Cells[lineCount, 19].Value = item.OtherEmail;
                            sheetMain.Cells[lineCount, 20].Value = item.MedicalScheme;
                            sheetMain.Cells[lineCount, 21].Value = item.MedicalSchemeOption;
                            sheetMain.Cells[lineCount, 22].Value = item.MedicalAidNumber;
                        }

                        lineCount++;
                        lineCount++;

                        //Billing Rates table

                        // add main heading
                        sheetMain.Cells[lineCount, 1].Value = "Billing Rates";
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Merge = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(79,
                                129,
                                189)); //Set color to dark blue
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Color.SetColor(Color.White);

                        // add sub headings
                        lineCount++;

                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(192,
                                192,
                                192)); //Set color to light grey

                        sheetMain.Cells[lineCount, 1].Value = "Rate";
                        sheetMain.Cells[lineCount, 2].Value = "Start Date";
                        sheetMain.Cells[lineCount, 3].Value = "End Date";

                        foreach (var item in account.BillingRates)
                        {
                            lineCount++;

                            sheetMain.Cells[lineCount, 1].Value = item.Rate;
                            if (item.StartDate != null) sheetMain.Cells[lineCount, 2].Value = item.StartDate.ToShortDateString();
                            if (item.EndDate != null) sheetMain.Cells[lineCount, 3].Value = item.EndDate.ToShortDateString();
                        }

                        lineCount++;
                        lineCount++;

                        // Emergency Contact Information

                        // add main heading
                        sheetMain.Cells[lineCount, 1].Value = "Emergency Contact Information";
                        sheetMain.Cells[lineCount, 1, lineCount, 5].Merge = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(79,
                                129,
                                189)); //Set color to dark blue
                        sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Font.Color.SetColor(Color.White);

                        // add sub headings
                        lineCount++;

                        sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(192,
                                192,
                                192)); //Set color to light grey

                        sheetMain.Cells[lineCount, 1].Value = "Name";
                        sheetMain.Cells[lineCount, 2].Value = "Surname";
                        sheetMain.Cells[lineCount, 3].Value = "Relationship";
                        sheetMain.Cells[lineCount, 4].Value = "Cell Phone Number";
                        sheetMain.Cells[lineCount, 5].Value = "Land Line Number";

                        foreach (var item in account.EmergancyContacts)
                        {
                            lineCount++;

                            sheetMain.Cells[lineCount, 1].Value = item.Name;
                            sheetMain.Cells[lineCount, 2].Value = item.Surname;
                            sheetMain.Cells[lineCount, 3].Value = item.Relationship;
                            sheetMain.Cells[lineCount, 4].Value = item.CellphoneNumber;
                            sheetMain.Cells[lineCount, 5].Value = item.LandLineNumber;
                        }

                        lineCount++;
                        lineCount++;

                        // Travel Information

                        // add main heading
                        sheetMain.Cells[lineCount, 1].Value = "Travel Information";
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Merge = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(79,
                                129,
                                189)); //Set color to dark blue
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Color.SetColor(Color.White);

                        // add sub headings
                        lineCount++;

                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(192,
                                192,
                                192)); //Set color to light grey

                        sheetMain.Cells[lineCount, 1].Value = "Document Type";
                        sheetMain.Cells[lineCount, 2].Value = "Number";
                        sheetMain.Cells[lineCount, 3].Value = "Expiry Date";

                        foreach (var item in account.TravelInformations)
                        {
                            lineCount++;

                            sheetMain.Cells[lineCount, 1].Value = item.DocumentType;
                            sheetMain.Cells[lineCount, 2].Value = item.Number;
                            if (item.ExpiryDate != null) sheetMain.Cells[lineCount, 3].Value = item.ExpiryDate.ToShortDateString();
                        }

                        lineCount++;
                        lineCount++;

                        // Office Equipemnt

                        // add main heading
                        sheetMain.Cells[lineCount, 1].Value = "Asset Register";
                        sheetMain.Cells[lineCount, 1, lineCount, 9].Merge = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(79,
                                129,
                                189)); //Set color to dark blue
                        sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Font.Color.SetColor(Color.White);

                        // add sub headings
                        lineCount++;

                        sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(192,
                                192,
                                192)); //Set color to light grey

                        sheetMain.Cells[lineCount, 1].Value = "Type";
                        sheetMain.Cells[lineCount, 2].Value = "Supplier Name";
                        sheetMain.Cells[lineCount, 3].Value = "Serial Number";
                        sheetMain.Cells[lineCount, 4].Value = "Cost";
                        sheetMain.Cells[lineCount, 5].Value = "Purchase Date";
                        sheetMain.Cells[lineCount, 6].Value = "Invoice Number";
                        sheetMain.Cells[lineCount, 7].Value = "Assigned Date";
                        sheetMain.Cells[lineCount, 8].Value = "Return Date";
                        sheetMain.Cells[lineCount, 9].Value = "Asset Register";

                        foreach (var item in account.OfficeEquipemnt)
                        {
                            lineCount++;

                            sheetMain.Cells[lineCount, 1].Value = item.Type;
                            sheetMain.Cells[lineCount, 2].Value = item.SupplierName;
                            sheetMain.Cells[lineCount, 3].Value = item.SerialNumber;
                            sheetMain.Cells[lineCount, 4].Value = item.Cost;
                            if (item.PurchaseDate != null) sheetMain.Cells[lineCount, 5].Value = item.PurchaseDate.ToShortDateString();
                            sheetMain.Cells[lineCount, 6].Value = item.InvoiceNumber;
                            if (item.AssignedDate != null) sheetMain.Cells[lineCount, 7].Value = item.AssignedDate.Value.ToShortDateString();
                            if (item.ReturnDate != null) sheetMain.Cells[lineCount, 8].Value = item.ReturnDate.Value.ToShortDateString();
                            sheetMain.Cells[lineCount, 9].Value = item.AssetRegister;
                        }

                        lineCount++;
                        lineCount++;

                        // team and job designation

                        // add main heading
                        sheetMain.Cells[lineCount, 1].Value = "Team and Job Designation";
                        sheetMain.Cells[lineCount, 1, lineCount, 6].Merge = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(79,
                                129,
                                189)); //Set color to dark blue
                        sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Font.Color.SetColor(Color.White);

                        // add sub headings
                        lineCount++;

                        sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Font.Bold = true;
                        sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //Set Pattern for the background to Solid
                        sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Fill.BackgroundColor.SetColor(
                            Color.FromArgb(192,
                                192,
                                192)); //Set color to light grey

                        sheetMain.Cells[lineCount, 1].Value = "Client";
                        sheetMain.Cells[lineCount, 2].Value = "Line Leader";
                        sheetMain.Cells[lineCount, 3].Value = "Job Designation";
                        sheetMain.Cells[lineCount, 4].Value = "Start Date";
                        sheetMain.Cells[lineCount, 5].Value = "End Date";
                        sheetMain.Cells[lineCount, 6].Value = "Employer";

                        var teamJob = DataContext.TeamJobDesignationSet.Include(t => t.UserAccount).Include(t => t.Client).Where(t => t.UserAccountId == account.Id).ToList();
                        foreach (var item in teamJob)
                        {
                            lineCount++;
                            sheetMain.Cells[lineCount, 1].Value = item.Client.EntityName;
                            if (item.LineLeader != null)
                                sheetMain.Cells[lineCount, 2].Value = item.LineLeader.FirstName + " " + item.LineLeader.Surname;
                            sheetMain.Cells[lineCount, 3].Value = item.JobDesignation;
                            if (item.StartDate != null) sheetMain.Cells[lineCount, 4].Value = item.StartDate.ToShortDateString();
                            if (item.EndDate != null)
                                if (item.EndDate != null) sheetMain.Cells[lineCount, 5].Value = ((DateTime)item.EndDate).ToShortDateString();
                            sheetMain.Cells[lineCount, 6].Value = item.Location;
                        }

                        lineCount++;
                        lineCount++;
                    }

                    AutoWidthColumns(ref sheetMain);
                    return pck.GetAsByteArray();
                }
            }

            using (var pck = new ExcelPackage())
            {
                //Create the worksheet
                var sheets = new List<ExcelWorksheet>();
                var sheetMain = pck.Workbook.Worksheets.Add("User Summary");
                sheets.Add(sheetMain);

                var teamJob = DataContext.TeamJobDesignationSet.Include(t => t.UserAccount).Include(t => t.Client).Where(t => t.UserAccountId == userID).ToList();
                var userAccount = DataContext.UserAccountSet.Include(a => a.TeamJobDesignation).FirstOrDefault(a => a.Id == userID);

                // STARTING POINT
                short lineCount = 0;
                sheetMain.Column(1).Width = 22;

                //Header Section
                lineCount++;
                sheetMain.Cells[lineCount, 1].Value = "User Summary Report";
                sheetMain.Cells[lineCount, 1, lineCount, 7].Merge = true;
                sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Font.Size = 18;

                lineCount++;
                lineCount++;

                sheetMain.Cells[lineCount, 1].Value = "Name: " + userAccount.FirstName;
                lineCount++;
                sheetMain.Cells[lineCount, 1].Value = "Surname: " + userAccount.Surname;
                sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheetMain.Cells[lineCount, 1, lineCount, 7].Style.Border.Bottom.Color.SetColor(Color.Black);

                //Body Section

                // Personal Information
                lineCount++;
                lineCount++;

                // add main heading
                sheetMain.Cells[lineCount, 1].Value = "Personal Information";
                sheetMain.Cells[lineCount, 1, lineCount, 22].Merge = true;
                sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79,
                    129,
                    189)); //Set color to dark blue
                sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Font.Color.SetColor(Color.White);

                // add sub headings
                lineCount++;

                sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 22].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(
                    192, 192,
                    192)); //Set color to light grey

                sheetMain.Cells[lineCount, 1].Value = "Full Names";
                sheetMain.Cells[lineCount, 2].Value = "Surname";
                sheetMain.Cells[lineCount, 3].Value = "Title";
                sheetMain.Cells[lineCount, 4].Value = "Id Number";
                sheetMain.Cells[lineCount, 5].Value = "DOB";
                sheetMain.Cells[lineCount, 6].Value = "Spouse Name";
                sheetMain.Cells[lineCount, 7].Value = "Children";
                sheetMain.Cells[lineCount, 8].Value = "Company";
                sheetMain.Cells[lineCount, 9].Value = "Work Experience Start Date";
                sheetMain.Cells[lineCount, 10].Value = "Employment Start Date";
                sheetMain.Cells[lineCount, 11].Value = "Employment End Date";
                sheetMain.Cells[lineCount, 12].Value = "Race";
                sheetMain.Cells[lineCount, 13].Value = "Gender";
                sheetMain.Cells[lineCount, 14].Value = "Door Tag #";
                sheetMain.Cells[lineCount, 15].Value = "CellPhone Number";
                sheetMain.Cells[lineCount, 16].Value = "Phone Extension";
                sheetMain.Cells[lineCount, 17].Value = "Land line Number";
                sheetMain.Cells[lineCount, 18].Value = "Company Email";
                sheetMain.Cells[lineCount, 19].Value = "Other Email";
                sheetMain.Cells[lineCount, 20].Value = "Medical Aid Scheme";
                sheetMain.Cells[lineCount, 21].Value = "Medical Aid Scheme option";
                sheetMain.Cells[lineCount, 22].Value = "Medical Aid Number";

                foreach (var item in userAccount.PersonalInformation)
                {
                    lineCount++;

                    sheetMain.Cells[lineCount, 1].Value = item.FullNames;
                    sheetMain.Cells[lineCount, 2].Value = item.Surname;
                    sheetMain.Cells[lineCount, 3].Value = item.Title;
                    sheetMain.Cells[lineCount, 4].Value = item.IdNumber;
                    if (item.Dob != null) sheetMain.Cells[lineCount, 5].Value = item.Dob.ToShortDateString();
                    sheetMain.Cells[lineCount, 6].Value = item.SpouseName;
                    sheetMain.Cells[lineCount, 7].Value = item.Children;
                    sheetMain.Cells[lineCount, 8].Value = item.Company;
                    if (item.WorkExperienceStartDate != null) sheetMain.Cells[lineCount, 9].Value = item.WorkExperienceStartDate.ToShortDateString();
                    if (item.EmploymentStartDate != null) sheetMain.Cells[lineCount, 10].Value = item.EmploymentStartDate.ToShortDateString();
                    sheetMain.Cells[lineCount, 11].Value = item.EmploymentEndDate;
                    sheetMain.Cells[lineCount, 12].Value = item.Race;
                    sheetMain.Cells[lineCount, 13].Value = item.Gender;
                    sheetMain.Cells[lineCount, 14].Value = item.DoorTagNumber;
                    sheetMain.Cells[lineCount, 15].Value = item.CellPhone;
                    sheetMain.Cells[lineCount, 16].Value = item.PhoneExtension;
                    sheetMain.Cells[lineCount, 17].Value = item.LandLinePhone;
                    sheetMain.Cells[lineCount, 18].Value = item.CompanyEmail;
                    sheetMain.Cells[lineCount, 19].Value = item.OtherEmail;
                    sheetMain.Cells[lineCount, 20].Value = item.MedicalScheme;
                    sheetMain.Cells[lineCount, 21].Value = item.MedicalSchemeOption;
                    sheetMain.Cells[lineCount, 22].Value = item.MedicalAidNumber;
                }

                lineCount++;
                lineCount++;

                //Billing Rates table

                // add main heading
                sheetMain.Cells[lineCount, 1].Value = "Billing Rates";
                sheetMain.Cells[lineCount, 1, lineCount, 3].Merge = true;
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79,
                    129,
                    189)); //Set color to dark blue
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Color.SetColor(Color.White);

                // add sub headings
                lineCount++;

                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192,
                    192,
                    192)); //Set color to light grey

                sheetMain.Cells[lineCount, 1].Value = "Rate";
                sheetMain.Cells[lineCount, 2].Value = "Start Date";
                sheetMain.Cells[lineCount, 3].Value = "End Date";

                foreach (var item in userAccount.BillingRates)
                {
                    lineCount++;

                    sheetMain.Cells[lineCount, 1].Value = item.Rate;
                    if (item.StartDate != null) sheetMain.Cells[lineCount, 2].Value = item.StartDate.ToShortDateString();
                    if (item.EndDate != null) sheetMain.Cells[lineCount, 3].Value = item.EndDate.ToShortDateString();
                }

                lineCount++;
                lineCount++;

                // Emergency Contact Information

                // add main heading
                sheetMain.Cells[lineCount, 1].Value = "Emergency Contact Information";
                sheetMain.Cells[lineCount, 1, lineCount, 5].Merge = true;
                sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79,
                    129,
                    189)); //Set color to dark blue
                sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Font.Color.SetColor(Color.White);

                // add sub headings
                lineCount++;

                sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 5].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192,
                    192,
                    192)); //Set color to light grey

                sheetMain.Cells[lineCount, 1].Value = "Name";
                sheetMain.Cells[lineCount, 2].Value = "Surname";
                sheetMain.Cells[lineCount, 3].Value = "Relationship";
                sheetMain.Cells[lineCount, 4].Value = "Cell Phone Number";
                sheetMain.Cells[lineCount, 5].Value = "Land Line Number";

                foreach (var item in userAccount.EmergancyContacts)
                {
                    lineCount++;

                    sheetMain.Cells[lineCount, 1].Value = item.Name;
                    sheetMain.Cells[lineCount, 2].Value = item.Surname;
                    sheetMain.Cells[lineCount, 3].Value = item.Relationship;
                    sheetMain.Cells[lineCount, 4].Value = item.CellphoneNumber;
                    sheetMain.Cells[lineCount, 5].Value = item.LandLineNumber;
                }

                lineCount++;
                lineCount++;

                // Travel Information

                // add main heading
                sheetMain.Cells[lineCount, 1].Value = "Travel Information";
                sheetMain.Cells[lineCount, 1, lineCount, 3].Merge = true;
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79,
                    129,
                    189)); //Set color to dark blue
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Color.SetColor(Color.White);

                // add sub headings
                lineCount++;

                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192,
                    192,
                    192)); //Set color to light grey

                sheetMain.Cells[lineCount, 1].Value = "Document Type";
                sheetMain.Cells[lineCount, 2].Value = "Number";
                sheetMain.Cells[lineCount, 3].Value = "Expiry Date";

                foreach (var item in userAccount.TravelInformations)
                {
                    lineCount++;

                    sheetMain.Cells[lineCount, 1].Value = item.DocumentType;
                    sheetMain.Cells[lineCount, 2].Value = item.Number;
                    if (item.ExpiryDate != null) sheetMain.Cells[lineCount, 3].Value = item.ExpiryDate.ToShortDateString();
                }

                lineCount++;
                lineCount++;

                // Office Equipemnt

                // add main heading
                sheetMain.Cells[lineCount, 1].Value = "Asset Register";
                sheetMain.Cells[lineCount, 1, lineCount, 9].Merge = true;
                sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79,
                    129,
                    189)); //Set color to dark blue

                // add sub headings
                lineCount++;

                sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 9].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192,
                    192,
                    192)); //Set color to light grey

                sheetMain.Cells[lineCount, 1].Value = "Type";
                sheetMain.Cells[lineCount, 2].Value = "Supplier Name";
                sheetMain.Cells[lineCount, 3].Value = "Serial Number";
                sheetMain.Cells[lineCount, 4].Value = "Cost";
                sheetMain.Cells[lineCount, 5].Value = "Purchase Date";
                sheetMain.Cells[lineCount, 6].Value = "Invoice Number";
                sheetMain.Cells[lineCount, 7].Value = "Assigned Date";
                sheetMain.Cells[lineCount, 8].Value = "Return Date";
                sheetMain.Cells[lineCount, 9].Value = "Asset Register";

                foreach (var item in userAccount.OfficeEquipemnt)
                {
                    lineCount++;

                    sheetMain.Cells[lineCount, 1].Value = item.Type;
                    sheetMain.Cells[lineCount, 2].Value = item.SupplierName;
                    sheetMain.Cells[lineCount, 3].Value = item.SerialNumber;
                    sheetMain.Cells[lineCount, 4].Value = item.Cost;
                    if (item.PurchaseDate != null) sheetMain.Cells[lineCount, 5].Value = item.PurchaseDate.ToShortDateString();
                    sheetMain.Cells[lineCount, 6].Value = item.InvoiceNumber;
                    if (item.AssignedDate != null) sheetMain.Cells[lineCount, 7].Value = item.AssignedDate.Value.ToShortDateString();
                    if (item.ReturnDate != null) sheetMain.Cells[lineCount, 8].Value = item.ReturnDate.Value.ToShortDateString();
                    sheetMain.Cells[lineCount, 9].Value = item.AssetRegister;
                }

                lineCount++;
                lineCount++;

                // team and job designation

                // add main heading
                sheetMain.Cells[lineCount, 1].Value = "Team and Job Designation";
                sheetMain.Cells[lineCount, 1, lineCount, 6].Merge = true;
                sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79,
                    129,
                    189)); //Set color to dark blue
                sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Font.Color.SetColor(Color.White);

                // add sub headings
                lineCount++;

                sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //Set Pattern for the background to Solid
                sheetMain.Cells[lineCount, 1, lineCount, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192,
                    192,
                    192)); //Set color to light grey

                sheetMain.Cells[lineCount, 1].Value = "Client";
                sheetMain.Cells[lineCount, 2].Value = "Line Leader";
                sheetMain.Cells[lineCount, 3].Value = "Job Designation";
                sheetMain.Cells[lineCount, 4].Value = "Start Date";
                sheetMain.Cells[lineCount, 5].Value = "End Date";
                sheetMain.Cells[lineCount, 6].Value = "Location";

                foreach (var item in teamJob)
                {
                    lineCount++;

                    sheetMain.Cells[lineCount, 1].Value = item.Client.EntityName;
                    if (item.LineLeader != null)
                        sheetMain.Cells[lineCount, 2].Value = item.LineLeader.FirstName + " " + item.LineLeader.Surname;
                    sheetMain.Cells[lineCount, 3].Value = item.JobDesignation;
                    if (item.StartDate != null) sheetMain.Cells[lineCount, 4].Value = item.StartDate.ToShortDateString();
                    if (item.EndDate != null)
                        if (item.EndDate != null) sheetMain.Cells[lineCount, 5].Value = ((DateTime)item.EndDate).ToShortDateString();
                    sheetMain.Cells[lineCount, 6].Value = item.Location;
                }

                AutoWidthColumns(ref sheetMain);
                return pck.GetAsByteArray();
            }
        }

/*        public byte[] GenerateProjectAllocationCSV(string userAccounts, bool onlyActiveUsers, bool onlyActiveClients, bool onlyActiveProjects, bool onlyActiveSubProjects)
        {
            Authenticate(PrivilegeType.ReportGenerationUserProjects);
            StringBuilder pck = new StringBuilder();


            userAccounts = userAccounts.Replace(",", "','");
            if (userAccounts != "All") userAccounts = "'" + userAccounts + "'";

            var query = DataContext.Database.SqlQuery<ProjectAllocationReportModel>("exec [dbo].[GetProjectAllocationReport] @userAccountIDs,@onlyActiveUsers,@onlyActiveClients,@onlyActiveProjects,@onlyActiveSubProjects",
            new SqlParameter("@userAccountIDs", userAccounts),
            new SqlParameter("@onlyActiveUsers", !onlyActiveUsers),
            new SqlParameter("@onlyActiveClients", !onlyActiveClients),
            new SqlParameter("@onlyActiveProjects", !onlyActiveProjects),
            new SqlParameter("@onlyActiveSubProjects", !onlyActiveSubProjects)
            );

            var data = query.ToList();

            // Create the worksheet

            short lineCount = 0;

            // Header Section
//            lineCount++;
//            pck.AppendLine("Report Date : " + string.Format("{0}", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString()));


            // Title Section
//            lineCount++;
//            pck.AppendLine("User Project Allocation Report");

//            lineCount++;

            pck.AppendLine("Users,User Active,Customer,Customer Active,Project,Project Active,Sub Project,Sub Project Active");
            lineCount++;


            for (int i = 0; i < data.Count; i++)
            {
                pck.AppendLine(data[i].FullName.Replace(",",";") + "," + data[i].UserActive + "," + data[i].ClientName.Replace(",", ";") + "," + data[i].ClientActive + "," + data[i].ProjectName.Replace(",", ";") + "," + data[i].ProjectActive + "," + data[i].SubProjectName.Replace(",", ";") + "," + data[i].SubProjectActive);
                lineCount++;
            }

            return Encoding.ASCII.GetBytes((pck.ToString()));

        }
*/
        public byte[] GenerateProjectAllocation(string userAccounts, bool onlyActiveUsers, bool onlyActiveClients, bool onlyActiveProjects, bool onlyActiveSubProjects)
        {
            Authenticate(PrivilegeType.ReportGenerationUserProjects);
            using (var pck = new ExcelPackage())
            {

                userAccounts = userAccounts.Replace(",", "','");
                if (userAccounts != "All") userAccounts = "'" + userAccounts + "'";

                var query = DataContext.Database.SqlQuery<ProjectAllocationReportModel>("exec [dbo].[GetProjectAllocationReport] @userAccountIDs,@onlyActiveUsers,@onlyActiveClients,@onlyActiveProjects,@onlyActiveSubProjects",
                new SqlParameter("@userAccountIDs", userAccounts),
                new SqlParameter("@onlyActiveUsers", !onlyActiveUsers),
                new SqlParameter("@onlyActiveClients", !onlyActiveClients),
                new SqlParameter("@onlyActiveProjects", !onlyActiveProjects),
                new SqlParameter("@onlyActiveSubProjects", !onlyActiveSubProjects)
                );

                var data = query.ToList();

                // Create the worksheet
                var sheets = new List<ExcelWorksheet>();
                var sheetMain = pck.Workbook.Worksheets.Add("User Project Allocation");
                sheets.Add(sheetMain);

                short lineCount = 0;

                // Header Section
                lineCount++;
                sheetMain.Cells[lineCount, 1].Value = "Report Date";
                sheetMain.Cells[lineCount, 1].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 2].Value = string.Format("{0}", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

                // Title Section
                lineCount++;
                sheetMain.Cells[lineCount, 1].Value = "User Project Allocation Report";
                sheetMain.Cells[lineCount, 1, lineCount, 4].Merge = true;
                sheetMain.Cells[lineCount, 1, lineCount, 4].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 4].Style.Font.Size = 18;
                lineCount++;

                sheetMain.Cells[lineCount, 1].Value = "User";
                sheetMain.Cells[lineCount, 2].Value = "User Active";
                sheetMain.Cells[lineCount, 3].Value = "Customer";
                sheetMain.Cells[lineCount, 4].Value = "Customer Active";
                sheetMain.Cells[lineCount, 5].Value = "Project Allocation";
                sheetMain.Cells[lineCount, 6].Value = "Project Code";
                sheetMain.Cells[lineCount, 7].Value = "Project";
                sheetMain.Cells[lineCount, 8].Value = "Project Active";
                sheetMain.Cells[lineCount, 9].Value = "Sub Project Allocation";
                sheetMain.Cells[lineCount, 10].Value = "Sub Project Code";
                sheetMain.Cells[lineCount, 11].Value = "Sub Project";
                sheetMain.Cells[lineCount, 12].Value = "Sub Project Active";
                lineCount++;

                var lines = sheetMain.Cells.Rows;

                for (int i = 0; (i < data.Count && i <=32000) ; i++)
                {
                    sheetMain.Cells[lineCount, 1].Value = data[i].FullName;
                    sheetMain.Cells[lineCount, 2].Value = data[i].UserActive;
                    sheetMain.Cells[lineCount, 3].Value = data[i].ClientName;
                    sheetMain.Cells[lineCount, 4].Value = data[i].ClientActive;
                    if (data[i].ProjectNumber == null)
                    {
                        sheetMain.Cells[lineCount, 5].Value = "Automatic";
                        sheetMain.Cells[lineCount, 6].Value = "";
                        sheetMain.Cells[lineCount, 7].Value = "";
                        sheetMain.Cells[lineCount, 8].Value = "";
                        sheetMain.Cells[lineCount, 9].Value = "Automatic";
                        sheetMain.Cells[lineCount, 10].Value = "";
                        sheetMain.Cells[lineCount, 11].Value = "";
                        sheetMain.Cells[lineCount, 12].Value = "";

                    }
                    else
                    {
                        sheetMain.Cells[lineCount, 5].Value = "Manual";
                        sheetMain.Cells[lineCount, 6].Value = data[i].ProjectNumber;
                        sheetMain.Cells[lineCount, 7].Value = data[i].ProjectName;
                        sheetMain.Cells[lineCount, 8].Value = data[i].ProjectActive != null ? data[i].ProjectActive + "" : "";
                        if (data[i].SubProjectNumber == null)
                        {
                            sheetMain.Cells[lineCount, 9].Value = "Automatic";
                            sheetMain.Cells[lineCount, 10].Value = "";
                            sheetMain.Cells[lineCount, 11].Value = "";
                            sheetMain.Cells[lineCount, 12].Value = "";
                        }
                        else
                        {
                            sheetMain.Cells[lineCount, 9].Value = "Manual";
                            sheetMain.Cells[lineCount, 10].Value = data[i].SubProjectNumber;
                            sheetMain.Cells[lineCount, 11].Value = data[i].SubProjectName;
                            sheetMain.Cells[lineCount, 12].Value = data[i].SubProjectActive != null ? data[i].SubProjectActive + "" : "";

                        }

                    }
                    lineCount++;
                }

                AutoWidthColumns(ref sheetMain);
                return pck.GetAsByteArray();
            }
        }

        public byte[] GenerateRoleAllocation(List<Guid> userAccounts, bool includeInactiveRoles, bool includeInactiveUsers)
        {
            Authenticate(PrivilegeType.ReportGenerationUserRoles);
            using (var pck = new ExcelPackage())
            {
                if (userAccounts.Count == 0)
                {
                    userAccounts = DataContext.UserAccountSet.Select(ua => ua.Id).ToList();
                }

                var users = DataContext.UserAccountSet.Where(ui => userAccounts.Contains(ui.Id)).OrderBy(ui => ui.FirstName).ThenBy(ui => ui.Surname).ToList();
                var privelages = DataContext.PrivilegeSet.OrderBy(priv => priv.Description).ToList();

                // Remove if not valid includeInactiveUsers
                if (!includeInactiveUsers)
                {
                    users = users.Where(u => u.Active == true).ToList();
                }

                //Create the worksheet
                var sheets = new List<ExcelWorksheet>();
                var sheetMain = pck.Workbook.Worksheets.Add("User Role Allocation");
                sheets.Add(sheetMain);

                short lineCount = 0;

                // Header Section
                lineCount++;
                sheetMain.Cells[lineCount, 1].Value = "Report Date";
                sheetMain.Cells[lineCount, 1].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 2].Value = string.Format("{0}", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

                // Title Section
                lineCount++;
                sheetMain.Cells[lineCount, 1].Value = "User Role Allocation Report";
                sheetMain.Cells[lineCount, 1, lineCount, 3].Merge = true;
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Bold = true;
                sheetMain.Cells[lineCount, 1, lineCount, 3].Style.Font.Size = 18;
                lineCount++;

                sheetMain.Cells[lineCount, 1].Value = "User";
                sheetMain.Cells[lineCount, 2].Value = "Role";
                sheetMain.Cells[lineCount, 3].Value = "Active/Inactive";
                sheetMain.Cells[lineCount, 4].Value = "Privelage";
                sheetMain.Cells[lineCount, 5].Value = "Active/Inactive";

                lineCount++;

                for (int i = 0; i < users.Count; i++)
                {
                    sheetMain.Cells[lineCount, 1].Value = users[i].Fullname.Trim().Equals("") ? users[i].AccountName : users[i].Fullname;
                    var userRoles = users[i].Roles.OrderBy(role => role.RoleName).ToList();

                    if (users[i].IsSystemAdmin)
                    {
                        userRoles.Add(
                            new Role()
                            {
                                RoleName = "System Administrator",
                                Privileges = privelages,
                                isActive = true
                            });
                    }

                    if (!includeInactiveRoles)
                    {
                        userRoles = userRoles.Where(ur => ur.isActive).ToList();
                    }

                    for (int j = 0; j < userRoles.Count; j++)
                    {
                        var rolePriv = userRoles[j].Privileges.OrderBy(priv => priv.Description).ToList();
                        sheetMain.Cells[lineCount, 1].Value = users[i].Fullname.Trim().Equals("") ? users[i].AccountName : users[i].Fullname;
                        sheetMain.Cells[lineCount, 2].Value = userRoles[j].RoleName;
                        sheetMain.Cells[lineCount, 3].Value = userRoles[j].isActive;

                        for (int k = 0; k < rolePriv.Count; k++)
                        {
                            sheetMain.Cells[lineCount, 1].Value = users[i].Fullname.Trim().Equals("") ? users[i].AccountName : users[i].Fullname;
                            sheetMain.Cells[lineCount, 2].Value = userRoles[j].RoleName;
                            sheetMain.Cells[lineCount, 3].Value = users[i].Active;
                            sheetMain.Cells[lineCount, 4].Value = rolePriv[k].Description;
                            sheetMain.Cells[lineCount, 5].Value = userRoles[j].isActive;
                            if (rolePriv.Count > (k + 1))
                                lineCount++;
                        }

                        if (userRoles.Count > (j + 1))
                            lineCount++;

                    }

                    lineCount++;
                }

                AutoWidthColumns(ref sheetMain);
                return pck.GetAsByteArray();
            }
        }

        #endregion User

        #region Timesheets

        public void printRespurceRates(ref ExcelWorksheet sheet, List<Guid> persons, DateTime startdate, DateTime enddate)
        {
            int rowIndex = 1;
            sheet.Cells[rowIndex, 1].Value = "Resource";
            sheet.Cells[rowIndex, 2].Value = "Rate StartDate";
            sheet.Cells[rowIndex, 3].Value = "Rate EndDate";
            sheet.Cells[rowIndex, 4].Value = "Rate";
            using (var rng = sheet.Cells[rowIndex, 1, rowIndex, 4])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }

            rowIndex++;
            foreach (var user in DataContext.UserAccountSet.Include(u => u.BillingRates).Where(u => persons.Contains(u.Id)).OrderBy(u => u.FirstName))
            {
                sheet.Cells[rowIndex, 1].Value = user.FirstName + " " + user.Surname;
                foreach (var rate in user.BillingRates.OrderBy(r => r.StartDate))
                {
                    if (startdate.CompareTo(rate.EndDate) <= 0 &&
                        rate.StartDate.CompareTo(enddate) <= 0)
                    {
                        sheet.Cells[rowIndex, 2].Value = rate.StartDate.ToShortDateString();
                        sheet.Cells[rowIndex, 3].Value = rate.EndDate.ToShortDateString();
                        sheet.Cells[rowIndex, 4].Value = rate.Rate;
                        sheet.Cells[rowIndex, 4].Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                        rowIndex++;
                    }
                }
                rowIndex++;
            }
            AutoWidthColumns(ref sheet);
        }

        public void printTotalRow(ref ExcelWorksheet sheetMain, int tableDataRow, List<decimal> data, int userCount, Color backGroudColor, Color textColor, bool doCurrencyFormat = false)
        {
            using (var rng = sheetMain.Cells[tableDataRow, 3, tableDataRow, userCount + 6])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; // Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(backGroudColor); // Set color to dark blue
                rng.Style.Font.Color.SetColor(textColor);

                if (doCurrencyFormat)
                    rng.Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
            }

            decimal rowTotal = 0;
            for (int i = 0; i < userCount; i++)
            {
                sheetMain.Cells[tableDataRow, i + 6].Value = data[i];
                rowTotal += data[i];
            }
            sheetMain.Cells[tableDataRow, userCount + 6].Value = rowTotal;
        }

        public String filterClientListForClientReporter(String clients)
        {
            List<String> AlllowedclientIds = DataContext.ClientReporterSet.Where(u => u.UserAccountId == CurrentUser.Id).Select(a => a.ClientId.ToString()).ToList();
            if (clients == "" || clients == "All")
            {
                clients = string.Join(",", AlllowedclientIds);
            }
            else
            {
                foreach (string clientid in clients.Split(','))
                {
                    if (!AlllowedclientIds.Contains(clientid))
                        throw new GenericSecurityException("Client Not allowed!");
                }
            }
            if (clients == "" || clients == "All")
                throw new GenericSecurityException("Client Not allowed!");

            return clients;
        }

        public byte[] GenerateTimesheetSummaryClientReporter(DateTime startDate, DateTime endDate,
                        String userAccounts, String clients, String projects, String projectWildCardSearch, bool showPhases)
        {
            Authenticate(PrivilegeType.CustomerReportAccess);

            clients = filterClientListForClientReporter(clients);

            return GenerateTimesheetSummaryWorker(startDate, endDate, userAccounts, clients, projects, projectWildCardSearch, false, false, showPhases, true);
        }

        public byte[] GenerateTimesheetSummaryOld(DateTime startDate, DateTime endDate,
            String userAccounts, String clients, String projects, String projectWildCardSearch, String employers,
            bool showBillingCycle, bool showRates, bool showPhases, bool showOnlyBillbale = false)
        {
            Authenticate(PrivilegeType.ReportGenerationTimesheet);

            return GenerateTimesheetSummaryWorker(startDate, endDate, userAccounts, clients, projects, projectWildCardSearch, showBillingCycle, showRates, showPhases, showOnlyBillbale);
        }

        public byte[] GenerateTimesheetSummary(DateTime startDate, DateTime endDate,
            String userAccounts, String clients, String projects, String projectWildCardSearch, String employers, bool showUnassigned,
            bool showBillingCycle, bool showRates, bool showPhases, bool showOnlyBillbale = false)
        {
            Authenticate(PrivilegeType.ReportGenerationTimesheet);

            return GenerateTimesheetSummaryWithEmployersWorker(startDate, endDate, userAccounts, clients, projects, projectWildCardSearch, employers, showUnassigned, showBillingCycle, showRates, showPhases, showOnlyBillbale);
        }

        public byte[] GenerateTimesheetSummaryWorker(DateTime startDate, DateTime endDate,
            String userAccounts, String clients, String projects, String projectWildCardSearch,
            bool showBillingCycle, bool showRates, bool showPhases, bool showOnlyBillbale = false)
        {
            try
            {
                using (var pck = new ExcelPackage())
                {
                    // Create the worksheet
                    var sheets = new List<ExcelWorksheet>();
                    var sheetMain = pck.Workbook.Worksheets.Add("Timesheet Summary");
                    sheets.Add(sheetMain);

                    var ratesSheet = pck.Workbook.Worksheets.Add("Rates");
                    if (showRates)
                    {
                        sheets.Add(ratesSheet);
                    }
                    else
                    {
                        pck.Workbook.Worksheets.Delete(2); // Rates Worksheet Index = 2
                    }

                    var repEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0).AddDays(1);
                    var repStartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

                    var timesheetStoreProc = DataContext.ExecuteTimesheetReportProcedure(repStartDate, repEndDate,
                        showPhases, showOnlyBillbale, userAccounts == null ? "All" : userAccounts,
                        clients == null ? "All" : clients,
                        projects == null ? "All" : projects,
                        projectWildCardSearch);

                    if (showRates)
                    {
                        List<Guid> persons = timesheetStoreProc.Select(a => a.UserAccountIdGuid).Distinct().ToList();
                        printRespurceRates(ref ratesSheet, persons, startDate, endDate);
                    }

                    SortedList<int, PivotedTimesheetRow> timesheetRows = new SortedList<int, PivotedTimesheetRow>();
                    SortedList<int, string> users = new SortedList<int, string>();
                    List<string> userTeamNames = new List<string>();
                    List<Guid> userIds = new List<Guid>();

                    int i = 0;
                    PivotedTimesheetRow row = null;
                    string currentProject = "";
                    string currentPhase = "";
                    int j = 0;
                    bool userArrayCreated = false;
                    foreach (var record in timesheetStoreProc.ToList())
                    {
                        if (!record.ProjectName.Equals(currentProject) || !record.PhaseName.Equals(currentPhase)) // Project name change
                        {
                            currentProject = record.ProjectName;
                            currentPhase = record.PhaseName;
                            if (row != null)
                            {
                                userArrayCreated = true;
                            }

                            row = new PivotedTimesheetRow();
                            row.Billable = record.Billable;
                            row.Client = record.Client;
                            row.PhaseName = record.PhaseName;
                            row.ProjectName = record.ProjectName;
                            row.ProjectTypeName = record.ProjectType;
                            row.SubProjectTypeName = record.SubProjectType;

                            timesheetRows.Add(i++, row);
                            j = 0;
                        }
                        if (!userArrayCreated)
                        {
                            users.Add(j, record.Person);
                            userTeamNames.Add(record.CurrentClientName);
                            userIds.Add(record.UserAccountIdGuid);
                        }

                        row.hours.Add(record.Hours);
                        row.cost.Add(record.Cost);
                        j++;
                    }

                    // Rules on where Data Exists
                    var headerDescriptionStart = 1;
                    var tableHeaderStart = 4;
                    var tableDataRow = 5;

                    // Print Timesheet Summary Headings
                    sheetMain.Cells[headerDescriptionStart++, 1].Value = "TRIZ All Employee Timesheet Summary ";
                    printDateRangeHeading(startDate, endDate, ref sheetMain, 2, 1);

                    sheetMain.Cells[headerDescriptionStart + 1, 1].Value = string.Format("Date Generated : {0} ", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

                    if (!projectWildCardSearch.Equals("*"))
                    {
                        sheetMain.Cells[headerDescriptionStart + 2, 1].Value = string.Format("Project / Subproject Search Text : {0} ", projectWildCardSearch);
                        tableHeaderStart++;
                    }

                    if (tableHeaderStart <= headerDescriptionStart)
                        tableHeaderStart = headerDescriptionStart + 2;

                    sheetMain.Cells[tableHeaderStart, 1].Value = "Billable";
                    sheetMain.Cells[tableHeaderStart, 2].Value = "Client";
                    sheetMain.Cells[tableHeaderStart, 3].Value = "Project Description & Code";
                    sheetMain.Cells[tableHeaderStart, 4].Value = "Project Level";
                    sheetMain.Cells[tableHeaderStart, 5].Value = "Project Type";

                    // Build Table
                    var headerColumns = 6;
                    foreach (var user in users)
                        sheetMain.Cells[tableHeaderStart, headerColumns++].Value = user.Value;
                    var userCount = users.Count();
                    sheetMain.Cells[tableHeaderStart, headerColumns++].Value = "Total";
                    using (var rng = sheetMain.Cells[tableHeaderStart, 1, tableHeaderStart, headerColumns - 1])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    // Billable ------------------------------------------------------------------------------ START
                    if (tableDataRow <= tableHeaderStart)
                        tableDataRow = tableHeaderStart + 1;
                    var firstRecord = true;
                    var clientName = "";
                    var prevBillable = true;

                    List<decimal> projectUserTotals = new List<decimal>();
                    List<decimal> projectUserTotalsCost = new List<decimal>();
                    List<decimal> billableHoursTotal = new List<decimal>();
                    List<decimal> billableCostTotal = new List<decimal>();
                    List<decimal> nonBillableCostTotal = new List<decimal>();
                    List<decimal> nonBillableHoursTotal = new List<decimal>();

                    // Efficiency Totals
                    List<decimal> flexTotals = new List<decimal>();
                    List<decimal> nonInvoiceableTotals = new List<decimal>();
                    List<decimal> adminTotals = new List<decimal>();
                    List<decimal> leaveVacationTotals = new List<decimal>();
                    List<decimal> leaveSickTotals = new List<decimal>();
                    List<decimal> leaveStudyTotals = new List<decimal>();
                    List<decimal> systemIssueTotals = new List<decimal>();
                    List<decimal> trainingTotals = new List<decimal>();
                    List<decimal> nonElegibleTotals = new List<decimal>();

                    bool printClientName = false;
                    bool printNonBillable = false;
                    for (i = 0; i < userCount; i++)
                    {
                        projectUserTotals.Add(0);
                        projectUserTotalsCost.Add(0);
                        billableHoursTotal.Add(0);
                        billableCostTotal.Add(0);
                        nonBillableCostTotal.Add(0);
                        nonBillableHoursTotal.Add(0);

                        flexTotals.Add(0);
                        nonInvoiceableTotals.Add(0);
                        adminTotals.Add(0);
                        leaveVacationTotals.Add(0);
                        leaveSickTotals.Add(0);
                        leaveStudyTotals.Add(0);
                        systemIssueTotals.Add(0);
                        trainingTotals.Add(0);
                        nonElegibleTotals.Add(0);
                    }

                    var billableFlexProjects = new List<PivotedTimesheetRow>();
                    foreach (var item in timesheetRows)
                    {
                        if (item.Value.Billable && (item.Value.ProjectTypeName == "Flex Engineering" || item.Value.SubProjectTypeName == "Flex Engineering"))
                        {
                            billableFlexProjects.Add(new PivotedTimesheetRow()
                            {
                                Billable = item.Value.Billable,
                                Client = item.Value.Client,
                                ProjectName = item.Value.ProjectName,
                                PhaseName = item.Value.PhaseName,
                                ProjectTypeName = item.Value.ProjectTypeName,
                                SubProjectTypeName = item.Value.SubProjectTypeName,
                                Hours = item.Value.Hours,
                                Cost = item.Value.Cost,
                                cost = new List<decimal>(item.Value.cost),
                                hours = new List<decimal>(item.Value.hours)
                            });
                        }
                    }

                    decimal rowTotal = 0;
                    sheetMain.Cells[tableDataRow, 1].Value = "Yes";
                    var tsRows = timesheetRows.ToArray();
                    int index = 0;
                    while (index < tsRows.Length)
                    {
                        var tsrow = tsRows[index];
                        printClientName = false;
                        printNonBillable = false;
                        if (tsrow.Value.Client != clientName) { printClientName = true; }
                        if (prevBillable != tsrow.Value.Billable) { printNonBillable = true; printClientName = true; }

                        //print totals
                        if (!firstRecord && printClientName)
                        {
                            sheetMain.Cells[tableDataRow, 3].Value = "Total " + clientName + " Hours";
                            printTotalRow(ref sheetMain, tableDataRow, projectUserTotals, userCount, Color.FromArgb(200, 200, 200), Color.Black);
                            tableDataRow++;

                            if (showRates)
                            {
                                sheetMain.Cells[tableDataRow, 3].Value = "Total " + clientName + " Cost";
                                printTotalRow(ref sheetMain, tableDataRow, projectUserTotalsCost, userCount, Color.FromArgb(225, 225, 225), Color.Black, true);
                                tableDataRow++;
                            }

                            for (i = 0; i < userCount; i++)
                            {
                                if (prevBillable)
                                {
                                    billableHoursTotal[i] += projectUserTotals[i];
                                    billableCostTotal[i] += projectUserTotalsCost[i];
                                }
                                else
                                {
                                    nonBillableHoursTotal[i] += projectUserTotals[i];
                                    nonBillableCostTotal[i] += projectUserTotalsCost[i];
                                }

                                projectUserTotals[i] = 0;
                                projectUserTotalsCost[i] = 0;
                            }
                        }
                        clientName = tsrow.Value.Client;
                        firstRecord = false;

                        if (printClientName)
                        {
                            if (printNonBillable)
                            {
                                //Do Billable Totals
                                sheetMain.Cells[tableDataRow, 3].Value = "Total Billable Hours";
                                printTotalRow(ref sheetMain, tableDataRow, billableHoursTotal, userCount, Color.FromArgb(50, 50, 50), Color.White);
                                tableDataRow++;

                                if (showRates)
                                {
                                    sheetMain.Cells[tableDataRow, 3].Value = "Total Billable Cost";
                                    printTotalRow(ref sheetMain, tableDataRow, billableCostTotal, userCount, Color.FromArgb(50, 50, 50), Color.White, true);
                                    tableDataRow++;
                                }
                                sheetMain.Cells[tableDataRow, 1].Value = "No";
                            }
                            sheetMain.Cells[tableDataRow, 2].Value = tsrow.Value.Client;
                        }
                        sheetMain.Cells[tableDataRow, 3].Value = tsrow.Value.ProjectName;
                        sheetMain.Cells[tableDataRow, 4].Value = "Project";
                        sheetMain.Cells[tableDataRow, 5].Value = tsrow.Value.ProjectTypeName;

                        decimal sumHours = 0;
                        var numPhases = 0;
                        //print subdetail for phases
                        if (!tsrow.Value.PhaseName.Trim().Equals(""))
                        {
                            using (var rng = sheetMain.Cells[tableDataRow, 2, tableDataRow, userCount + USER_COL_OFFSET])
                            {
                                rng.Style.Font.Bold = true;
                            }

                            var phasePosIndex = index;
                            while (phasePosIndex < tsRows.Length && tsRows[phasePosIndex].Value.ProjectName.Equals(tsrow.Value.ProjectName))
                            {
                                numPhases++;
                                sumHours = 0;
                                sheetMain.Cells[tableDataRow + numPhases, 3].Value = "      " + tsRows[phasePosIndex].Value.PhaseName;
                                sheetMain.Cells[tableDataRow + numPhases, 4].Value = "Sub-Project";
                                sheetMain.Cells[tableDataRow + numPhases, 5].Value = tsRows[phasePosIndex].Value.SubProjectTypeName;
                                for (i = 0; i < userCount; i++)
                                {
                                    if (tsRows[phasePosIndex].Value.hours[i] != 0)
                                        sheetMain.Cells[tableDataRow + numPhases, i + USER_COL_OFFSET].Value = tsRows[phasePosIndex].Value.hours[i];
                                    sumHours += tsRows[phasePosIndex].Value.hours[i];

                                    if (phasePosIndex != index)
                                    {
                                        tsRows[index].Value.hours[i] += tsRows[phasePosIndex].Value.hours[i];
                                        tsRows[index].Value.cost[i] += tsRows[phasePosIndex].Value.cost[i];
                                    }
                                }
                                sheetMain.Cells[tableDataRow + numPhases, userCount + USER_COL_OFFSET].Value = sumHours;

                                using (var rng = sheetMain.Cells[tableDataRow + numPhases, 3, tableDataRow + numPhases, userCount + USER_COL_OFFSET])
                                {
                                    rng.Style.Font.Color.SetColor(Color.Red);
                                }

                                phasePosIndex++;
                            }
                        }

                        sumHours = 0;
                        for (i = 0; i < userCount; i++)
                        {
                            if (tsrow.Value.hours[i] != 0)
                            {
                                sheetMain.Cells[tableDataRow, i + USER_COL_OFFSET].Value = tsrow.Value.hours[i];
                                projectUserTotals[i] += tsrow.Value.hours[i];
                                sumHours += tsrow.Value.hours[i];

                                // Increment efficiency calc totals
                                int type = tsrow.Value.PhaseName.Trim() == "" ? determineEfficeincyType(tsrow.Value.ProjectTypeName) : determineEfficeincyType(tsrow.Value.SubProjectTypeName);
                                if (type != 0)
                                {
                                    switch (type)
                                    {
                                        case 1:
                                            leaveStudyTotals[i] += tsrow.Value.hours[i];
                                            break;

                                        case 2:
                                            flexTotals[i] += tsrow.Value.hours[i];
                                            break;

                                        case 3:
                                            systemIssueTotals[i] += tsrow.Value.hours[i];
                                            break;

                                        case 4:
                                            adminTotals[i] += tsrow.Value.hours[i];
                                            break;

                                        case 5:
                                            leaveVacationTotals[i] += tsrow.Value.hours[i];
                                            break;

                                        case 6:
                                            trainingTotals[i] += tsrow.Value.hours[i];
                                            break;

                                        case 7:
                                            nonInvoiceableTotals[i] += tsrow.Value.hours[i];
                                            break;

                                        case 8:
                                            leaveSickTotals[i] += tsrow.Value.hours[i];
                                            break;

                                        case 9:
                                            nonElegibleTotals[i] += tsrow.Value.hours[i];
                                            break;
                                    }
                                }
                            }

                            if (tsrow.Value.cost[i] != 0)
                            {
                                projectUserTotalsCost[i] += tsrow.Value.cost[i];
                            }
                        }

                        sheetMain.Cells[tableDataRow, userCount + USER_COL_OFFSET].Value = sumHours;
                        tableDataRow++;
                        prevBillable = tsrow.Value.Billable;
                        index++;
                        if (numPhases > 0)
                        {
                            tableDataRow = tableDataRow + numPhases;
                            index = index + numPhases - 1;
                        }
                    }

                    //Do Client Totals
                    sheetMain.Cells[tableDataRow, 3].Value = "Total " + clientName + " Hours";
                    printTotalRow(ref sheetMain, tableDataRow, projectUserTotals, userCount, Color.FromArgb(200, 200, 200), Color.Black);
                    tableDataRow++;
                    if (showRates)
                    {
                        sheetMain.Cells[tableDataRow, 3].Value = "Total " + clientName + " Cost";
                        printTotalRow(ref sheetMain, tableDataRow, projectUserTotalsCost, userCount, Color.FromArgb(225, 225, 225), Color.Black, true);
                        tableDataRow++;
                    }

                    for (i = 0; i < userCount; i++)
                    {
                        if (prevBillable)
                        {
                            billableHoursTotal[i] += projectUserTotals[i];
                            billableCostTotal[i] += projectUserTotalsCost[i];
                        }
                        else
                        {
                            nonBillableHoursTotal[i] += projectUserTotals[i];
                            nonBillableCostTotal[i] += projectUserTotalsCost[i];
                        }

                        projectUserTotals[i] = 0;
                        projectUserTotalsCost[i] = 0;
                    }
                    tableDataRow++;

                    if (tsRows.Length > 0 && tsRows[tsRows.Length - 1].Value.Billable) // If there was non-billable entries
                    {
                        //Do Billable Totals
                        sheetMain.Cells[tableDataRow, 3].Value = "Total Billable Hours";
                        printTotalRow(ref sheetMain, tableDataRow, billableHoursTotal, userCount, Color.FromArgb(50, 50, 50), Color.White);
                        tableDataRow++;

                        if (showRates)
                        {
                            sheetMain.Cells[tableDataRow, 3].Value = "Total Billable Cost";
                            printTotalRow(ref sheetMain, tableDataRow, billableCostTotal, userCount, Color.FromArgb(50, 50, 50), Color.White, true);
                            tableDataRow++;
                        }
                    }
                    else
                    {
                        //Do Non-Billable Totals
                        sheetMain.Cells[tableDataRow, 3].Value = "Total Non Billable Hours";
                        printTotalRow(ref sheetMain, tableDataRow, nonBillableHoursTotal, userCount, Color.FromArgb(50, 50, 50), Color.White);
                        tableDataRow++;
                        if (showRates)
                        {
                            sheetMain.Cells[tableDataRow, 3].Value = "Total Non Billable Cost";
                            printTotalRow(ref sheetMain, tableDataRow, nonBillableCostTotal, userCount, Color.FromArgb(50, 50, 50), Color.White, true);
                            tableDataRow++;
                        }
                    }

                    if (showBillingCycle)
                    {
                        // Additional Summary Info
                        printAdditionalSummaryInfo(ref sheetMain, startDate, endDate, ref tableDataRow, ref rowTotal, userCount, ref index, users, userTeamNames, showBillingCycle, billableHoursTotal, nonBillableHoursTotal, nonElegibleTotals, flexTotals, leaveVacationTotals, leaveSickTotals);
                        // Efficiency Calcs
                        printEfficiencyCalcs(ref sheetMain, startDate, endDate, ref tableDataRow, userIds, billableHoursTotal, billableCostTotal, nonBillableHoursTotal, nonElegibleTotals, flexTotals, leaveStudyTotals, systemIssueTotals, adminTotals, leaveVacationTotals, trainingTotals, nonInvoiceableTotals, leaveSickTotals);
                        // Flex Engineering section
                        printFlexSection(ref sheetMain, ref tableDataRow, users, billableFlexProjects, showPhases);
                    }

                    // Sheet wide Styles
                    AutoWidthColumns(ref sheetMain);

                    return pck.GetAsByteArray();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void PrintClientHours(ExcelWorksheet timesheet, List<TimesheetSummaryGroup> data, ref int row, Dictionary<Guid, Project> projectDict, Dictionary<Guid, SubProject> subProjectDict, Dictionary<Guid, ProjectType> projectTypesDict, Dictionary<Employer, List<UserAccount>> employerUsers, bool showBillingCycle, bool showRates, bool showPhases, int employerUserColumnCount)
        {
            int hourCol = 6;
            decimal clientHours = 0;
            decimal employerHours = 0;
            decimal hours = 0;

            foreach (var group in data)
            {
                // Client
                timesheet.Cells[row, 1].Value = group.Billable ? "Yes" : "No";
                timesheet.Cells[row, 2].Value = group.ClientName;

                // Project
                var groupedByProject = group.Entries.OrderBy(e => e.ProjectCode).ThenBy(e => e.ProjectName).GroupBy(e => e.ProjectId);
                decimal projectHours = 0;
                foreach (var projectGroup in groupedByProject)
                {
                    projectHours = 0;
                    timesheet.Cells[row, 3].Value = $"{projectDict[projectGroup.Key].ProjectNumber} : {projectDict[projectGroup.Key].ProjectName}";
                    timesheet.Cells[row, 4].Value = "Project";
                    timesheet.Cells[row, 5].Value = projectTypesDict[projectDict[projectGroup.Key].ProjectTypeId.Value].Name;

                    hourCol = 6;
                    foreach (var employer in employerUsers)
                    {
                        employerHours = 0;
                        hours = 0;
                        foreach (var user in employer.Value)
                        {
                            hours = projectGroup.ToList().Where(pg => pg.EmployerId == employer.Key.Id && pg.UserAccountId == user.Id).Sum(pg => pg.Hours);

                            // Only add values if they are not 0
                            if (hours != 0)
                            {
                                timesheet.Cells[row, hourCol].Value = hours;
                                projectHours += hours;
                                employerHours += hours;
                                clientHours += hours;
                            }

                            hourCol++;
                        }

                        // Add total row
                        if (employerHours > 0)
                            timesheet.Cells[row, hourCol].Value = employerHours;

                        hourCol += 2;
                    }

                    // Grand Total
                    if (projectHours > 0)
                        timesheet.Cells[row, hourCol].Value = projectHours;

                    // Sub-project
                    if (showPhases)
                    {
                        row++;
                        var groupedBySubProject = projectGroup.ToList().OrderBy(e => e.SubProjectCode).ThenBy(e => e.SubProjectNumber).ThenBy(e => e.ProjectName).GroupBy(e => e.SubProjectId);

                        if (groupedBySubProject.Where(g => g.Key != null).Count() > 0)
                        {
                            // Make cells bold
                            using (var rng = timesheet.Cells[row - 1, 3, row - 1, 6 + employerUserColumnCount])
                            {
                                rng.Style.Font.Bold = true;
                            }
                        }

                        decimal subProjectHours = 0;
                        foreach (var subProjectGroup in groupedBySubProject)
                        {
                            // Project group, not sub projects
                            if (subProjectGroup.Key != null)
                            {
                                subProjectHours = 0;
                                timesheet.Cells[row, 3].Value = $"      {projectDict[projectGroup.Key].ProjectNumber} : {subProjectDict[subProjectGroup.Key.Value].SubProjectNumber} {subProjectDict[subProjectGroup.Key.Value].ProjectName}";
                                timesheet.Cells[row, 4].Value = "Sub-Project";
                                timesheet.Cells[row, 5].Value = projectTypesDict[subProjectDict[subProjectGroup.Key.Value].SubProjectTypeId.Value].Name;

                                // Make cells red
                                using (var rng = timesheet.Cells[row, 3, row, 6 + employerUserColumnCount])
                                {
                                    rng.Style.Font.Color.SetColor(Color.Red);
                                }

                                hourCol = 6;
                                employerHours = 0;
                                hours = 0;
                                foreach (var employer in employerUsers)
                                {
                                    employerHours = 0;

                                    foreach (var user in employer.Value)
                                    {
                                        hours = subProjectGroup.ToList().Where(pg => pg.EmployerId == employer.Key.Id && pg.UserAccountId == user.Id).Sum(pg => pg.Hours);

                                        // Only add values if they are not 0
                                        if (hours != 0)
                                        {
                                            timesheet.Cells[row, hourCol].Value = hours;
                                            subProjectHours += hours;
                                            employerHours += hours;
                                        }

                                        hourCol++;
                                    }

                                    // Add total row
                                    if (employerHours > 0)
                                        timesheet.Cells[row, hourCol].Value = employerHours;

                                    hourCol += 2;
                                }

                                // Grand Total
                                if (subProjectHours > 0)
                                    timesheet.Cells[row, hourCol].Value = subProjectHours;

                                row++;
                            }
                        }
                    }
                    else
                    {
                        row++;
                    }
                }

                // Total Rows
                timesheet.Cells[row, 3].Value = $"Total {group.ClientName} Hours";

                // Make cells grey
                using (var rng = timesheet.Cells[row, 3, row, 6 + employerUserColumnCount])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200)); //Set color to dark blue
                }

                // Cost rows
                if (showRates)
                {
                    timesheet.Cells[row + 1, 3].Value = $"Total {group.ClientName} Cost";
                    // Make cells grey
                    using (var rng = timesheet.Cells[row + 1, 3, row + 1, 6 + employerUserColumnCount])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(225, 225, 225)); //Set color to dark blue
                        rng.Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                    }
                }

                hourCol = 6;
                hours = 0;
                employerHours = 0;
                var cost = decimal.Zero;
                var employerCost = decimal.Zero;
                var totalHours = decimal.Zero;
                var totalCost = decimal.Zero;

                IEnumerable<TimesheetSummaryEntry> entries;

                foreach (var employer in employerUsers)
                {
                    employerHours = 0;
                    employerCost = decimal.Zero;

                    foreach (var user in employer.Value)
                    {
                        entries = group.Entries.ToList().Where(pg => pg.EmployerId == employer.Key.Id && pg.UserAccountId == user.Id);

                        hours = entries.Sum(pg => pg.Hours);
                        cost = entries.Sum(pg => pg.Cost);

                        timesheet.Cells[row, hourCol].Value = hours;

                        if (showRates)
                            timesheet.Cells[row + 1, hourCol].Value = cost;

                        employerHours += hours;
                        employerCost += cost;
                        totalHours += hours;
                        totalCost += cost;
                        hourCol++;
                    }

                    // Add total row
                    timesheet.Cells[row, hourCol].Value = employerHours;
                    if (showRates)
                        timesheet.Cells[row + 1, hourCol].Value = employerCost;

                    hourCol += 2;
                }

                // Grand Total
                timesheet.Cells[row, hourCol].Value = totalHours;

                if (showRates)
                {
                    timesheet.Cells[row + 1, hourCol].Value = totalCost;
                    row++;
                }

                row++;
            }
        }

        private void PrintBillableTotals(ExcelWorksheet timesheet, Dictionary<HourBreakdown, List<HourBreakdown>> employerUsers, ref int row, bool showRates, bool isBillable, int employerUserColumnCount)
        {
            if (isBillable)
            {
                timesheet.Cells[row, 3].Value = "Total Billable Hours";
            }
            else
            {
                timesheet.Cells[row, 3].Value = "Total Non-Billable Hours";
            }

            // Make cells black
            using (var rng = timesheet.Cells[row, 3, row, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(Color.Black);
                rng.Style.Font.Color.SetColor(Color.White);
            }

            if (showRates)
            {
                if (isBillable)
                {
                    timesheet.Cells[row + 1, 3].Value = "Total Billable Cost";
                }
                else
                {
                    timesheet.Cells[row + 1, 3].Value = "Total Non-Billable Cost";
                }

                // Make cells grey
                using (var rng = timesheet.Cells[row + 1, 3, row + 1, 6 + employerUserColumnCount])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                }
            }

            var col = 6;
            var totalHours = decimal.Zero;
            var totalCost = decimal.Zero;

            if (isBillable)
            {
                foreach (var employer in employerUsers)
                {
                    foreach (var user in employer.Value)
                    {
                        timesheet.Cells[row, col].Value = user.BillableHours;

                        if (showRates)
                        {
                            timesheet.Cells[row + 1, col].Value = user.BillableCost;
                        }

                        col++;
                    }

                    timesheet.Cells[row, col].Value = employer.Key.BillableHours;

                    if (showRates)
                    {
                        timesheet.Cells[row + 1, col].Value = employer.Key.BillableCost;
                    }

                    col += 2;

                    totalHours += employer.Key.BillableHours;
                    totalCost += employer.Key.BillableCost;
                }

                timesheet.Cells[row, col].Value = totalHours;

                if (showRates)
                {
                    timesheet.Cells[row + 1, col].Value = totalCost;
                    row++;
                }
            }
            else
            {
                foreach (var employer in employerUsers)
                {
                    foreach (var user in employer.Value)
                    {
                        timesheet.Cells[row, col].Value = user.NonBillableHours;

                        if (showRates)
                        {
                            timesheet.Cells[row + 1, col].Value = user.NonBillableCost;
                        }

                        col++;
                    }

                    timesheet.Cells[row, col].Value = employer.Key.NonBillableHours;

                    if (showRates)
                    {
                        timesheet.Cells[row + 1, col].Value = employer.Key.NonBillableCost;
                    }

                    col += 2;

                    totalHours += employer.Key.NonBillableHours;
                    totalCost += employer.Key.NonBillableCost;
                }

                timesheet.Cells[row, col].Value = totalHours;

                if (showRates)
                {
                    timesheet.Cells[row + 1, col].Value = totalCost;
                    row++;
                }
            }

            row++;
        }

        private void PrintSummary(ExcelWorksheet timesheet, Dictionary<Employer, List<UserAccount>> employerUsers, Dictionary<HourBreakdown, List<HourBreakdown>> employerUserHourBreakdown, ref int row, int employerUserColumnCount, Dictionary<Guid, ClientEntity> clientsDict)
        {
            // Column headers
            row++;
            timesheet.Cells[row, 1].Value = "SUMMARY";

            using (var rng = timesheet.Cells[row, 1, row, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(50, 50, 50)); //Set color to dark grey
                rng.Style.Font.Color.SetColor(Color.White);
            }

            row += 3;
            timesheet.Cells[row, 2].Value = "Code";
            timesheet.Cells[row, 3].Value = "Employee";

            // Make cells blue
            using (var rng = timesheet.Cells[row - 2, 1, row, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }

            var col = 6;
            foreach (var employer in employerUsers)
            {
                timesheet.Cells[row - 2, col].Value = employer.Key.Name;

                // User names
                foreach (var user in employer.Value)
                {
                    using (var ctx = new DataContext())
                    {
                        var designation = ctx.TeamJobDesignationSet.Where(t => t.UserAccountId == user.Id && DateTime.Now >= t.StartDate && DateTime.Now <= t.EndDate).FirstOrDefault();

                        if (designation != null)
                        {
                            timesheet.Cells[row - 1, col].Value = clientsDict[designation.ClientId].EntityName;
                        }
                        else
                        {
                            timesheet.Cells[row - 1, col].Value = "None";
                        }
                    }

                    timesheet.Cells[row, col].Value = user.FirstName + " " + user.Surname;
                    col++;
                }

                timesheet.Cells[row, col].Value = "Total";
                col += 2;
            }

            // Grand Total
            timesheet.Cells[row, col].Value = "Grand Total";

            // col 2
            timesheet.Cells[row + 1, 2].Value = "B1";
            timesheet.Cells[row + 2, 2].Value = "B2";
            timesheet.Cells[row + 3, 2].Value = "A1";
            timesheet.Cells[row + 4, 2].Value = "A2";

            // col 3
            timesheet.Cells[row + 1, 3].Value = "Total Logged";
            timesheet.Cells[row + 2, 3].Value = "Total Billed";
            timesheet.Cells[row + 3, 3].Value = "Eligible Logged (All Logged excluding Non-Eligible)";
            timesheet.Cells[row + 4, 3].Value = "No. of Engineers";

            col = 6;
            var grandSummary = new HourBreakdown();
            foreach (var employer in employerUserHourBreakdown)
            {
                foreach (var user in employer.Value)
                {
                    timesheet.Cells[row + 1, col].Value = user.Hours;
                    timesheet.Cells[row + 2, col].Value = user.BillableHours - user.FlexHours;
                    timesheet.Cells[row + 3, col].Value = user.Hours - (user.NonEligibleHours + user.FlexHours);
                    timesheet.Cells[row + 4, col].Value = 1;

                    col++;
                }

                timesheet.Cells[row + 1, col].Value = employer.Key.Hours;
                timesheet.Cells[row + 2, col].Value = employer.Key.BillableHours - employer.Key.FlexHours;
                timesheet.Cells[row + 3, col].Value = employer.Key.Hours - (employer.Key.NonEligibleHours + employer.Key.FlexHours);
                timesheet.Cells[row + 4, col].Value = employer.Value.Count();

                grandSummary.Hours += employer.Key.Hours;
                grandSummary.BillableHours += employer.Key.BillableHours - employer.Key.FlexHours;
                grandSummary.EligibleHours += employer.Key.Hours - (employer.Key.NonEligibleHours + employer.Key.FlexHours);
                grandSummary.UserCount += employer.Value.Count();

                col += 2;
            }

            timesheet.Cells[row + 1, col].Value = grandSummary.Hours;
            timesheet.Cells[row + 2, col].Value = grandSummary.BillableHours;
            timesheet.Cells[row + 3, col].Value = grandSummary.EligibleHours;
            timesheet.Cells[row + 4, col].Value = grandSummary.UserCount;
        }

        private void PrintEfficeincySummary(ExcelWorksheet timesheet, Dictionary<Employer, List<UserAccount>> employerUsers, Dictionary<HourBreakdown, List<HourBreakdown>> employerUserHourBreakdown, BillingCycleEntry billingCycle, DateTime startDate, DateTime endDate, ref int row, int employerUserColumnCount, Dictionary<Guid, ClientEntity> clientsDict)
        {
            // Column headers
            row++;

            timesheet.Cells[row, 1].Value = "SUMMARY";
            using (var rng = timesheet.Cells[row, 1, row, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(50, 50, 50)); //Set color to dark grey
                rng.Style.Font.Color.SetColor(Color.White);
            }

            row += 3;
            timesheet.Cells[row, 2].Value = "Code";
            timesheet.Cells[row, 3].Value = "Employee";

            // Make cells blue
            using (var rng = timesheet.Cells[row - 2, 1, row, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }

            var col = 6;
            foreach (var employer in employerUsers)
            {
                timesheet.Cells[row - 2, col].Value = employer.Key.Name;

                // User names
                foreach (var user in employer.Value)
                {
                    using (var ctx = new DataContext())
                    {
                        var designation = ctx.TeamJobDesignationSet.Where(t => t.UserAccountId == user.Id && DateTime.Now >= t.StartDate && DateTime.Now <= t.EndDate).FirstOrDefault();

                        if (designation != null)
                        {
                            timesheet.Cells[row - 1, col].Value = clientsDict[designation.ClientId].EntityName;
                        }
                        else
                        {
                            timesheet.Cells[row - 1, col].Value = "None";
                        }
                    }

                    timesheet.Cells[row, col].Value = user.FirstName + " " + user.Surname;

                    col++;
                }

                timesheet.Cells[row, col].Value = "Total";
                col += 2;
            }

            // Grand Total
            timesheet.Cells[row, col].Value = "Grand Total";

            // Make cells blue
            using (var rng = timesheet.Cells[row - 1, 1, row, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }

            row++;

            // col 2
            timesheet.Cells[row, 2].Value = "B1";
            timesheet.Cells[row + 1, 2].Value = "B2";
            timesheet.Cells[row + 2, 2].Value = "B3";
            timesheet.Cells[row + 3, 2].Value = "A1";
            timesheet.Cells[row + 4, 2].Value = "A2";
            timesheet.Cells[row + 5, 2].Value = "A3";

            // col 3
            timesheet.Cells[row, 3].Value = "Total Logged";
            timesheet.Cells[row + 1, 3].Value = "Total Billed";
            timesheet.Cells[row + 2, 3].Value = "GVW Calender Financial Cycle Hours";
            timesheet.Cells[row + 3, 3].Value = "Eligible Logged (All Logged excluding Non-Eligible)";
            timesheet.Cells[row + 4, 3].Value = "No. of Engineers";
            timesheet.Cells[row + 5, 3].Value = "Month Available (Weekdays * 8 - Non-Eligible)";
            timesheet.Cells[row + 6, 3].Value = "Billing Budget Target (Total Billed / (Month Available - All Budgeted Leaves)) = B2/(A3-All Budgeted Leaves)";
            timesheet.Cells[row + 7, 3].Value = "Billing Efficiency (Total Billed / Month Available) = (B2/A3)";
            timesheet.Cells[row + 8, 3].Value = "Log Efficiency % Timesheet Score (Eligible Logged / Month Available) = (A1/A3)";
            timesheet.Cells[row + 9, 3].Value = "Log Efficiency (Average Timesheet Score Hours)";
            timesheet.Cells[row + 10, 3].Value = "Overall Billing Efficiency (Total Billed / Eligible Logged) = (B2 / A1)";
            timesheet.Cells[row + 11, 3].Value = "Vacation Leave (Pub Holidays & Annual) as % of Month Available";
            timesheet.Cells[row + 12, 3].Value = "Other Leaves (E.g. Sick, Study) as % of Month Available";
            timesheet.Cells[row + 13, 3].Value = "Admin (E.g. AES and other non - billable projects) as % Month Available";
            timesheet.Cells[row + 14, 3].Value = "Triz System Issues as % of Month Available";
            timesheet.Cells[row + 15, 3].Value = "Training as % of Month Available";
            timesheet.Cells[row + 16, 3].Value = "Non-Billable Engineering Quality and Efficiency as % of Month Available";
            timesheet.Cells[row + 17, 3].Value = "Additional Logged Hours (Above " + LOG_EFFICIENCY_THRESHOLD + " Hours)";
            timesheet.Cells[row + 18, 3].Value = "Additional Revenue";
            timesheet.Cells[row + 19, 3].Value = "All Budgeted Leaves";
            timesheet.Cells[row + 20, 3].Value = "Efficiency Sum Check";

            col = 6;
            var grandSummary = new HourBreakdown();

            decimal monthAvailable = 0;
            decimal eligibleLogged = 0;
            decimal billingBudgetTarget = 0;

            var billingEfficeincy = decimal.Zero;
            var leave = decimal.Zero;
            var leaveOther = decimal.Zero;
            var admin = decimal.Zero;
            var system = decimal.Zero;
            var training = decimal.Zero;
            var enigneeringQuality = decimal.Zero;
            var calcResult = decimal.Zero;
            var logEfficiency = decimal.Zero;
            var logEfficiencySum = decimal.Zero;

            foreach (var employer in employerUserHourBreakdown)
            {
                foreach (var user in employer.Value)
                {
                    // Reset for user
                    billingEfficeincy = decimal.Zero;
                    leave = decimal.Zero;
                    leaveOther = decimal.Zero;
                    admin = decimal.Zero;
                    system = decimal.Zero;
                    training = decimal.Zero;
                    enigneeringQuality = decimal.Zero;
                    calcResult = decimal.Zero;
                    logEfficiency = decimal.Zero;

                    monthAvailable = (billingCycle.Weekdays * LOG_EFFICIENCY_THRESHOLD) - user.NonEligibleHours;
                    eligibleLogged = user.Hours - (user.NonEligibleHours + user.FlexHours);

                    if (monthAvailable - (user.LeaveSickHours + user.LeaveVacationHours) == 0)
                    {
                        billingBudgetTarget = 0;
                    }
                    else
                    {
                        billingBudgetTarget = (user.BillableHours - user.FlexHours) / (monthAvailable - (user.LeaveSickHours + user.LeaveVacationHours));
                    }

                    timesheet.Cells[row, col].Value = user.Hours;
                    timesheet.Cells[row + 1, col].Value = user.BillableHours - user.FlexHours;
                    timesheet.Cells[row + 2, col].Value = billingCycle.Weekdays * LOG_EFFICIENCY_THRESHOLD;
                    timesheet.Cells[row + 3, col].Value = (user.Hours - user.FlexHours) - user.NonEligibleHours;
                    timesheet.Cells[row + 4, col].Value = 1;
                    timesheet.Cells[row + 5, col].Value = monthAvailable;

                    if (monthAvailable != 0)
                    {
                        logEfficiency = eligibleLogged == 0 ? 0 : decimal.Round(((eligibleLogged / monthAvailable) * LOG_EFFICIENCY_THRESHOLD), 2);

                        //Billing Budget Target (Total Billed / (Month Available - All Budgeted Leaves)) = B2/(A3-All Budgeted Leaves)
                        timesheet.Cells[row + 6, col].Value = billingBudgetTarget;

                        //Billing Efficiency(Total Billed / Month Available)  (B2 / A3)
                        timesheet.Cells[row + 7, col].Value = (user.BillableHours - user.FlexHours) / monthAvailable;

                        // Log Efficiency - Time sheet Score Hours (A1/A3) % (Eligible Logged / Month Available)
                        timesheet.Cells[row + 8, col].Value = eligibleLogged == 0 ? 0 : eligibleLogged / monthAvailable;

                        // Log Efficiency - Time sheet Score Hours
                        timesheet.Cells[row + 9, col].Value = eligibleLogged == 0 ? 0 : logEfficiency;

                        // Overall Billing Efficiency (Total Billed / Eligible Logged)  (B2/A1)
                        timesheet.Cells[row + 10, col].Value = eligibleLogged == 0 ? 0 : (user.BillableHours - user.FlexHours) / eligibleLogged;

                        // Vacation Leave (Pub Holidays & Annual) as % of (Month Available))
                        timesheet.Cells[row + 11, col].Value = user.LeaveVacationHours / monthAvailable;
                        timesheet.Cells[row + 11, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

                        // Other Leaves (Sick, Study) as % of (Month Available)
                        timesheet.Cells[row + 12, col].Value = (user.LeaveSickHours + user.LeaveStudyHours + user.LeaveOtherHours) / monthAvailable;
                        timesheet.Cells[row + 12, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

                        // Admin(Incl.AES and other non - billable projects) of Month Available
                        timesheet.Cells[row + 13, col].Value = user.AdminHours / monthAvailable;
                        timesheet.Cells[row + 13, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

                        // Triz System Issues / Not Billed as % of (Month Available))
                        timesheet.Cells[row + 14, col].Value = user.SystemIssueHours / monthAvailable;
                        timesheet.Cells[row + 14, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

                        // Training as % of Month Available
                        timesheet.Cells[row + 15, col].Value = user.TrainingHours / monthAvailable;
                        timesheet.Cells[row + 15, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

                        // Engineering Quality and Efficiency as % of Month Available
                        timesheet.Cells[row + 16, col].Value = user.NonInvoiceableHours / monthAvailable;
                        timesheet.Cells[row + 16, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

                        // Additional Logged Hours(Above LOG_EFFICIENCY_THRESHOLD Hours)
                        var extraHours = eligibleLogged - monthAvailable;
                        timesheet.Cells[row + 17, col].Value = extraHours;

                        var billingRate = DataContext.BillingRatesSet.Where(br => br.UserAccountId == user.UserAccountId && startDate.CompareTo(br.EndDate) <= 0 && br.StartDate.CompareTo(endDate) <= 0).FirstOrDefault();
                        decimal rate = billingRate == null ? 0 : billingRate.Rate;

                        // Additional Revenue
                        timesheet.Cells[row + 18, col].Value = rate * extraHours;
                        timesheet.Cells[row + 18, col].Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";

                        // Set value to red, meaning there was no rate for the user
                        if (rate == 0)
                        {
                            timesheet.Cells[row + 18, col].Style.Font.Color.SetColor(Color.Red);
                        }

                        // All Budgeted Leaves
                        timesheet.Cells[row + 19, col].Value = user.LeaveSickHours + user.LeaveVacationHours;

                        // Efficiency Sum Check
                        logEfficiencySum = eligibleLogged == 0 ? 0 : eligibleLogged / monthAvailable;

                        billingEfficeincy = (user.BillableHours - user.FlexHours) / monthAvailable;
                        leave = user.LeaveVacationHours / monthAvailable;
                        leaveOther = (user.LeaveSickHours + user.LeaveStudyHours + user.LeaveOtherHours) / monthAvailable;
                        admin = user.AdminHours / monthAvailable;
                        system = user.SystemIssueHours / monthAvailable;
                        training = user.TrainingHours / monthAvailable;
                        enigneeringQuality = user.NonInvoiceableHours / monthAvailable;
                        calcResult = (billingEfficeincy + leave + leaveOther + admin + system + training + enigneeringQuality) - logEfficiencySum;

                        timesheet.Cells[row + 20, col].Value = calcResult;
                        timesheet.Cells[row + 20, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

                        // Totals
                        employer.Key.TotalAdditionalRevenue += rate * extraHours;
                    }
                    else
                    {
                        // Make all 0
                        for (int k = 0; k < 15; k++)
                        {
                            timesheet.Cells[row + 6 + k, col].Value = 0;
                        }
                    }

                    // Format
                    timesheet.Cells[row + 6, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 7, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 8, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 10, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 11, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 12, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 13, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 14, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 15, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 16, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    timesheet.Cells[row + 18, col].Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                    timesheet.Cells[row + 20, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

                    employer.Key.MonthAvailable += monthAvailable;
                    employer.Key.EligibleHours += eligibleLogged;

                    col++;
                }

                timesheet.Cells[row, col].Value = employer.Key.Hours;
                timesheet.Cells[row + 1, col].Value = employer.Key.BillableHours - employer.Key.FlexHours;
                timesheet.Cells[row + 2, col].Value = (billingCycle.Weekdays * LOG_EFFICIENCY_THRESHOLD * employer.Value.Count());
                timesheet.Cells[row + 3, col].Value = employer.Key.EligibleHours;
                timesheet.Cells[row + 4, col].Value = employer.Value.Count();
                timesheet.Cells[row + 5, col].Value = employer.Key.MonthAvailable;

                if (employer.Key.MonthAvailable != 0)
                {
                    timesheet.Cells[row + 6, col].Value = (employer.Key.BillableHours - employer.Key.FlexHours) / (employer.Key.MonthAvailable - (employer.Key.LeaveSickHours + employer.Key.LeaveVacationHours));
                    timesheet.Cells[row + 7, col].Value = (employer.Key.BillableHours - employer.Key.FlexHours) / employer.Key.MonthAvailable;
                    timesheet.Cells[row + 8, col].Value = employer.Key.EligibleHours == 0 ? 0 : employer.Key.EligibleHours / employer.Key.MonthAvailable;
                    timesheet.Cells[row + 9, col].Value = employer.Key.EligibleHours == 0 ? 0 : decimal.Round(((employer.Key.EligibleHours / employer.Key.MonthAvailable) * LOG_EFFICIENCY_THRESHOLD), 2);
                    timesheet.Cells[row + 10, col].Value = employer.Key.EligibleHours == 0 ? 0 : (employer.Key.BillableHours - employer.Key.FlexHours) / employer.Key.EligibleHours;
                    timesheet.Cells[row + 11, col].Value = employer.Key.LeaveVacationHours / employer.Key.MonthAvailable;
                    timesheet.Cells[row + 12, col].Value = (employer.Key.LeaveSickHours + employer.Key.LeaveStudyHours + employer.Key.LeaveOtherHours) / employer.Key.MonthAvailable;
                    timesheet.Cells[row + 13, col].Value = employer.Key.AdminHours / employer.Key.MonthAvailable;
                    timesheet.Cells[row + 14, col].Value = employer.Key.SystemIssueHours / employer.Key.MonthAvailable;
                    timesheet.Cells[row + 15, col].Value = employer.Key.TrainingHours / employer.Key.MonthAvailable;
                    timesheet.Cells[row + 16, col].Value = employer.Key.NonInvoiceableHours / employer.Key.MonthAvailable;
                    timesheet.Cells[row + 17, col].Value = employer.Key.EligibleHours - employer.Key.MonthAvailable;
                    timesheet.Cells[row + 18, col].Value = employer.Key.TotalAdditionalRevenue;
                    timesheet.Cells[row + 19, col].Value = employer.Key.LeaveSickHours + employer.Key.LeaveVacationHours;

                    // Efficiency Sum Check
                    logEfficiencySum = employer.Key.EligibleHours == 0 ? 0 : employer.Key.EligibleHours / employer.Key.MonthAvailable;

                    billingEfficeincy = (employer.Key.BillableHours - employer.Key.FlexHours) / employer.Key.MonthAvailable;
                    leave = employer.Key.LeaveVacationHours / employer.Key.MonthAvailable;
                    leaveOther = (employer.Key.LeaveSickHours + employer.Key.LeaveStudyHours + employer.Key.LeaveOtherHours) / employer.Key.MonthAvailable;
                    admin = employer.Key.AdminHours / employer.Key.MonthAvailable;
                    system = employer.Key.SystemIssueHours / employer.Key.MonthAvailable;
                    training = employer.Key.TrainingHours / employer.Key.MonthAvailable;
                    enigneeringQuality = employer.Key.NonInvoiceableHours / employer.Key.MonthAvailable;
                    calcResult = (billingEfficeincy + leave + leaveOther + admin + system + training + enigneeringQuality) - logEfficiencySum;

                    timesheet.Cells[row + 20, col].Value = calcResult;
                }
                else
                {
                    // Make all 0
                    for (int k = 0; k < 15; k++)
                    {
                        timesheet.Cells[row + 6 + k, col].Value = 0;
                    }
                }

                // Format
                timesheet.Cells[row + 6, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 7, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 8, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 10, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 11, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 12, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 13, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 14, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 15, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 16, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                timesheet.Cells[row + 18, col].Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                timesheet.Cells[row + 20, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

                // Grand summary
                grandSummary.Hours += employer.Key.Hours;
                grandSummary.BillableHours += employer.Key.BillableHours - employer.Key.FlexHours;
                grandSummary.MonthAvailable += employer.Key.MonthAvailable;
                grandSummary.EligibleHours += employer.Key.EligibleHours;
                grandSummary.UserCount += employer.Value.Count();
                grandSummary.AdminHours += employer.Key.AdminHours;
                grandSummary.SystemIssueHours += employer.Key.SystemIssueHours;
                grandSummary.TrainingHours += employer.Key.TrainingHours;
                grandSummary.NonInvoiceableHours += employer.Key.NonInvoiceableHours;
                grandSummary.LeaveVacationHours += employer.Key.LeaveVacationHours;
                grandSummary.LeaveSickHours += employer.Key.LeaveSickHours;
                grandSummary.LeaveStudyHours += employer.Key.LeaveStudyHours;
                grandSummary.LeaveOtherHours += employer.Key.LeaveOtherHours;
                grandSummary.TotalAdditionalRevenue += employer.Key.TotalAdditionalRevenue;

                col += 2;
            }

            timesheet.Cells[row, col].Value = grandSummary.Hours;
            timesheet.Cells[row + 1, col].Value = grandSummary.BillableHours - grandSummary.FlexHours;
            timesheet.Cells[row + 2, col].Value = (billingCycle.Weekdays * LOG_EFFICIENCY_THRESHOLD * grandSummary.UserCount);
            timesheet.Cells[row + 3, col].Value = grandSummary.EligibleHours;
            timesheet.Cells[row + 4, col].Value = grandSummary.UserCount;
            timesheet.Cells[row + 5, col].Value = grandSummary.MonthAvailable;

            timesheet.Cells[row + 6, col].Value = (grandSummary.BillableHours - grandSummary.FlexHours) / (grandSummary.MonthAvailable - (grandSummary.LeaveSickHours + grandSummary.LeaveVacationHours));
            timesheet.Cells[row + 7, col].Value = (grandSummary.BillableHours - grandSummary.FlexHours) / grandSummary.MonthAvailable;
            timesheet.Cells[row + 8, col].Value = grandSummary.EligibleHours == 0 ? 0 : grandSummary.EligibleHours / grandSummary.MonthAvailable;
            timesheet.Cells[row + 9, col].Value = grandSummary.EligibleHours == 0 ? 0 : decimal.Round(((grandSummary.EligibleHours / grandSummary.MonthAvailable) * LOG_EFFICIENCY_THRESHOLD), 2);
            timesheet.Cells[row + 10, col].Value = grandSummary.EligibleHours == 0 ? 0 : (grandSummary.BillableHours - grandSummary.FlexHours) / grandSummary.EligibleHours;
            timesheet.Cells[row + 11, col].Value = grandSummary.LeaveVacationHours / grandSummary.MonthAvailable;
            timesheet.Cells[row + 12, col].Value = (grandSummary.LeaveSickHours + grandSummary.LeaveStudyHours + grandSummary.LeaveOtherHours) / grandSummary.MonthAvailable;
            timesheet.Cells[row + 13, col].Value = grandSummary.AdminHours / grandSummary.MonthAvailable;
            timesheet.Cells[row + 14, col].Value = grandSummary.SystemIssueHours / grandSummary.MonthAvailable;
            timesheet.Cells[row + 15, col].Value = grandSummary.TrainingHours / grandSummary.MonthAvailable;
            timesheet.Cells[row + 16, col].Value = grandSummary.NonInvoiceableHours / grandSummary.MonthAvailable;
            timesheet.Cells[row + 17, col].Value = grandSummary.EligibleHours - grandSummary.MonthAvailable;
            timesheet.Cells[row + 18, col].Value = grandSummary.TotalAdditionalRevenue;
            timesheet.Cells[row + 19, col].Value = grandSummary.LeaveSickHours + grandSummary.LeaveVacationHours;

            // Efficiency Sum Check
            logEfficiencySum = grandSummary.EligibleHours == 0 ? 0 : grandSummary.EligibleHours / grandSummary.MonthAvailable;

            billingEfficeincy = (grandSummary.BillableHours - grandSummary.FlexHours) / grandSummary.MonthAvailable;
            leave = grandSummary.LeaveVacationHours / grandSummary.MonthAvailable;
            leaveOther = (grandSummary.LeaveSickHours + grandSummary.LeaveStudyHours + grandSummary.LeaveOtherHours) / grandSummary.MonthAvailable;
            admin = grandSummary.AdminHours / grandSummary.MonthAvailable;
            system = grandSummary.SystemIssueHours / grandSummary.MonthAvailable;
            training = grandSummary.TrainingHours / grandSummary.MonthAvailable;
            enigneeringQuality = grandSummary.NonInvoiceableHours / grandSummary.MonthAvailable;
            calcResult = (billingEfficeincy + leave + leaveOther + admin + system + training + enigneeringQuality) - logEfficiencySum;

            timesheet.Cells[row + 20, col].Value = calcResult;

            // Format
            timesheet.Cells[row + 6, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 7, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 8, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 9, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 11, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 12, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 13, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 14, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 15, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 16, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;
            timesheet.Cells[row + 18, col].Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
            timesheet.Cells[row + 20, col].Style.Numberformat.Format = PERCENTAGE_FORMAT;

            /////////////////
            //// STYLING ////
            /////////////////

            // Billing Budget Target (Total Billed / (Month Available - All Budgeted Leaves)) B2/(A3-All Budgeted Leaves)
            using (var rng = timesheet.Cells[row + 6, 6, row + 6, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Color.SetColor(Color.FromArgb(0, 112, 192));
            }

            using (var rng = timesheet.Cells[row + 7, 6, row + 7, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Color.SetColor(Color.FromArgb(55, 86, 35));
            }

            // Admin(Incl.AES and other non - billable projects) of Month Available
            using (var rng = timesheet.Cells[row + 13, 6, row + 13, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
            }

            // Training as % of Month Available
            using (var rng = timesheet.Cells[row + 15, 6, row + 15, 6 + employerUserColumnCount])
            {
                rng.Style.Font.Color.SetColor(Color.FromArgb(122, 48, 160));
            }

            // Additional Logged Hours(Above 8 Hours)
            // Additional Revenue
            timesheet.Cells[row + 18, 3].Style.Font.Bold = true;
            timesheet.Cells[row + 19, 3].Style.Font.Bold = true;
            using (var rng = timesheet.Cells[row + 17, 3, row + 18, 6 + employerUserColumnCount])
            {
                rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
        }

        private void PrintFlexSummary(ExcelWorksheet timesheet, Dictionary<Employer, List<UserAccount>> employerUsers, List<TimesheetSummaryGroup> data, Dictionary<HourBreakdown, List<HourBreakdown>> employerUserHourBreakdown, Dictionary<Guid, Project> projectDict, Dictionary<Guid, SubProject> subProjectDict, Dictionary<Guid, ProjectType> projectTypesDict, List<Guid> projectTypes, BillingCycleEntry billingCycle, DateTime startDate, DateTime endDate, ref int row, bool showPhases, bool showRates, int employerUserColumnCount)
        {
            try
            {
                // Filter on flex
                var filteredData = data;

                // Delete entries where not flex
                foreach (var group in filteredData)
                {
                    group.Entries = group.Entries.Where(e => (e.SubProjectTypeId != null && e.SubProjectTypeId.Value == projectTypes[1]) || (e.ProjectTypeId != null && e.ProjectTypeId.Value == projectTypes[1])).ToList();
                }

                // Delete client groups that have no entries
                filteredData.RemoveAll(g => g.Entries.Count == 0);

                if (filteredData.Count() > 0)
                {
                    if (billingCycle != null)
                    {
                        row += 22;
                    }
                    else
                    {
                        // No flex calcs
                        row += 6;
                    }

                    timesheet.Cells[row, 1].Value = "FLEX ENGINEERING: Billable Only";
                    using (var rng = timesheet.Cells[row, 1, row, 6 + employerUserColumnCount])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(50, 50, 50)); //Set color to dark grey
                        rng.Style.Font.Color.SetColor(Color.White);
                    }
                    row += 2;

                    // Headers
                    timesheet.Cells[row, 2].Value = "Client";
                    timesheet.Cells[row, 3].Value = "Project Description & Code";
                    timesheet.Cells[row, 4].Value = "Project Level";
                    timesheet.Cells[row, 5].Value = "Project Type";

                    int col = 6;
                    foreach (var employer in employerUsers)
                    {
                        timesheet.Cells[row - 1, col].Value = employer.Key.Name;

                        // User names
                        foreach (var user in employer.Value)
                        {
                            timesheet.Cells[row, col].Value = user.FirstName + " " + user.Surname;
                            col++;
                        }

                        timesheet.Cells[row, col].Value = "Total";
                        col += 2;
                    }

                    // Grand Total
                    timesheet.Cells[row, col].Value = "Grand Total";

                    // Make cells blue
                    using (var rng = timesheet.Cells[row - 1, 1, row, 6 + employerUserColumnCount])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    row++;

                    var hourCol = 6;
                    var clientHours = decimal.Zero;
                    var employerHours = decimal.Zero;
                    var hours = decimal.Zero;
                    foreach (var group in filteredData)
                    {
                        // Client
                        timesheet.Cells[row, 2].Value = group.ClientName;

                        // Project
                        var groupedByProject = group.Entries.OrderBy(e => e.ProjectCode).ThenBy(e => e.ProjectName).GroupBy(e => e.ProjectId);
                        decimal projectHours = 0;
                        foreach (var projectGroup in groupedByProject)
                        {
                            projectHours = 0;
                            timesheet.Cells[row, 3].Value = $"{projectDict[projectGroup.Key].ProjectNumber} : {projectDict[projectGroup.Key].ProjectName}";
                            timesheet.Cells[row, 4].Value = "Project";
                            timesheet.Cells[row, 5].Value = projectTypesDict[projectDict[projectGroup.Key].ProjectTypeId.Value].Name;

                            hourCol = 6;
                            foreach (var employer in employerUsers)
                            {
                                employerHours = 0;
                                hours = 0;
                                foreach (var user in employer.Value)
                                {
                                    hours = projectGroup.ToList().Where(pg => pg.EmployerId == employer.Key.Id && pg.UserAccountId == user.Id).Sum(pg => pg.Hours);

                                    // Only add values if they are not 0
                                    if (hours != 0)
                                    {
                                        timesheet.Cells[row, hourCol].Value = hours;
                                        projectHours += hours;
                                        employerHours += hours;
                                        clientHours += hours;
                                    }

                                    hourCol++;
                                }

                                // Add total row
                                if (employerHours > 0)
                                    timesheet.Cells[row, hourCol].Value = employerHours;

                                hourCol += 2;
                            }

                            // Grand Total
                            if (projectHours > 0)
                                timesheet.Cells[row, hourCol].Value = projectHours;

                            // Sub-project
                            if (showPhases)
                            {
                                row++;
                                var groupedBySubProject = projectGroup.ToList().OrderBy(e => e.SubProjectCode).ThenBy(e => e.SubProjectNumber).ThenBy(e => e.ProjectName).GroupBy(e => e.SubProjectId);

                                if (groupedBySubProject.Where(g => g.Key != null).Count() > 0)
                                {
                                    // Make cells bold
                                    using (var rng = timesheet.Cells[row - 1, 3, row - 1, 6 + employerUserColumnCount])
                                    {
                                        rng.Style.Font.Bold = true;
                                    }
                                }

                                decimal subProjectHours = 0;
                                foreach (var subProjectGroup in groupedBySubProject)
                                {
                                    // Project group, not sub projects
                                    if (subProjectGroup.Key != null)
                                    {
                                        subProjectHours = 0;
                                        timesheet.Cells[row, 3].Value = $"      {projectDict[projectGroup.Key].ProjectNumber} : {subProjectDict[subProjectGroup.Key.Value].SubProjectNumber} {subProjectDict[subProjectGroup.Key.Value].ProjectName}";
                                        timesheet.Cells[row, 4].Value = "Sub-Project";
                                        timesheet.Cells[row, 5].Value = projectTypesDict[subProjectDict[subProjectGroup.Key.Value].SubProjectTypeId.Value].Name;

                                        // Make cells red
                                        using (var rng = timesheet.Cells[row, 3, row, 6 + employerUserColumnCount])
                                        {
                                            rng.Style.Font.Color.SetColor(Color.Red);
                                        }

                                        hourCol = 6;
                                        employerHours = 0;
                                        hours = 0;
                                        foreach (var employer in employerUsers)
                                        {
                                            employerHours = 0;

                                            foreach (var user in employer.Value)
                                            {
                                                hours = subProjectGroup.ToList().Where(pg => pg.EmployerId == employer.Key.Id && pg.UserAccountId == user.Id).Sum(pg => pg.Hours);

                                                // Only add values if they are not 0
                                                if (hours != 0)
                                                {
                                                    timesheet.Cells[row, hourCol].Value = hours;
                                                    subProjectHours += hours;
                                                    employerHours += hours;
                                                }

                                                hourCol++;
                                            }

                                            // Add total row
                                            if (employerHours > 0)
                                                timesheet.Cells[row, hourCol].Value = employerHours;

                                            hourCol += 2;
                                        }

                                        // Grand Total
                                        if (subProjectHours > 0)
                                            timesheet.Cells[row, hourCol].Value = subProjectHours;

                                        row++;
                                    }
                                }
                            }
                            else
                            {
                                row++;
                            }
                        }

                        // Total Rows
                        timesheet.Cells[row, 3].Value = $"Total {group.ClientName} Hours";

                        // Make cells grey
                        using (var rng = timesheet.Cells[row, 3, row, 6 + employerUserColumnCount])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                            rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200)); //Set color to dark blue
                        }

                        // Cost rows
                        if (showRates)
                        {
                            timesheet.Cells[row + 1, 3].Value = $"Total {group.ClientName} Cost";
                            // Make cells grey
                            using (var rng = timesheet.Cells[row + 1, 3, row + 1, 6 + employerUserColumnCount])
                            {
                                rng.Style.Font.Bold = true;
                                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(225, 225, 225)); //Set color to dark blue
                                rng.Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                            }
                        }

                        hourCol = 6;
                        hours = 0;
                        employerHours = 0;
                        var cost = decimal.Zero;
                        var employerCost = decimal.Zero;
                        var totalHours = decimal.Zero;
                        var totalCost = decimal.Zero;

                        IEnumerable<TimesheetSummaryEntry> entries;

                        foreach (var employer in employerUsers)
                        {
                            employerHours = 0;
                            employerCost = decimal.Zero;

                            foreach (var user in employer.Value)
                            {
                                entries = group.Entries.ToList().Where(pg => pg.EmployerId == employer.Key.Id && pg.UserAccountId == user.Id);

                                hours = entries.Sum(pg => pg.Hours);
                                cost = entries.Sum(pg => pg.Cost);

                                timesheet.Cells[row, hourCol].Value = hours;

                                if (showRates)
                                    timesheet.Cells[row + 1, hourCol].Value = cost;

                                employerHours += hours;
                                employerCost += cost;
                                totalHours += hours;
                                totalCost += cost;
                                hourCol++;
                            }

                            // Add total row
                            timesheet.Cells[row, hourCol].Value = employerHours;
                            if (showRates)
                                timesheet.Cells[row + 1, hourCol].Value = employerCost;

                            hourCol += 2;
                        }

                        // Grand Total
                        timesheet.Cells[row, hourCol].Value = totalHours;

                        if (showRates)
                        {
                            timesheet.Cells[row + 1, hourCol].Value = totalCost;
                            row++;
                        }

                        row++;
                    }

                    timesheet.Cells[row, 3].Value = "Total Flex Hours";
                    // Make cells black
                    using (var rng = timesheet.Cells[row, 3, row, 6 + employerUserColumnCount])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(Color.Black);
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    col = 6;
                    var grandSummary = new HourBreakdown();
                    foreach (var employer in employerUserHourBreakdown)
                    {
                        foreach (var user in employer.Value)
                        {
                            timesheet.Cells[row, col].Value = user.FlexHours;
                            col++;
                        }

                        timesheet.Cells[row, col].Value = employer.Key.FlexHours;

                        grandSummary.FlexHours += employer.Key.FlexHours;

                        col += 2;
                    }

                    timesheet.Cells[row, col].Value = grandSummary.FlexHours;

                    if (showRates)
                    {
                        row++;
                        timesheet.Cells[row, 3].Value = "Total Flex Cost";
                        // Make cells black
                        using (var rng = timesheet.Cells[row, 3, row, 6 + employerUserColumnCount])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rng.Style.Fill.BackgroundColor.SetColor(Color.Black);
                            rng.Style.Font.Color.SetColor(Color.White);
                            rng.Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                        }

                        col = 6;

                        grandSummary = new HourBreakdown();
                        var totalCost = decimal.Zero;
                        foreach (var employer in employerUserHourBreakdown)
                        {
                            foreach (var user in employer.Value)
                            {
                                timesheet.Cells[row, col].Value = user.FlexCost;
                                col++;
                            }

                            timesheet.Cells[row, col].Value = employer.Key.FlexCost;

                            totalCost += employer.Key.FlexCost;

                            col += 2;
                        }

                        timesheet.Cells[row, col].Value = totalCost;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("printFlexSection failed, " + e.Message);
            }
        }

        public byte[] GenerateTimesheetSummaryWithEmployersWorker(DateTime startDate, DateTime endDate,
        String userAccounts, String clients, String projects, String projectWildCardSearch, String employers, bool showUnassigned,
        bool showBillingCycle, bool showRates, bool showPhases, bool showOnlyBillbale = false)
        {
            try
            {
                using (var pck = new ExcelPackage())
                {
                    // Create the worksheet
                    var sheets = new List<ExcelWorksheet>();

                    var timesheet = pck.Workbook.Worksheets.Add("Timesheet Summary");
                    sheets.Add(timesheet);

                    // Normalise start and end date
                    var repEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0).AddDays(1);
                    var repStartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

                    // Filter params
                    var f_accounts = userAccounts != null && userAccounts != "All" ? userAccounts.Split(',').Select(Guid.Parse).ToList() : new List<Guid>();
                    var f_clients = clients != null && clients != "All" ? clients.Split(',').Select(Guid.Parse).ToList() : new List<Guid>();
                    var f_employers = employers != null && employers != "All" ? employers.Split(',').Select(Guid.Parse).ToList() : new List<Guid>();
                    var f_projects = projects != null && projects != "All" ? projects.Split(',').Select(Guid.Parse).ToList() : new List<Guid>();

                    var usersDict = DataContext.UserAccountSet.ToDictionary(a => a.Id, a => a);
                    var clientsDict = DataContext.ClientEntitySet.ToDictionary(a => a.Id, a => a);
                    var employersDict = DataContext.EmployerSet.ToDictionary(a => a.Id, a => a);
                    var projectDict = DataContext.ProjectSet.ToDictionary(a => a.Id, a => a);
                    var subProjectDict = DataContext.SubProjectSet.ToDictionary(a => a.Id, a => a);
                    var projectTypes = DataContext.ProjectTypeSet.OrderBy(pt => pt.SortOrder).Select(pt => pt.Id).ToList();
                    var projectTypesDict = DataContext.ProjectTypeSet.OrderBy(pt => pt.SortOrder).ToDictionary(a => a.Id, a => a);

                    // Get timesheet data
                    var timesheetDataRaw = from ts in DataContext.TimesheetEntrySet

                                           join p in DataContext.ProjectSet on ts.ProjectId equals p.Id

                                           join c in DataContext.ClientEntitySet on p.ClientId equals c.Id

                                           where ts.DateEntry >= repStartDate && ts.DateEntry < repEndDate && ts.Hours > 0
                                           select new
                                           {
                                               ClientId = c.Id,
                                               ClientName = c.EntityName,
                                               Billable = p.Billable,
                                               UserAccountId = ts.UserAccountId,
                                               ProjectId = ts.ProjectId,
                                               SubProjectId = ts.SubProjectId,
                                               ProjectTypeId = p.ProjectTypeId,
                                               Hours = ts.Hours,
                                               DateEntry = ts.DateEntry,
                                           };

                    // Filter
                    if (f_accounts.Count > 0)
                    {
                        timesheetDataRaw = timesheetDataRaw.Where(a => f_accounts.Contains(a.UserAccountId));
                    }

                    if (f_clients.Count > 0)
                    {
                        timesheetDataRaw = timesheetDataRaw.Where(a => f_clients.Contains(a.ClientId));
                    }

                    if (f_projects.Count > 0)
                    {
                        timesheetDataRaw = timesheetDataRaw.Where(a => f_projects.Contains(a.ProjectId));
                    }

                    // Group
                    var timesheetData = (from tdr in timesheetDataRaw
                                         group tdr by new { tdr.ClientId, tdr.ClientName, tdr.Billable } into tsGrouped

                                         orderby tsGrouped.Key.Billable descending, tsGrouped.Key.ClientName

                                         select new TimesheetSummaryGroup()
                                         {
                                             ClientId = tsGrouped.Key.ClientId,
                                             ClientName = tsGrouped.Key.ClientName,
                                             Billable = tsGrouped.Key.Billable,
                                             Entries = tsGrouped.Select(e => new TimesheetSummaryEntry
                                             {
                                                 UserAccountId = e.UserAccountId,
                                                 ProjectId = e.ProjectId,
                                                 SubProjectId = e.SubProjectId,
                                                 EmployerId = Guid.Empty,
                                                 ProjectTypeId = e.ProjectTypeId,
                                                 Hours = e.Hours,
                                                 Rate = 0,
                                                 Cost = 0,
                                                 DateEntry = e.DateEntry
                                             }).ToList()
                                         }).ToList();

                    // Enrich Data
                    var timesheetEmployerGuids = new List<Guid>();
                    bool addedBlank = false;
                    using (var ctx = new DataContext())
                    {
                        foreach (var group in timesheetData)
                        {
                            foreach (var entry in group.Entries)
                            {
                                var designation = ctx.TeamJobDesignationSet.FirstOrDefault(tj => tj.UserAccountId == entry.UserAccountId && entry.DateEntry >= tj.StartDate && entry.DateEntry <= tj.EndDate);

                                if (designation != null)
                                {
                                    entry.EmployerId = designation.EmployerId.Value;

                                    if (entry.EmployerId != null)
                                    {
                                        if (!timesheetEmployerGuids.Contains(entry.EmployerId.Value))
                                        {
                                            timesheetEmployerGuids.Add(entry.EmployerId.Value);
                                        }
                                    }

                                    // Set employer ID for unassigned entries
                                    if (entry.EmployerId == null)
                                    {
                                        entry.EmployerId = Guid.Empty;
                                    }
                                }
                                else
                                {
                                    if (!addedBlank)
                                    {
                                        timesheetEmployerGuids.Add(Guid.Empty);
                                        addedBlank = true;
                                    }
                                }

                                // Set project name
                                entry.ProjectName = projectDict[entry.ProjectId].ProjectName;
                                entry.ProjectCode = projectDict[entry.ProjectId].ProjectNumber;

                                if (entry.SubProjectId != null)
                                {
                                    entry.SubProjectName = subProjectDict[entry.SubProjectId.Value].ProjectName;
                                    entry.SubProjectNumber = subProjectDict[entry.SubProjectId.Value].SubProjectNumber;
                                    entry.SubProjectTypeId = subProjectDict[entry.SubProjectId.Value].SubProjectTypeId;
                                }

                                // Set billing rate
                                var billingRate = ctx.BillingRatesSet.FirstOrDefault(br => br.UserAccountId == entry.UserAccountId && entry.DateEntry >= br.StartDate && entry.DateEntry <= br.EndDate);

                                if (billingRate != null)
                                {
                                    entry.Rate = billingRate.Rate;
                                    entry.Cost = entry.Hours * entry.Rate;
                                }
                            }
                        }
                    }

                    // Filter on project Name
                    if (!projectWildCardSearch.Equals("*"))
                    {
                        // Delete where not in list
                        foreach (var group in timesheetData)
                        {
                            group.Entries = group.Entries.Where(e => e.ProjectName.Contains(projectWildCardSearch) || (e.SubProjectName != null && e.SubProjectName.Contains(projectWildCardSearch))).ToList();
                        }

                        // Delete client groups that have no entries
                        timesheetData.RemoveAll(g => g.Entries.Count == 0);
                    }

                    if (!showUnassigned)
                    {
                        // Delete where not in list
                        foreach (var group in timesheetData)
                        {
                            group.Entries = group.Entries.Where(e => e.EmployerId != Guid.Empty).ToList();
                        }

                        // Delete client groups that have no entries
                        timesheetData.RemoveAll(g => g.Entries.Count == 0);

                        // Remove from timesheetEmployerGuids
                        timesheetEmployerGuids.RemoveAll(e => e == Guid.Empty);
                    }

                    // Filter on employer
                    if (f_employers.Count > 0)
                    {
                        if (showUnassigned)
                            f_employers.Add(Guid.Empty);

                        // Delete where not in list
                        foreach (var group in timesheetData)
                        {
                            group.Entries = group.Entries.Where(e => f_employers.Contains(e.EmployerId.Value)).ToList();
                        }

                        // Delete client groups that have no entries
                        timesheetData.RemoveAll(g => g.Entries.Count == 0);

                        // Remove from timesheetEmployerGuids
                        timesheetEmployerGuids.RemoveAll(e => !f_employers.Contains(e));
                    }

                    var timesheetEmployers = new List<Employer>();
                    // Build employer list
                    foreach (var item in timesheetEmployerGuids)
                    {
                        if (item != Guid.Empty)
                        {
                            timesheetEmployers.Add(employersDict[item]);
                        }
                        else
                        {
                            timesheetEmployers.Add(new Employer()
                            {
                                Id = Guid.Empty,
                                Name = "Unassigned"
                            });
                        }
                    }
                    // Order by name
                    timesheetEmployers = timesheetEmployers.OrderBy(e => e.Name).ToList();

                    // Move unnasigned to end
                    if (timesheetEmployers.FindIndex(e => e.Id == Guid.Empty) > -1)
                    {
                        var index = timesheetEmployers.FindIndex(e => e.Id == Guid.Empty);
                        var item = timesheetEmployers[index];
                        timesheetEmployers.RemoveAt(index);
                        timesheetEmployers.Add(item);
                    }

                    // Build unique user list per employer
                    var employerUsers = new Dictionary<Employer, List<UserAccount>>();
                    var uniqueUsersIds = new List<Guid>();
                    var users = new List<UserAccount>();
                    foreach (var item in timesheetEmployers)
                    {
                        users = new List<UserAccount>();

                        foreach (var group in timesheetData)
                        {
                            foreach (var entry in group.Entries)
                            {
                                if (entry.EmployerId.Value == item.Id && !users.Contains(usersDict[entry.UserAccountId]))
                                {
                                    users.Add(usersDict[entry.UserAccountId]);
                                }

                                if (!uniqueUsersIds.Contains(entry.UserAccountId))
                                {
                                    uniqueUsersIds.Add(entry.UserAccountId);
                                }
                            }
                        }

                        users = users.OrderBy(u => u.FirstName).ThenBy(g => g.Surname).ToList();
                        employerUsers.Add(item, users);
                    }

                    // Build employer/user hour breakdown
                    Dictionary<HourBreakdown, List<HourBreakdown>> employerUserHourBreakdown = new Dictionary<HourBreakdown, List<HourBreakdown>>();
                    var employerBreakdown = new HourBreakdown();
                    var breakDownList = new List<HourBreakdown>();
                    var breakdown = new HourBreakdown();

                    foreach (var employer in employerUsers)
                    {
                        employerBreakdown = new HourBreakdown()
                        {
                            EmployerId = employer.Key.Id
                        };

                        breakDownList = new List<HourBreakdown>();

                        foreach (var user in employer.Value)
                        {
                            breakdown = new HourBreakdown()
                            {
                                UserAccountId = user.Id
                            };

                            foreach (var client in timesheetData)
                            {
                                var entries = client.Entries.Where(c => c.EmployerId == employer.Key.Id && c.UserAccountId == user.Id);
                                var hours = entries.Sum(e => e.Hours);
                                var cost = entries.Sum(e => e.Cost);

                                breakdown.Hours += hours;
                                breakdown.Cost += cost;

                                if (client.Billable)
                                {
                                    breakdown.BillableHours += hours;
                                    breakdown.BillableCost += cost;
                                }
                                else
                                {
                                    breakdown.NonBillableHours += hours;
                                    breakdown.NonBillableCost += cost;
                                }

                                // Set efficiency values
                                breakdown.AdminHours += entries.Where(e => e.ProjectTypeId == projectTypes[0] || e.SubProjectTypeId == projectTypes[0]).Sum(e => e.Hours);
                                breakdown.FlexHours += entries.Where(e => e.ProjectTypeId == projectTypes[1] || e.SubProjectTypeId == projectTypes[1]).Sum(e => e.Hours);
                                breakdown.LeaveSickHours += entries.Where(e => e.ProjectTypeId == projectTypes[2] || e.SubProjectTypeId == projectTypes[2]).Sum(e => e.Hours);
                                breakdown.LeaveStudyHours += entries.Where(e => e.ProjectTypeId == projectTypes[3] || e.SubProjectTypeId == projectTypes[3]).Sum(e => e.Hours);
                                breakdown.LeaveVacationHours += entries.Where(e => e.ProjectTypeId == projectTypes[4] || e.SubProjectTypeId == projectTypes[4]).Sum(e => e.Hours);
                                breakdown.LeaveOtherHours += entries.Where(e => e.ProjectTypeId == projectTypes[5] || e.SubProjectTypeId == projectTypes[5]).Sum(e => e.Hours);
                                breakdown.NonEligibleHours += entries.Where(e => e.ProjectTypeId == projectTypes[7] || e.SubProjectTypeId == projectTypes[7]).Sum(e => e.Hours);
                                breakdown.NonInvoiceableHours += entries.Where(e => e.ProjectTypeId == projectTypes[8] || e.SubProjectTypeId == projectTypes[8]).Sum(e => e.Hours);
                                breakdown.SystemIssueHours += entries.Where(e => e.ProjectTypeId == projectTypes[9] || e.SubProjectTypeId == projectTypes[9]).Sum(e => e.Hours);
                                breakdown.TrainingHours += entries.Where(e => e.ProjectTypeId == projectTypes[10] || e.SubProjectTypeId == projectTypes[10]).Sum(e => e.Hours);

                                // Flex
                                breakdown.FlexCost += entries.Where(e => e.ProjectTypeId == projectTypes[1] || e.SubProjectTypeId == projectTypes[1]).Sum(e => e.Cost);
                            }

                            // Update employer values
                            breakDownList.Add(breakdown);

                            employerBreakdown.Hours += breakdown.Hours;
                            employerBreakdown.Cost += breakdown.Cost;
                            employerBreakdown.BillableHours += breakdown.BillableHours;
                            employerBreakdown.BillableCost += breakdown.BillableCost;
                            employerBreakdown.NonBillableHours += breakdown.NonBillableHours;
                            employerBreakdown.NonBillableCost += breakdown.NonBillableCost;

                            // Flex
                            employerBreakdown.FlexCost += breakdown.FlexCost;

                            // Efficiency
                            employerBreakdown.SystemIssueHours += breakdown.SystemIssueHours;
                            employerBreakdown.LeaveVacationHours += breakdown.LeaveVacationHours;
                            employerBreakdown.NonInvoiceableHours += breakdown.NonInvoiceableHours;
                            employerBreakdown.TrainingHours += breakdown.TrainingHours;
                            employerBreakdown.LeaveStudyHours += breakdown.LeaveStudyHours;
                            employerBreakdown.FlexHours += breakdown.FlexHours;
                            employerBreakdown.NonEligibleHours += breakdown.NonEligibleHours;
                            employerBreakdown.AdminHours += breakdown.AdminHours;
                            employerBreakdown.LeaveSickHours += breakdown.LeaveSickHours;
                            employerBreakdown.LeaveOtherHours += breakdown.LeaveOtherHours;
                        }

                        employerUserHourBreakdown.Add(employerBreakdown, breakDownList);
                    }

                    // Rate Sheet
                    if (showRates)
                    {
                        var ratesSheet = pck.Workbook.Worksheets.Add("Rates");
                        sheets.Add(ratesSheet);

                        printRespurceRates(ref ratesSheet, uniqueUsersIds, repStartDate, repEndDate);
                    }

                    // Start Printing
                    int headerRow = 1;
                    timesheet.Cells[headerRow, 1].Value = "TRIZ All Employee Timesheet Summary ";
                    printDateRangeHeading(startDate, endDate, ref timesheet, ++headerRow, 1);

                    // Project search logic
                    if (!projectWildCardSearch.Equals("*"))
                    {
                        timesheet.Cells[++headerRow, 1].Value = string.Format("Project / Subproject Search Text : {0} ", projectWildCardSearch);
                    }

                    // Generated Date
                    timesheet.Cells[++headerRow, 1].Value = string.Format("Date Generated : {0} ", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

                    // Column headers
                    headerRow += 2;
                    timesheet.Cells[headerRow, 1].Value = "Billable";
                    timesheet.Cells[headerRow, 2].Value = "Client";
                    timesheet.Cells[headerRow, 3].Value = "Project Description & Code";
                    timesheet.Cells[headerRow, 4].Value = "Project Level";
                    timesheet.Cells[headerRow, 5].Value = "Project Type";

                    int col = 6;
                    int employerUserColumnCount = 0;
                    foreach (var employer in employerUsers)
                    {
                        timesheet.Cells[headerRow - 1, col].Value = employer.Key.Name;

                        // User names
                        foreach (var user in employer.Value)
                        {
                            timesheet.Cells[headerRow, col].Value = user.FirstName + " " + user.Surname;
                            col++;

                            employerUserColumnCount++;
                        }

                        timesheet.Cells[headerRow, col].Value = "Total";
                        col += 2;

                        employerUserColumnCount += 2;
                    }

                    // Grand Total
                    timesheet.Cells[headerRow, col].Value = "Grand Total";

                    // Make cells blue
                    using (var rng = timesheet.Cells[headerRow - 1, 1, headerRow, 6 + employerUserColumnCount])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    headerRow += 1;

                    var billable = timesheetData.Where(c => c.Billable).ToList();
                    this.PrintClientHours(timesheet, billable, ref headerRow, projectDict, subProjectDict, projectTypesDict, employerUsers, showBillingCycle, showRates, showPhases, employerUserColumnCount);
                    this.PrintBillableTotals(timesheet, employerUserHourBreakdown, ref headerRow, showRates, true, employerUserColumnCount);

                    if (!showOnlyBillbale)
                    {
                        var nonBillable = timesheetData.Where(c => !c.Billable).ToList();

                        if (nonBillable.Count > 0)
                        {
                            this.PrintClientHours(timesheet, nonBillable, ref headerRow, projectDict, subProjectDict, projectTypesDict, employerUsers, showBillingCycle, showRates, showPhases, employerUserColumnCount);
                            this.PrintBillableTotals(timesheet, employerUserHourBreakdown, ref headerRow, showRates, false, employerUserColumnCount);
                        }
                    }

                    // Summary
                    var billingCycle = DataContext.BillingCycleEntrySet.FirstOrDefault(a => a.StartDate == startDate && a.EndDate == endDate);
                    var multipleBillingCycles = DataContext.BillingCycleEntrySet.Where(a => a.StartDate >= startDate && a.EndDate <= endDate);

                    if (billingCycle == null && multipleBillingCycles.Count() == 0)
                    {
                        this.PrintSummary(timesheet, employerUsers, employerUserHourBreakdown, ref headerRow, employerUserColumnCount, clientsDict);
                    }
                    else
                    {
                        if (multipleBillingCycles.Count() > 0)
                        {
                            billingCycle = new BillingCycleEntry()
                            {
                                Weekdays = (short)multipleBillingCycles.Sum(b => b.Weekdays),
                                PublicHolidays = (short)multipleBillingCycles.Sum(b => b.PublicHolidays),
                                WorkDays = (short)multipleBillingCycles.Sum(b => b.WorkDays)
                            };
                        }

                        this.PrintEfficeincySummary(timesheet, employerUsers, employerUserHourBreakdown, billingCycle, startDate, endDate, ref headerRow, employerUserColumnCount, clientsDict);
                    }

                    if (showBillingCycle)
                    {
                        this.PrintFlexSummary(timesheet, employerUsers, billable, employerUserHourBreakdown, projectDict, subProjectDict, projectTypesDict, projectTypes, billingCycle, startDate, endDate, ref headerRow, showPhases, showRates, employerUserColumnCount);
                    }

                    // Sheet wide Styles
                    AutoWidthColumns(ref timesheet);

                    return pck.GetAsByteArray();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private int determineEfficeincyType(string projectTypeName)
        {
            switch (projectTypeName)
            {
                case "Leave (Study)":
                    return 1;

                case "Flex Engineering":
                    return 2;

                case "System":
                    return 3;

                case "Admin":
                    return 4;

                case "Leave (Vacation)":
                    return 5;

                case "Training":
                    return 6;

                case "Non-Invoiceable Engineering":
                    return 7;

                case "Leave (Sick)":
                    return 8;

                case "Non-Eligible":
                    return 9;

                default:
                    return 0;
            }
        }

        private void printAdditionalSummaryInfo(ref ExcelWorksheet sheetMain, DateTime startDate, DateTime endDate, ref int tableDataRow, ref decimal rowTotal,
            int userCount, ref int index, SortedList<int, string> users, List<string> userTeamNames, bool showBillingCycle,
            List<decimal> billableHoursTotal, List<decimal> nonBillableHoursTotal,
            List<decimal> nonElegibleTotals, List<decimal> flexTotals, List<decimal> leaveVacationTotals, List<decimal> leaveSickTotals)
        {
            try
            {
                sheetMain.Cells[tableDataRow, 1].Value = "SUMMARY";
                using (var rng = sheetMain.Cells[tableDataRow, 1, tableDataRow, userCount + USER_COL_OFFSET])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(50, 50, 50)); //Set color to dark grey
                    rng.Style.Font.Color.SetColor(Color.White);
                }
                tableDataRow++;

                sheetMain.Cells[tableDataRow, 2].Value = "Code";
                sheetMain.Cells[tableDataRow, 3].Value = "Employee";
                sheetMain.Cells[tableDataRow, userCount + USER_COL_OFFSET_SUMMARY].Value = "Total";

                // Set header Style
                using (var rng = sheetMain.Cells[tableDataRow, 1, tableDataRow, USER_COL_OFFSET_SUMMARY + users.Count])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                tableDataRow++;
                // Team Name/ Current Client Name
                sheetMain.Cells[tableDataRow, 3].Value = "Team";
                for (int i = 0; i < userTeamNames.Count(); i++)
                {
                    sheetMain.Cells[tableDataRow, i + USER_COL_OFFSET_SUMMARY].Value = userTeamNames[i];
                }

                tableDataRow++;
                sheetMain.Cells[tableDataRow, 2].Value = "B1";
                sheetMain.Cells[tableDataRow, 3].Value = "Total Logged";

                rowTotal = 0;
                for (int i = 0; i < userCount; i++)
                {
                    sheetMain.Cells[tableDataRow - 2, i + USER_COL_OFFSET_SUMMARY].Value = users.Values[i];
                    sheetMain.Cells[tableDataRow, i + USER_COL_OFFSET_SUMMARY].Value = billableHoursTotal[i] + nonBillableHoursTotal[i];
                    rowTotal += billableHoursTotal[i] + nonBillableHoursTotal[i];
                }

                // Total column
                sheetMain.Cells[tableDataRow, userCount + USER_COL_OFFSET_SUMMARY].Value = rowTotal;
                using (var rng = sheetMain.Cells[tableDataRow, userCount + USER_COL_OFFSET_SUMMARY, tableDataRow, userCount + USER_COL_OFFSET_SUMMARY])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0)); //Set color to yellow
                    rng.Style.Font.Color.SetColor(Color.Black);
                }
                tableDataRow++;

                sheetMain.Cells[tableDataRow, 2].Value = "B2";
                sheetMain.Cells[tableDataRow, 3].Value = "Total Billed";
                rowTotal = 0;
                for (int i = 0; i < userCount; i++)
                {
                    sheetMain.Cells[tableDataRow, i + USER_COL_OFFSET_SUMMARY].Value = billableHoursTotal[i] - flexTotals[i];
                    rowTotal += billableHoursTotal[i] - flexTotals[i];
                }
                sheetMain.Cells[tableDataRow, userCount + USER_COL_OFFSET_SUMMARY].Value = rowTotal;
                using (var rng = sheetMain.Cells[tableDataRow, userCount + USER_COL_OFFSET_SUMMARY, tableDataRow, userCount + USER_COL_OFFSET_SUMMARY])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0)); //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.Black);
                }
                tableDataRow++;

                // Billing Cycle Info
                if (showBillingCycle)
                {
                    var billngCycle = DataContext.BillingCycleEntrySet.FirstOrDefault(a => a.StartDate == startDate && a.EndDate == endDate);
                    if (billngCycle != null)
                    {
                        sheetMain.Cells[tableDataRow, 2].Value = "B3";
                        sheetMain.Cells[tableDataRow, 3].Value = "GVW Calender Financial Cycle Hours";
                        sheetMain.Cells[tableDataRow + 3, 2].Value = "A3";
                        sheetMain.Cells[tableDataRow + 3, 3].Value = "Month Available (Weekdays * 8 - Non-Eligible)";
                        sheetMain.Cells[tableDataRow + 4, 3].Value = "Billing Budget Target (Total Billed / (Month Available - All Budgeted Leaves)) = B2/(A3-All Budgeted Leaves)";

                        decimal monthAvailable = 0;
                        decimal billingBudgetTarget = 0;
                        for (int i = 0; i < userCount; i++)
                        {
                            monthAvailable = (billngCycle.Weekdays * LOG_EFFICIENCY_THRESHOLD) - (nonElegibleTotals[i]);

                            if (monthAvailable - (leaveSickTotals[i] + leaveVacationTotals[i]) == 0)
                            {
                                billingBudgetTarget = 0;
                            }
                            else
                            {
                                billingBudgetTarget = (billableHoursTotal[i] - flexTotals[i]) / (monthAvailable - (leaveSickTotals[i] + leaveVacationTotals[i]));
                            }

                            // GVW Calender Financial Cycle Hours
                            sheetMain.Cells[tableDataRow, i + USER_COL_OFFSET_SUMMARY].Value = billngCycle.Weekdays * LOG_EFFICIENCY_THRESHOLD;

                            // Month Available
                            sheetMain.Cells[tableDataRow + 3, i + USER_COL_OFFSET_SUMMARY].Value = monthAvailable;

                            // Billing Budget Target
                            sheetMain.Cells[tableDataRow + 4, i + USER_COL_OFFSET_SUMMARY].Value = billingBudgetTarget;
                            sheetMain.Cells[tableDataRow + 4, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                        }

                        sheetMain.Cells[tableDataRow, userCount + USER_COL_OFFSET_SUMMARY].Value = billngCycle.Weekdays * LOG_EFFICIENCY_THRESHOLD * userCount;

                        // Calculate billing budget target total
                        var totalBillable = billableHoursTotal.Sum();
                        var totalFlex = flexTotals.Sum();
                        var totalSick = leaveSickTotals.Sum();
                        var totalVaction = leaveVacationTotals.Sum();
                        var monthAvailableTotal = (billngCycle.Weekdays * LOG_EFFICIENCY_THRESHOLD * userCount) - nonElegibleTotals.Sum();
                        var totalBillingBudgetTarget = (totalBillable - totalFlex) / (monthAvailableTotal - (totalSick + totalVaction));

                        sheetMain.Cells[tableDataRow + 3, userCount + USER_COL_OFFSET_SUMMARY].Value = monthAvailableTotal;
                        sheetMain.Cells[tableDataRow + 4, userCount + USER_COL_OFFSET_SUMMARY].Value = totalBillingBudgetTarget;

                        // Format billing budget color
                        using (var rng = sheetMain.Cells[tableDataRow + 4, USER_COL_OFFSET_SUMMARY, tableDataRow + 4, userCount + USER_COL_OFFSET_SUMMARY])
                        {
                            rng.Style.Font.Color.SetColor(Color.FromArgb(0, 112, 192));
                            rng.Style.Numberformat.Format = PERCENTAGE_FORMAT;
                        }

                        tableDataRow++;
                    }
                    index++;
                }

                // Eligible Logged
                sheetMain.Cells[tableDataRow, 2].Value = "A1";
                sheetMain.Cells[tableDataRow, 3].Value = "Eligible Logged (All Logged excluding Non-Eligible)";
                sheetMain.Cells[tableDataRow + 1, 2].Value = "A2";
                sheetMain.Cells[tableDataRow + 1, 3].Value = "No. of Engineers";
                rowTotal = 0;
                for (int i = 0; i < userCount; i++)
                {
                    sheetMain.Cells[tableDataRow, i + USER_COL_OFFSET_SUMMARY].Value = (billableHoursTotal[i] + nonBillableHoursTotal[i]) - (nonElegibleTotals[i]);
                    sheetMain.Cells[tableDataRow + 1, i + USER_COL_OFFSET_SUMMARY].Value = 1;
                    rowTotal += (billableHoursTotal[i] + nonBillableHoursTotal[i]) - (nonElegibleTotals[i] + flexTotals[i]);
                }
                sheetMain.Cells[tableDataRow, userCount + USER_COL_OFFSET_SUMMARY].Value = rowTotal;
                sheetMain.Cells[tableDataRow + 1, userCount + USER_COL_OFFSET_SUMMARY].Value = userCount;
            }
            catch (Exception e)
            {
                throw new Exception("printAdditionalSummaryInfo failed, " + e.Message);
            }
        }

        private void printEfficiencyCalcs(ref ExcelWorksheet sheetMain, DateTime startDate, DateTime endDate, ref int tableDataRow, List<Guid> userIds,
           List<decimal> billableHoursTotal, List<decimal> billableCostTotal, List<decimal> nonBillableHoursTotal,
           List<decimal> nonElegibleTotals, List<decimal> flexTotals, List<decimal> leaveStudyTotals, List<decimal> systemIssueTotals, List<decimal> adminTotals, List<decimal> leaveVacationTotals, List<decimal> trainingTotals, List<decimal> nonInvoiceableTotals, List<decimal> leaveSickTotals)
        {
            try
            {
                // Check that there is a billing cycle
                var billngCycle = DataContext.BillingCycleEntrySet.FirstOrDefault(a => a.StartDate >= startDate && a.EndDate <= endDate);
                if (billngCycle != null)
                {
                    // Calc variables
                    decimal eligibleLogged = 0;
                    decimal monthAvailable = 0;
                    decimal totalBilledPerUser = 0;
                    decimal logEfficiency = 0;

                    // Average / Total variables
                    decimal totalMonthAvailable = 0;
                    decimal totalBilled = 0;
                    decimal totalEligibleLogged = 0;

                    decimal totalLogEfficiency = 0;
                    decimal totalLogEfficiencyHours = 0;
                    decimal totalBillingEffciency = 0;
                    decimal totalOverallBillingEffciency = 0;

                    decimal totalLeave = 0;
                    decimal totalLeaveOther = 0;
                    decimal totalAdmin = 0;
                    decimal totalSystemIssues = 0;
                    decimal totalTraining = 0;
                    decimal totalNonInvoiceable = 0;
                    decimal totalAdditionalLoggedHours = 0;
                    decimal totalAdditionalRevenue = 0;
                    decimal totalBudgetedLeaves = 0;

                    tableDataRow += 4;

                    // Row descriptions
                    sheetMain.Cells[tableDataRow, 3].Value = "Billing Effciency (Total Billed / Month Available) = (B2/A3)";
                    sheetMain.Cells[tableDataRow + 1, 3].Value = "Log Efficiency % Timesheet Score (Eligible Logged / Month Available) = (A1/A3)";
                    sheetMain.Cells[tableDataRow + 2, 3].Value = "Log Efficiency (Average Timesheet Score Hours)";
                    sheetMain.Cells[tableDataRow + 3, 3].Value = "Overall Billing Effciency (Total Billed / Eligible Logged) = (B2 / A1)";
                    sheetMain.Cells[tableDataRow + 4, 3].Value = "Vacation Leave (Pub Holidays & Annual) as % of Month Availalble";
                    sheetMain.Cells[tableDataRow + 5, 3].Value = "Other Leaves (E.g. Sick, Study) as % of Month Availalble";
                    sheetMain.Cells[tableDataRow + 6, 3].Value = "Admin (E.g. AES and other non - billable projects) as % Month Available";
                    sheetMain.Cells[tableDataRow + 7, 3].Value = "Triz System Issues as % of Month Availalble";
                    sheetMain.Cells[tableDataRow + 8, 3].Value = "Training as % of Month Available";
                    sheetMain.Cells[tableDataRow + 9, 3].Value = "Non-Billable Engineering Quality and Efficiency as % of Month Availalble";
                    sheetMain.Cells[tableDataRow + 10, 3].Value = "Additional Logged Hours (Above " + LOG_EFFICIENCY_THRESHOLD + " Hours)";
                    sheetMain.Cells[tableDataRow + 11, 3].Value = "Additional Revenue";
                    sheetMain.Cells[tableDataRow + 12, 3].Value = "All Budgeted Leaves";
                    sheetMain.Cells[tableDataRow + 13, 3].Value = "Efficiency Sum Check";

                    bool eligibleLoggedZero = false;
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        eligibleLoggedZero = false;
                        eligibleLogged = (billableHoursTotal[i] + nonBillableHoursTotal[i]) - (nonElegibleTotals[i] + flexTotals[i]);
                        monthAvailable = (billngCycle.Weekdays * LOG_EFFICIENCY_THRESHOLD) - nonElegibleTotals[i];

                        totalBilledPerUser = billableHoursTotal[i] - flexTotals[i];
                        totalMonthAvailable += monthAvailable;
                        totalEligibleLogged += eligibleLogged;

                        if (eligibleLogged == 0)
                        {
                            eligibleLoggedZero = true;
                        }

                        if (monthAvailable != 0)
                        {
                            logEfficiency = eligibleLoggedZero ? 0 : decimal.Round(((eligibleLogged / monthAvailable) * LOG_EFFICIENCY_THRESHOLD), 2);

                            // Billing Effciency (Total Billed / Month Available)  (B2/A3)
                            sheetMain.Cells[tableDataRow, i + USER_COL_OFFSET_SUMMARY].Value = totalBilledPerUser / monthAvailable;
                            sheetMain.Cells[tableDataRow, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                            totalBillingEffciency += totalBilledPerUser / monthAvailable;
                            totalBilled += totalBilledPerUser;

                            // Log Efficiency - Timesheet Score Hours (A1/A3) % (Eligible Logged / Month Available)
                            sheetMain.Cells[tableDataRow + 1, i + USER_COL_OFFSET_SUMMARY].Value = eligibleLoggedZero ? 0 : eligibleLogged / monthAvailable;
                            sheetMain.Cells[tableDataRow + 1, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                            totalLogEfficiency += eligibleLoggedZero ? 0 : eligibleLogged / monthAvailable;

                            // Log Efficiency - Timesheet Score Hours
                            sheetMain.Cells[tableDataRow + 2, i + USER_COL_OFFSET_SUMMARY].Value = eligibleLoggedZero ? 0 : logEfficiency;
                            totalLogEfficiencyHours += eligibleLoggedZero ? 0 : logEfficiency;

                            // Overall Billing Effciency (Total Billed / Eligible Logged)  (B2/A1)
                            sheetMain.Cells[tableDataRow + 3, i + USER_COL_OFFSET_SUMMARY].Value = eligibleLoggedZero ? 0 : totalBilledPerUser / eligibleLogged;
                            sheetMain.Cells[tableDataRow + 3, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                            totalOverallBillingEffciency += eligibleLoggedZero ? 0 : totalBilledPerUser / eligibleLogged;

                            // Vacation Leave (Pub Holidays & Annual) as % of (Month Availalble))
                            sheetMain.Cells[tableDataRow + 4, i + USER_COL_OFFSET_SUMMARY].Value = leaveVacationTotals[i] / monthAvailable;
                            sheetMain.Cells[tableDataRow + 4, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                            totalLeave += leaveVacationTotals[i];

                            // Other Leaves (Sick, Study) as % of (Month Availalble)
                            sheetMain.Cells[tableDataRow + 5, i + USER_COL_OFFSET_SUMMARY].Value = (leaveSickTotals[i] + leaveStudyTotals[i]) / monthAvailable;
                            sheetMain.Cells[tableDataRow + 5, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                            totalLeaveOther += leaveSickTotals[i] + leaveStudyTotals[i];

                            // Admin(Incl.AES and other non - billable projects) of Month Available
                            sheetMain.Cells[tableDataRow + 6, i + USER_COL_OFFSET_SUMMARY].Value = adminTotals[i] / monthAvailable;
                            sheetMain.Cells[tableDataRow + 6, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                            totalAdmin += adminTotals[i];

                            // Triz System Issues / Not Billed as % of (Month Availalble))
                            sheetMain.Cells[tableDataRow + 7, i + USER_COL_OFFSET_SUMMARY].Value = systemIssueTotals[i] / monthAvailable;
                            sheetMain.Cells[tableDataRow + 7, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                            totalSystemIssues += systemIssueTotals[i];

                            // Training as % of Month Available
                            sheetMain.Cells[tableDataRow + 8, i + USER_COL_OFFSET_SUMMARY].Value = trainingTotals[i] / monthAvailable;
                            sheetMain.Cells[tableDataRow + 8, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                            totalTraining += trainingTotals[i];

                            // Engineering Quality and Efficiency as % of Month Availalble
                            sheetMain.Cells[tableDataRow + 9, i + USER_COL_OFFSET_SUMMARY].Value = nonInvoiceableTotals[i] / monthAvailable;
                            sheetMain.Cells[tableDataRow + 9, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = PERCENTAGE_FORMAT;
                            totalNonInvoiceable += nonInvoiceableTotals[i];

                            // Additional Logged Hours(Above LOG_EFFICIENCY_THRESHOLD Hours)
                            var extraHours = eligibleLogged - monthAvailable;
                            Guid userId = userIds[i];
                            var billingRate = DataContext.BillingRatesSet.Where(br => br.UserAccountId == userId && startDate.CompareTo(br.EndDate) <= 0 && br.StartDate.CompareTo(endDate) <= 0).FirstOrDefault();
                            decimal rate = billingRate == null ? 0 : billingRate.Rate;

                            sheetMain.Cells[tableDataRow + 10, i + USER_COL_OFFSET_SUMMARY].Value = extraHours;
                            totalAdditionalLoggedHours += extraHours;

                            // Additional Revenue
                            sheetMain.Cells[tableDataRow + 11, i + USER_COL_OFFSET_SUMMARY].Value = rate * extraHours;
                            totalAdditionalRevenue += rate * extraHours;

                            // Set value to red, meaning there was no rate for the user
                            if (rate == 0)
                            {
                                sheetMain.Cells[tableDataRow + 11, i + USER_COL_OFFSET_SUMMARY].Style.Font.Color.SetColor(Color.Red);
                            }

                            sheetMain.Cells[tableDataRow + 11, i + USER_COL_OFFSET_SUMMARY].Style.Numberformat.Format = "$#,##0.00";

                            // All Budgeted Leaves
                            sheetMain.Cells[tableDataRow + 12, i + USER_COL_OFFSET_SUMMARY].Value = leaveSickTotals[i] + leaveVacationTotals[i];
                            totalBudgetedLeaves += leaveSickTotals[i] + leaveVacationTotals[i];

                            // Efficiency Sum Check
                            var logEfficeincy = eligibleLoggedZero ? 0 : eligibleLogged / monthAvailable;

                            var billingEfficeincy = totalBilledPerUser / monthAvailable;
                            var leave = leaveVacationTotals[i] / monthAvailable;
                            var leaveOther = (leaveSickTotals[i] + leaveStudyTotals[i]) / monthAvailable;
                            var admin = adminTotals[i] / monthAvailable;
                            var system = systemIssueTotals[i] / monthAvailable;
                            var training = trainingTotals[i] / monthAvailable;
                            var enigneeringQuality = nonInvoiceableTotals[i] / monthAvailable;
                            var calcResult = (billingEfficeincy + leave + leaveOther + admin + system + training + enigneeringQuality) - logEfficeincy;

                            // TODO: Next time
                            //var t = GetExcelColumnName(i);

                            sheetMain.Cells[tableDataRow + 13, i + USER_COL_OFFSET_SUMMARY].Value = calcResult;
                        }
                        else
                        {
                            // Make all 0
                            for (int k = 0; k < 14; k++)
                            {
                                sheetMain.Cells[tableDataRow + k, i + USER_COL_OFFSET_SUMMARY].Value = 0;
                            }
                        }
                    }

                    // Totals / Averages
                    sheetMain.Cells[tableDataRow, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalBilled / totalMonthAvailable;
                    sheetMain.Cells[tableDataRow + 1, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalEligibleLogged / totalMonthAvailable;
                    sheetMain.Cells[tableDataRow + 2, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = (totalEligibleLogged / totalMonthAvailable) * LOG_EFFICIENCY_THRESHOLD;
                    sheetMain.Cells[tableDataRow + 3, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalBilled / totalEligibleLogged;
                    sheetMain.Cells[tableDataRow + 4, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalLeave / totalMonthAvailable;
                    sheetMain.Cells[tableDataRow + 5, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalLeaveOther / totalMonthAvailable;
                    sheetMain.Cells[tableDataRow + 6, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalAdmin / totalMonthAvailable;
                    sheetMain.Cells[tableDataRow + 7, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalSystemIssues / totalMonthAvailable;
                    sheetMain.Cells[tableDataRow + 8, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalTraining / totalMonthAvailable;
                    sheetMain.Cells[tableDataRow + 9, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalNonInvoiceable / totalMonthAvailable;
                    sheetMain.Cells[tableDataRow + 10, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalAdditionalLoggedHours;
                    sheetMain.Cells[tableDataRow + 11, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalAdditionalRevenue;
                    sheetMain.Cells[tableDataRow + 12, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = totalBudgetedLeaves;

                    // Efficiency Sum Check
                    var logEfficeincyTotal = totalEligibleLogged / totalMonthAvailable;

                    var billingEfficeincyTotal = totalBilled / totalMonthAvailable;
                    var leaveTotal = totalLeave / totalMonthAvailable;
                    var leaveOtherTotal = totalLeaveOther / totalMonthAvailable;
                    var adminTotal = totalAdmin / totalMonthAvailable;
                    var systemTotal = totalSystemIssues / totalMonthAvailable;
                    var trainingTotal = totalTraining / totalMonthAvailable;
                    var enigneeringQualityTotal = totalNonInvoiceable / totalMonthAvailable;
                    var calcResultTotal = (billingEfficeincyTotal + leaveTotal + leaveOtherTotal + adminTotal + systemTotal + trainingTotal + enigneeringQualityTotal) - logEfficeincyTotal;
                    sheetMain.Cells[tableDataRow + 13, userIds.Count() + USER_COL_OFFSET_SUMMARY].Value = calcResultTotal;

                    /////////////////
                    //// STYLING ////
                    /////////////////

                    // Billing Budget Target (Total Billed / (Month Available - All Budgeted Leaves)) B2/(A3-All Budgeted Leaves)
                    using (var rng = sheetMain.Cells[tableDataRow, USER_COL_OFFSET_SUMMARY, tableDataRow, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Font.Color.SetColor(Color.FromArgb(55, 86, 35));
                    }

                    // Admin(Incl.AES and other non - billable projects) of Month Available
                    using (var rng = sheetMain.Cells[tableDataRow + 6, USER_COL_OFFSET_SUMMARY, tableDataRow + 6, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
                    }

                    // Training as % of Month Available
                    using (var rng = sheetMain.Cells[tableDataRow + 8, USER_COL_OFFSET_SUMMARY, tableDataRow + 8, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Font.Color.SetColor(Color.FromArgb(122, 48, 160));
                    }

                    // Additional Logged Hours(Above 8 Hours)
                    // Additional Revenue
                    sheetMain.Cells[tableDataRow + 10, 3].Style.Font.Bold = true;
                    sheetMain.Cells[tableDataRow + 11, 3].Style.Font.Bold = true;
                    using (var rng = sheetMain.Cells[tableDataRow + 10, 3, tableDataRow + 11, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    // Format Totals as Percentages
                    using (var rng = sheetMain.Cells[tableDataRow, 3, tableDataRow + 10, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    }

                    // Overwrite formatting for specific cases
                    // Billing Effciency (Total Billed / Month Available) (B2/A3)
                    using (var rng = sheetMain.Cells[tableDataRow, USER_COL_OFFSET_SUMMARY, tableDataRow, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    }

                    // Log Efficiency - Timesheet Score Hours
                    using (var rng = sheetMain.Cells[tableDataRow + 2, USER_COL_OFFSET_SUMMARY, tableDataRow + 2, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Numberformat.Format = "0.00";
                    }
                    // Additional Logged Hours(Above 8 Hours)
                    using (var rng = sheetMain.Cells[tableDataRow + 10, USER_COL_OFFSET_SUMMARY, tableDataRow + 10, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Numberformat.Format = "0.00";
                    }
                    // Additional Revenue
                    using (var rng = sheetMain.Cells[tableDataRow + 11, USER_COL_OFFSET_SUMMARY, tableDataRow + 11, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Numberformat.Format = "$#,##0.00";
                    }

                    // Efficiency Sum Check
                    using (var rng = sheetMain.Cells[tableDataRow + 13, USER_COL_OFFSET_SUMMARY, tableDataRow + 13, userIds.Count() + USER_COL_OFFSET_SUMMARY])
                    {
                        rng.Style.Numberformat.Format = PERCENTAGE_FORMAT;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("printEfficiencyCalcs failed, " + e.Message);
            }
        }

        private void printFlexSection(ref ExcelWorksheet sheetMain, ref int tableDataRow, SortedList<int, string> users, List<PivotedTimesheetRow> billableFlexProjects, bool showPhases)
        {
            try
            {
                tableDataRow += 15;

                sheetMain.Cells[tableDataRow, 1].Value = "FLEX ENGINEERING: Billable Only";
                using (var rng = sheetMain.Cells[tableDataRow, 1, tableDataRow, users.Count() + USER_COL_OFFSET])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(50, 50, 50)); //Set color to dark grey
                    rng.Style.Font.Color.SetColor(Color.White);
                }
                tableDataRow++;

                // Print column headings
                sheetMain.Cells[tableDataRow, 2].Value = "Client";
                sheetMain.Cells[tableDataRow, 3].Value = "Project Description & Code";
                sheetMain.Cells[tableDataRow, 4].Value = "Project Level";
                sheetMain.Cells[tableDataRow, 5].Value = "Project Type";
                sheetMain.Cells[tableDataRow, USER_COL_OFFSET_SUMMARY + users.Count()].Value = "Total";

                // Print user names
                for (int i = 0; i < users.Count(); i++)
                {
                    sheetMain.Cells[tableDataRow, USER_COL_OFFSET_SUMMARY + i].Value = users.Values[i];
                }

                // Set header Style
                using (var rng = sheetMain.Cells[tableDataRow, 1, tableDataRow, USER_COL_OFFSET_SUMMARY + users.Count()])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                tableDataRow++;

                // Print project names and totals
                string clientName = "";
                string projectName = "";

                decimal projectTotal = 0;
                decimal projectTotalCost = 0;

                List<decimal> userTotals = new List<decimal>(new decimal[users.Count]);
                List<decimal> userTotalsCost = new List<decimal>(new decimal[users.Count]);

                decimal finalTotalHours = 0;
                decimal finalTotalCost = 0;
                for (int i = 0; i < billableFlexProjects.Count(); i++)
                {
                    if (clientName != billableFlexProjects[i].Client)
                    {
                        clientName = billableFlexProjects[i].Client;
                        sheetMain.Cells[tableDataRow, 2].Value = clientName;
                    }

                    if (showPhases)
                    {
                        // Print Project Name if only sub projects return
                        if (projectName != billableFlexProjects[i].ProjectName)
                        {
                            projectName = billableFlexProjects[i].ProjectName;
                            sheetMain.Cells[tableDataRow, 3].Value = projectName;
                            sheetMain.Cells[tableDataRow, 4].Value = "Project";
                            sheetMain.Cells[tableDataRow, 5].Value = billableFlexProjects[i].ProjectTypeName;
                            using (var rng = sheetMain.Cells[tableDataRow, 3, tableDataRow, USER_COL_OFFSET_SUMMARY + users.Count()])
                            {
                                rng.Style.Font.Bold = true;
                            }

                            tableDataRow++;
                        }
                    }

                    if (billableFlexProjects[i].SubProjectTypeName == null)
                    {
                        sheetMain.Cells[tableDataRow, 3].Value = billableFlexProjects[i].ProjectName;
                        sheetMain.Cells[tableDataRow, 4].Value = "Project";
                        sheetMain.Cells[tableDataRow, 5].Value = billableFlexProjects[i].ProjectTypeName;
                    }
                    else
                    {
                        sheetMain.Cells[tableDataRow, 3].Value = "      " + billableFlexProjects[i].PhaseName;
                        sheetMain.Cells[tableDataRow, 4].Value = "Sub-Project";
                        sheetMain.Cells[tableDataRow, 5].Value = billableFlexProjects[i].SubProjectTypeName;

                        using (var rng = sheetMain.Cells[tableDataRow, 3, tableDataRow, USER_COL_OFFSET_SUMMARY + users.Count()])
                        {
                            rng.Style.Font.Color.SetColor(Color.Red);
                        }
                    }

                    // Print totals per project
                    projectTotal = 0;
                    projectTotalCost = 0;
                    for (int j = 0; j < billableFlexProjects[i].hours.Count(); j++)
                    {
                        userTotals[j] += billableFlexProjects[i].hours[j];
                        userTotalsCost[j] += billableFlexProjects[i].cost[j];

                        projectTotal += billableFlexProjects[i].hours[j];
                        projectTotalCost += billableFlexProjects[i].cost[j];

                        sheetMain.Cells[tableDataRow, USER_COL_OFFSET_SUMMARY + j].Value = billableFlexProjects[i].hours[j];
                    }

                    finalTotalHours += projectTotal;
                    finalTotalCost += projectTotalCost;

                    // Set total column
                    sheetMain.Cells[tableDataRow, USER_COL_OFFSET_SUMMARY + users.Count()].Value = projectTotal;
                    tableDataRow++;
                }

                // Print user totals
                tableDataRow++;
                sheetMain.Cells[tableDataRow, 3].Value = "Total Flex Hours";
                sheetMain.Cells[tableDataRow + 1, 3].Value = "Total Flex Revenue";
                for (int i = 0; i < userTotals.Count(); i++)
                {
                    sheetMain.Cells[tableDataRow, i + USER_COL_OFFSET_SUMMARY].Value = userTotals[i];
                    sheetMain.Cells[tableDataRow + 1, i + USER_COL_OFFSET_SUMMARY].Value = userTotalsCost[i];
                }

                // Print final total
                sheetMain.Cells[tableDataRow, USER_COL_OFFSET_SUMMARY + users.Count()].Value = finalTotalHours;
                sheetMain.Cells[tableDataRow + 1, USER_COL_OFFSET_SUMMARY + users.Count()].Value = finalTotalCost;

                // Set Style
                using (var rng = sheetMain.Cells[tableDataRow, 3, tableDataRow + 1, USER_COL_OFFSET_SUMMARY + users.Count()])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(50, 50, 50));
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                // Additional Revenue
                using (var rng = sheetMain.Cells[tableDataRow + 1, 3, tableDataRow + 1, USER_COL_OFFSET_SUMMARY + users.Count()])
                {
                    rng.Style.Numberformat.Format = "$#,##0.00";
                }
            }
            catch (Exception e)
            {
                throw new Exception("printFlexSection failed, " + e.Message);
            }
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        private void printDateRangeHeading(DateTime startDate, DateTime endDate, ref ExcelWorksheet sheet, int row, int col)
        {
            string startDateString = "Earliest";
            string endDateString = "Latest";

            //Small hack - if start and end date is unspecified it gets set to these values and needs to show on report as Earliest and Latest
            if (startDate.Year != 1971)
                startDateString = startDate.ToShortDateString();
            if (endDate.Year != 2071)
                endDateString = endDate.ToShortDateString();

            sheet.Cells[row, col].Value = string.Format("REPORT PERIOD {0} to {1}",
                startDateString, endDateString);
        }

        public byte[] GenerateTimesheetDetailClientReporter(DateTime startDate, DateTime endDate, String clients, String projects,
            String userAccounts, bool showPhases)
        {
            Authenticate(PrivilegeType.CustomerReportAccess);
            clients = filterClientListForClientReporter(clients);

            return GenerateTimesheetDetailWorker(startDate, endDate, projects, userAccounts, clients, false, showPhases, true);
        }

        public byte[] GenerateTimesheetDetailOld(DateTime startDate, DateTime endDate, String projects,
            String userAccounts, String clients, String employers, bool showRates, bool showPhases)
        {
            Authenticate(PrivilegeType.ReportGenerationTimesheet);
            return GenerateTimesheetDetailWorker(startDate, endDate, projects, userAccounts, clients, showRates, showPhases);
        }

        public byte[] GenerateTimesheetDetail(DateTime startDate, DateTime endDate, String projects,
            String userAccounts, String clients, String employers, bool showUnassigned, bool showRates, bool showPhases)
        {
            Authenticate(PrivilegeType.ReportGenerationTimesheet);
            return GenerateTimesheetDetailWithEmployersWorker(startDate, endDate, projects, userAccounts, clients, employers, showUnassigned, showRates, showPhases);
        }

        public byte[] GenerateTimesheetDetailWorker(DateTime startDate, DateTime endDate, String projects,
            String userAccounts, String clients, bool showRates, bool showPhases, bool billable = false)
        {
            var repEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0).AddDays(1);
            var repStartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

            var timesheetStoreProc = DataContext.ExecuteTimesheetDetailProcedure(repStartDate, repEndDate,
                    userAccounts == null ? "All" : userAccounts,
                    clients == null ? "All" : clients,
                    projects == null ? "All" : projects,
                    billable);

            using (var pck = new ExcelPackage())
            {
                //Create the worksheet
                var sheets = new List<ExcelWorksheet>();
                var sheetMain = pck.Workbook.Worksheets.Add("Timesheet Detail");
                sheets.Add(sheetMain);

                //Show Top side Values
                sheetMain.Cells[1, 1].Value = "TRIZ Timesheet Detail ";
                sheetMain.Cells[1, 1].Style.Font.Bold = true;
                sheetMain.Cells[1, 1].Style.Font.Size = 11;
                printDateRangeHeading(startDate, endDate, ref sheetMain, 2, 1);
                sheetMain.Cells[3, 1].Value = string.Format("Date Generated : {0} ", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

                sheetMain.Cells[1, 1, 1, 3].Merge = true;
                sheetMain.Cells[2, 1, 2, 3].Merge = true;
                sheetMain.Cells[3, 1, 3, 3].Merge = true;

                var rowIndex = 4;
                //Column Headings
                sheetMain.Cells[rowIndex, 1].Value = "Employee";
                sheetMain.Cells[rowIndex, 2].Value = "Timesheet Date";
                sheetMain.Cells[rowIndex, 3].Value = "Client";
                sheetMain.Cells[rowIndex, 4].Value = "Project";
                sheetMain.Cells[rowIndex, 5].Value = "Project Description";
                sheetMain.Cells[rowIndex, 6].Value = "Project Type";
                sheetMain.Cells[rowIndex, 7].Value = "Billable (Yes/No)";
                sheetMain.Cells[rowIndex, 8].Value = "Team";
                sheetMain.Cells[rowIndex, 9].Value = "Activity";
                sheetMain.Cells[rowIndex, 10].Value = "Comments";
                sheetMain.Cells[rowIndex, 11].Value = "Hours";
                if (showRates)
                {
                    sheetMain.Cells[rowIndex, 12].Value = "Rate/Hour";
                    sheetMain.Cells[rowIndex, 13].Value = "Amount";
                    sheetMain.Cells[rowIndex, 12].Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                    sheetMain.Cells[rowIndex, 13].Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                }

                using (var rng = sheetMain.Cells[rowIndex, 1, rowIndex, showRates ? 13 : 11])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.LightSlateGray); //Set color to dark blue
                    rng.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                }

                foreach (var row in timesheetStoreProc.ToList())
                {
                    rowIndex++;
                    sheetMain.Cells[rowIndex, 1].Value = row.Contact;
                    sheetMain.Cells[rowIndex, 2].Value = row.DateEntry.ToString("MMM dd, yyyy");
                    sheetMain.Cells[rowIndex, 3].Value = row.Client;
                    if (showPhases && row.SubProjectNumber != null)
                    {
                        sheetMain.Cells[rowIndex, 4].Value = row.ProjectNumber + "-" + row.SubProjectNumber;
                        sheetMain.Cells[rowIndex, 5].Value = row.ProjectName + " [" + row.SubProjectName + "]";
                        sheetMain.Cells[rowIndex, 6].Value = row.SubProjectBillableType;
                    }
                    else
                    {
                        sheetMain.Cells[rowIndex, 4].Value = row.ProjectNumber;
                        sheetMain.Cells[rowIndex, 5].Value = row.ProjectName;
                        sheetMain.Cells[rowIndex, 6].Value = row.ProjectBillableType;
                    }

                    sheetMain.Cells[rowIndex, 7].Value = row.Billable == true ? "Yes" : "No";
                    sheetMain.Cells[rowIndex, 8].Value = row.TeamName;
                    sheetMain.Cells[rowIndex, 9].Value = row.ActivityName;
                    sheetMain.Cells[rowIndex, 10].Value = row.Comments;
                    sheetMain.Cells[rowIndex, 11].Value = row.Hours;
                    if (showRates && row.Rate != null)
                    {
                        sheetMain.Cells[rowIndex, 12].Value = row.Rate;
                        sheetMain.Cells[rowIndex, 13].Value = row.Rate * row.Hours;
                    }
                }

                AutoWidthColumns(ref sheetMain);
                return pck.GetAsByteArray();
            }
        }

        public byte[] GenerateTimesheetDetailWithEmployersWorker(DateTime startDate, DateTime endDate, String projects,
            String userAccounts, String clients, String employers, bool showUnassigned, bool showRates, bool showPhases, bool billable = false)
        {
            // Normalise start and end date
            var repEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0).AddDays(1);
            var repStartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

            // Filter params
            var f_employers = employers != null && employers != "All" ? employers.Split(',').Select(Guid.Parse).ToList() : new List<Guid>();
            var employersDict = DataContext.EmployerSet.ToDictionary(a => a.Id, a => a);

            var timesheetData = DataContext.ExecuteTimesheetDetailProcedure(repStartDate, repEndDate,
                   userAccounts == null ? "All" : userAccounts,
                   clients == null ? "All" : clients,
                   projects == null ? "All" : projects,
                   billable).ToList();

            // Enrich Data
            using (var ctx = new DataContext())
            {
                foreach (var entry in timesheetData)
                {
                    entry.EmployerId = Guid.Empty;
                    entry.EmployerName = "Unassigned";

                    var designation = ctx.TeamJobDesignationSet.FirstOrDefault(tj => tj.UserAccountId == entry.UserAccountId && entry.DateEntry >= tj.StartDate && entry.DateEntry <= tj.EndDate);

                    if (designation != null)
                    {
                        entry.EmployerId = designation.EmployerId.Value;
                        entry.EmployerName = employersDict[entry.EmployerId].Name;
                    }
                }
            }

            if (!showUnassigned)
            {
                timesheetData.RemoveAll(e => e.EmployerId == Guid.Empty);
            }

            // Filter on employer
            if (f_employers.Count > 0)
            {
                if (showUnassigned)
                {
                    f_employers.Add(Guid.Empty);
                }

                timesheetData.RemoveAll(e => !f_employers.Contains(e.EmployerId));
            }

            // Order
            timesheetData = timesheetData.OrderBy(t => t.Contact).ThenBy(t => t.DateEntry).ThenBy(t => t.EmployerName).ThenBy(t => t.ProjectNumber).ToList();

            using (var pck = new ExcelPackage())
            {
                //Create the worksheet
                var sheets = new List<ExcelWorksheet>();
                var sheetMain = pck.Workbook.Worksheets.Add("Timesheet Detail");
                sheets.Add(sheetMain);

                //Show Top side Values
                sheetMain.Cells[1, 1].Value = "TRIZ Timesheet Detail ";
                sheetMain.Cells[1, 1].Style.Font.Bold = true;
                sheetMain.Cells[1, 1].Style.Font.Size = 11;
                printDateRangeHeading(startDate, endDate, ref sheetMain, 2, 1);
                sheetMain.Cells[3, 1].Value = string.Format("Date Generated : {0} ", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

                sheetMain.Cells[1, 1, 1, 3].Merge = true;
                sheetMain.Cells[2, 1, 2, 3].Merge = true;
                sheetMain.Cells[3, 1, 3, 3].Merge = true;

                var rowIndex = 4;
                //Column Headings
                sheetMain.Cells[rowIndex, 1].Value = "Employee";
                sheetMain.Cells[rowIndex, 2].Value = "Timesheet Date";
                sheetMain.Cells[rowIndex, 3].Value = "Employer";
                sheetMain.Cells[rowIndex, 4].Value = "Client";
                sheetMain.Cells[rowIndex, 5].Value = "Project";
                sheetMain.Cells[rowIndex, 6].Value = "Project Description";
                sheetMain.Cells[rowIndex, 7].Value = "Project Type";
                sheetMain.Cells[rowIndex, 8].Value = "Billable (Yes/No)";
                sheetMain.Cells[rowIndex, 9].Value = "Team";
                sheetMain.Cells[rowIndex, 10].Value = "Activity";
                sheetMain.Cells[rowIndex, 11].Value = "Comments";
                sheetMain.Cells[rowIndex, 12].Value = "Hours";

                if (showRates)
                {
                    sheetMain.Cells[rowIndex, 13].Value = "Rate/Hour";
                    sheetMain.Cells[rowIndex, 14].Value = "Amount";
                    sheetMain.Cells[rowIndex, 13].Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                    sheetMain.Cells[rowIndex, 14].Style.Numberformat.Format = "\"$\"#,##0.00;[Red]\"$\"#,##0.00";
                }

                using (var rng = sheetMain.Cells[rowIndex, 1, rowIndex, showRates ? 14 : 12])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.LightSlateGray); //Set color to dark blue
                    rng.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                }

                foreach (var row in timesheetData)
                {
                    rowIndex++;
                    sheetMain.Cells[rowIndex, 1].Value = row.Contact;
                    sheetMain.Cells[rowIndex, 2].Value = row.DateEntry.ToString("MMM dd, yyyy");
                    sheetMain.Cells[rowIndex, 3].Value = row.EmployerName;
                    sheetMain.Cells[rowIndex, 4].Value = row.Client;

                    if (showPhases && row.SubProjectNumber != null)
                    {
                        sheetMain.Cells[rowIndex, 5].Value = row.ProjectNumber + "-" + row.SubProjectNumber;
                        sheetMain.Cells[rowIndex, 6].Value = row.ProjectName + " [" + row.SubProjectName + "]";
                        sheetMain.Cells[rowIndex, 7].Value = row.SubProjectBillableType;
                    }
                    else
                    {
                        sheetMain.Cells[rowIndex, 5].Value = row.ProjectNumber;
                        sheetMain.Cells[rowIndex, 6].Value = row.ProjectName;
                        sheetMain.Cells[rowIndex, 7].Value = row.ProjectBillableType;
                    }

                    sheetMain.Cells[rowIndex, 8].Value = row.Billable == true ? "Yes" : "No";
                    sheetMain.Cells[rowIndex, 9].Value = row.TeamName;
                    sheetMain.Cells[rowIndex, 10].Value = row.ActivityName;
                    sheetMain.Cells[rowIndex, 11].Value = row.Comments;
                    sheetMain.Cells[rowIndex, 12].Value = row.Hours;

                    if (showRates && row.Rate != null)
                    {
                        sheetMain.Cells[rowIndex, 13].Value = row.Rate;
                        sheetMain.Cells[rowIndex, 14].Value = row.Hours * row.Rate;
                    }
                }

                AutoWidthColumns(ref sheetMain);
                return pck.GetAsByteArray();
            }
        }

        public byte[] GenerateBillingReportClientReporter(DateTime startDate, DateTime endDate, String clients, List<Guid> projectIds, List<Guid> employerIds, bool showUnassigned, bool showRates)
        {
            Authenticate(PrivilegeType.CustomerReportAccess);
            clients = filterClientListForClientReporter("All");
            List<String> clientids = clients.Split(',').ToList();

            var repEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0).AddDays(1);
            var repStartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

            if (projectIds.Count == 0)
            {
                projectIds = DataContext.TimesheetEntrySet
                    .Where(ts => ts.DateEntry >= startDate)
                    .Where(ts => ts.DateEntry <= endDate)
                    .Where(ts => clients.Contains(ts.Project.ClientId.ToString()))
                    .OrderBy(ts => ts.Project.ProjectNumber)
                    //.ThenBy(ts => ts.SubProject.ProjectName)
                    .Select(ts => ts.ProjectId).ToList();
            }
            using (var pck = new ExcelPackage())
            {
                var i = 1;
                foreach (var projectId in projectIds.Distinct())
                {
                    var project = DataContext.ProjectSet
                        .Where(p => p.Id == projectId)
                        .Where(p => p.Billable == true)
                        .Where(p => clientids.Contains(p.ClientId.ToString())).FirstOrDefault();
                    //Create the worksheet
                    if (project != null)
                    {
                        var sheets = new List<ExcelWorksheet>();
                        var sheetMain = pck.Workbook.Worksheets.Add(i++ + "." + project.ProjectNumber);
                        sheets.Add(sheetMain);
                        GenerateBillingReportDetailForProject(startDate, endDate, project, employerIds, showUnassigned, showRates, ref sheetMain);
                    }
                }
                if (i == 1)
                {
                    var sheets = new List<ExcelWorksheet>();
                    var sheetMain = pck.Workbook.Worksheets.Add("Sheet 1");
                    sheets.Add(sheetMain);
                }

                return pck.GetAsByteArray();
            }
        }

        public byte[] GenerateBillingReport(DateTime startDate, DateTime endDate, List<Guid> projectIds, List<Guid> employerIds, bool showUnassigned, bool showRates)
        {
            Authenticate(PrivilegeType.ReportGenerationTimesheet);
            var repEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0).AddDays(1);
            var repStartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

            if (projectIds.Count == 0)
            {
                projectIds = DataContext.TimesheetEntrySet
                    .Where(ts => ts.DateEntry >= startDate)
                    .Where(ts => ts.DateEntry <= endDate)
                    .OrderBy(ts => ts.Project.ProjectNumber)
                    //.ThenBy(ts => ts.SubProject.ProjectName)
                    .Select(ts => ts.ProjectId).ToList();
            }
            using (var pck = new ExcelPackage())
            {
                var i = 1;
                foreach (var projectId in projectIds.Distinct())
                {
                    var project = DataContext.ProjectSet.Where(p => p.Id == projectId).FirstOrDefault();
                    //Create the worksheet
                    if (project != null)
                    {
                        var sheets = new List<ExcelWorksheet>();
                        var sheetMain = pck.Workbook.Worksheets.Add(i++ + "." + project.ProjectNumber);
                        sheets.Add(sheetMain);

                        GenerateBillingReportDetailForProject(startDate, endDate, project, employerIds, showUnassigned, showRates, ref sheetMain);
                    }
                }
                if (i == 0)
                {
                    var sheets = new List<ExcelWorksheet>();
                    var sheetMain = pck.Workbook.Worksheets.Add("Sheet 1");
                    sheets.Add(sheetMain);
                }

                return pck.GetAsByteArray();
            }
        }

        public void GenerateBillingReportDetailForProject(DateTime startDate, DateTime endDate, Project project, List<Guid> employerIDs, bool showUnassigned, bool showRates, ref ExcelWorksheet sheet)
        {
            var repEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0).AddDays(1);
            var repStartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

            sheet.Cells[1, 1].Value = "PROJECT NAME: " + project.ProjectNumber + "-" + project.ProjectName;
            sheet.Cells[1, 1].Style.Font.Bold = true;
            sheet.Cells[1, 1].Style.Font.Size = 11;

            sheet.Cells[2, 1].Value = "CUSTOMER: " + project.Client.EntityName;
            sheet.Cells[2, 1].Style.Font.Bold = true;
            sheet.Cells[2, 1].Style.Font.Size = 11;

            printDateRangeHeading(startDate, endDate, ref sheet, 3, 1);
            using (var rng = sheet.Cells[3, 1, 3, 3])
                rng.Style.Font.Size = 11;

            sheet.Cells[4, 1].Value = string.Format("Date Generated : {0} ", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

            sheet.Cells[5, 1].Value = "PROJECT BILLABLE HOURS ANALYSIS";
            sheet.Cells[5, 1].Style.Font.Bold = true;
            sheet.Cells[5, 1].Style.Font.Size = 12;
            sheet.Cells[5, 1].Style.Font.UnderLine = true;

            sheet.Cells[10, 6].Value = "Billable Hours";
            sheet.Cells[10, 6].Style.Font.Bold = true;
            sheet.Cells[10, 6].Style.Font.Size = 10;
            sheet.Cells[10, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            sheet.Cells[11, 4].Value = "Total Hours For Report Period";
            sheet.Cells[11, 4].Style.Font.Bold = true;
            sheet.Cells[11, 4].Style.Font.Size = 10;

            if (showRates)
            {
                sheet.Cells[13, 4].Value = "Total Cost For Report Period";
                sheet.Cells[13, 4].Style.Font.Bold = true;
                sheet.Cells[13, 4].Style.Font.Size = 10;
            }

            //Column Headings
            sheet.Cells[16, 1].Value = "SUB PROJECT";
            sheet.Cells[16, 2].Value = "TEAM";
            sheet.Cells[16, 3].Value = "ACTIVITY";
            sheet.Cells[16, 4].Value = "NAME";
            sheet.Cells[16, 5].Value = "EMPLOYER";
            sheet.Cells[16, 6].Value = "BILLABLE HRS";
            if (showRates)
            {
                sheet.Cells[16, 7].Value = "RATE/HOUR";
                sheet.Cells[16, 8].Value = "AMOUNT";
            }

            using (var rng = sheet.Cells[16, 1, 16, showRates ? 8 : 6])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.LightSlateGray); //Set color to dark blue
                rng.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }

            var rowIndex = 17;
            var timesheetStoreProc = DataContext.ExecuteBillableHoursProcedure(repStartDate, repEndDate, project.Id.ToString()).ToList();

            if (showUnassigned)
            {
                // Exclude based on employer
                if (employerIDs.Count() > 0)
                {
                    timesheetStoreProc = timesheetStoreProc.Where(e => e.EmployerId == null || employerIDs.Contains(Guid.Parse(e.EmployerId == null ? Guid.Empty.ToString() : e.EmployerId))).ToList();
                }
            }
            else
            {
                timesheetStoreProc = timesheetStoreProc.Where(e => e.EmployerId != null).ToList();

                // Exclude based on employer
                if (employerIDs.Count() > 0)
                    timesheetStoreProc = timesheetStoreProc.Where(e => employerIDs.Contains(Guid.Parse(e.EmployerId))).ToList();
            }

            var rows = timesheetStoreProc;

            var i = 0;
            decimal totalHours = 0;
            decimal totalCost = 0;
            while (i < rows.Count)
            {
                decimal phaseHours = 0;
                decimal phaseCost = 0;
                var currentPhase = rows[i].Phase;
                while (i < rows.Count && rows[i].Phase == currentPhase)
                {
                    decimal activityHours = 0;
                    decimal activityCost = 0;

                    var currentActivity = rows[i].ActivityName;
                    while (i < rows.Count && rows[i].ActivityName == currentActivity)
                    {
                        var currentTeam = rows[i].TeamName;
                        while (i < rows.Count && rows[i].TeamName == currentTeam)
                        {
                            sheet.Cells[rowIndex, 1].Value = rows[i].Phase;
                            sheet.Cells[rowIndex, 2].Value = rows[i].TeamName;
                            sheet.Cells[rowIndex, 3].Value = rows[i].ActivityName;
                            sheet.Cells[rowIndex, 4].Value = rows[i].Contact;
                            sheet.Cells[rowIndex, 5].Value = rows[i].Employer == null ? "Unassigned" : rows[i].Employer;
                            sheet.Cells[rowIndex, 6].Value = rows[i].Hours;

                            if (showRates && rows[i].Rate != null)
                            {
                                sheet.Cells[rowIndex, 7].Value = rows[i].Rate;
                                sheet.Cells[rowIndex, 8].Value = rows[i].Rate * rows[i].Hours;
                                sheet.Cells[rowIndex, 7].Style.Numberformat.Format = "#,##0.00";
                                sheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0.00";
                                activityCost += (decimal)rows[i].Rate * rows[i].Hours;
                                phaseCost += (decimal)rows[i].Rate * rows[i].Hours;
                            }

                            activityHours += rows[i].Hours;
                            phaseHours += rows[i].Hours;

                            i++;
                            rowIndex++;
                        }
                    }
                    sheet.Cells[rowIndex, 2].Value = currentActivity;
                    sheet.Cells[rowIndex, 6].Value = activityHours;
                    if (showRates)
                        sheet.Cells[rowIndex, 8].Value = activityCost;
                    using (var rng = sheet.Cells[rowIndex, 1, rowIndex, showRates ? 8 : 6])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Font.Size = 8;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }

                    totalHours += activityHours;
                    totalCost += activityCost;
                    rowIndex++;
                }
                sheet.Cells[rowIndex, 1].Value = currentPhase;
                sheet.Cells[rowIndex, 6].Value = phaseHours;
                if (showRates)
                    sheet.Cells[rowIndex, 8].Value = phaseCost;
                using (var rng = sheet.Cells[rowIndex, 1, rowIndex, showRates ? 8 : 6])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Size = 8;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Silver);
                }

                rowIndex++;
            }

            sheet.Cells[rowIndex, 1].Value = "Summary";
            sheet.Cells[rowIndex, 6].Value = totalHours;
            sheet.Cells[rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            if (showRates)
            {
                sheet.Cells[rowIndex, 8].Value = totalCost;
                sheet.Cells[rowIndex, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells[13, 6].Value = totalCost;
                sheet.Cells[13, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[13, 6].Style.Numberformat.Format = "#,##0.00";
            }

            sheet.Cells[11, 6].Value = totalHours;
            sheet.Cells[11, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[11, 6].Style.Numberformat.Format = "#,##0.00";

            using (var rng = sheet.Cells[rowIndex, 1, rowIndex, showRates ? 8 : 6])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }

            using (var rng = sheet.Cells[16, 6, rowIndex, 6])
            {
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.Numberformat.Format = "#,##0.00";
            }

            using (var rng = sheet.Cells[15, 1, rowIndex, showRates ? 8 : 6])
                rng.Style.Font.Size = 8;

            AutoWidthColumns(ref sheet);
        }

        #endregion Timesheets

        #region Scorecards

        public byte[] GenerateScorecardEmployeeSummary(Guid scorecardTemplateId, Guid[] scorecardTemplatePeriodsIds,
            Guid employeeId)
        {
            throw new NotImplementedException();
        }

        public static string ConvertHtmlToText(string source)
        {
            if (source == null) return "";
            string result;

            // Remove HTML Development formatting
            // Replace line breaks with space
            // because browsers inserts space
            result = source.Replace("\r", " ");
            // Replace line breaks with space
            // because browsers inserts space
            result = result.Replace("\n", " ");
            // Remove step-formatting
            result = result.Replace("\t", string.Empty);
            // Remove repeating speces becuase browsers ignore them
            result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                  @"( )+", " ");

            // Remove the header (prepare first by clearing attributes)
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*head([^>])*>", "<head>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"(<( )*(/)( )*head( )*>)", "</head>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(<head>).*(</head>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // remove all scripts (prepare first by clearing attributes)
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*script([^>])*>", "<script>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"(<( )*(/)( )*script( )*>)", "</script>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //result = System.Text.RegularExpressions.Regex.Replace(result,
            //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
            //         string.Empty,
            //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"(<script>).*(</script>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // remove all styles (prepare first by clearing attributes)
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*style([^>])*>", "<style>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"(<( )*(/)( )*style( )*>)", "</style>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(<style>).*(</style>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // insert tabs in spaces of <td> tags
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*td([^>])*>", "\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // insert line breaks in places of <BR> and <LI> tags
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*br( )*>", "\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*li( )*>", "\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // insert line paragraphs (double line breaks) in place
            // if <P>, <DIV> and <TR> tags
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*div([^>])*>", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*tr([^>])*>", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*p([^>])*>", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Remove remaining tags like <a>, links, images,
            // comments etc - anything thats enclosed inside < >
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<[^>]*>", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // replace special characters:
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&nbsp;", " ",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&bull;", " * ",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&lsaquo;", "<",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&rsaquo;", ">",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&trade;", "(tm)",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&frasl;", "/",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<", "<",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @">", ">",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&copy;", "(c)",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&reg;", "(r)",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // Remove all others. More can be added, see
            // http://hotwired.lycos.com/webmonkey/reference/special_characters/
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&(.{2,6});", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // make line breaking consistent
            result = result.Replace("\n", "\r");

            // Remove extra line breaks and tabs:
            // replace over 2 breaks with 2 and over 4 tabs with 4.
            // Prepare first to remove any whitespaces inbetween
            // the escaped characters and remove redundant tabs inbetween linebreaks
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\r)( )+(\r)", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\t)( )+(\t)", "\t\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\t)( )+(\r)", "\t\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\r)( )+(\t)", "\r\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // Remove redundant tabs
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\r)(\t)+(\r)", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // Remove multible tabs followind a linebreak with just one tab
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\r)(\t)+", "\r\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // Initial replacement target string for linebreaks
            string breaks = "\r\r\r";
            // Initial replacement target string for tabs
            string tabs = "\t\t\t\t\t";
            for (int index = 0; index < result.Length; index++)
            {
                result = result.Replace(breaks, "\r\r");
                result = result.Replace(tabs, "\t\t\t\t");
                breaks = breaks + "\r";
                tabs = tabs + "\t";
            }

            // Thats it.
            return result;
        }

        private void printScoreCardTemplateKey(ScorecardTemplate template, ref int index, ref ExcelWorksheet sheet)
        {
            sheet.Cells[index, 1].Value = "MEASURE";
            sheet.Cells[index, 3].Value = "DESCRIPTION";
            sheet.SelectedRange[index, 1, index, 3].Style.Font.Bold = true;

            index++;
            foreach (var item in template.ScorecardTemplateItems)
            {
                //                sheet.Cells[1, 1].IsRichText = true;
                sheet.Cells[index, 1].Value = item.Description;
                sheet.Cells[index, 2].Value = String.Format("[E] {0}\n[A] {1}\n [I] {2}", item.ExcellentDefinition, item.AdequateDefinition, item.InadequateDefinition);
                //                var d = h.ToRtf(item.Definition);
                if (item.Definition != null)
                    sheet.Cells[index, 3].Value = ConvertHtmlToText(item.Definition);

                for (int i = 1; i <= 3; i++)
                {
                    sheet.Cells[index, i].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    sheet.Cells[index, i].Style.WrapText = true;
                }
                index++;
            }

            AutoWidthColumns(ref sheet);
            sheet.Column(1).Width = 40;
            sheet.Column(2).Width = 40;
            sheet.Column(3).Width = 80;
        }

        private void printUserSelectionHeader(string heading, bool searchAllYears, string[] reviewYears,
            bool searchAllPeriods, Guid[] reviewPeriods, int submitted, int locked,
            int employeeHasScorecard, Guid[] employees, Guid[] clients, Guid[] lineManagers,
            Guid[] evaluators, Guid[] scorecards, ref int rowIndex, ref ExcelWorksheet sheet)
        {
            var periodsChosen = new List<ScorecardTemplatePeriod>();
            if (!searchAllPeriods)
            {
                periodsChosen = DataContext.ScorecardTemplatePeriodSet.Where(st => reviewPeriods.Contains(st.Id)).OrderBy(st => st.StartDate).ToList();
            }

            var employeesChosen = new List<UserIdentity>();
            if (employees != null)
            {
                employeesChosen = DataContext.UserIdentitySet.Where(u => employees.Contains(u.Id)).OrderBy(u => u.FirstName).ThenBy(u => u.Surname).ToList();
            }

            var clientsChosen = new List<ClientEntity>();
            if (clients != null)
            {
                clientsChosen = DataContext.ClientEntitySet.Where(c => clients.Contains(c.Id)).OrderBy(c => c.EntityName).ToList();
            }

            var lineManagersChosen = new List<UserIdentity>();
            if (lineManagers != null)
            {
                lineManagersChosen = DataContext.UserIdentitySet.Where(u => lineManagers.Contains(u.Id)).OrderBy(u => u.FirstName).ThenBy(u => u.Surname).ToList();
            }

            var evaluatorsChosen = new List<UserIdentity>();
            if (evaluators != null)
            {
                evaluatorsChosen = DataContext.UserIdentitySet.Where(u => evaluators.Contains(u.Id)).OrderBy(u => u.FirstName).ThenBy(u => u.Surname).ToList();
            }

            var scorecardsChosen = new List<ScorecardTemplate>();
            if (scorecards != null)
            {
                scorecardsChosen = DataContext.ScorecardTemplateSet.Where(t => scorecards.Contains(t.Id)).OrderBy(t => t.ScorecardName).ToList();
            }

            sheet.Cells[rowIndex, 1].Value = heading;
            sheet.Cells[rowIndex++, 1].Style.Font.Bold = true;

            sheet.Cells[rowIndex, 1].Value = "Report Date";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            sheet.Cells[rowIndex++, 2].Value = string.Format("{0}", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

            var user = DataContext.UserIdentitySet.SingleOrDefault(x => x.AccountName == HttpContext.Current.User.Identity.Name);
            sheet.Cells[rowIndex, 1].Value = "Created By";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            sheet.Cells[rowIndex++, 2].Value = user.FirstName + " " + user.Surname;

            //            sheet.Cells[rowIndex++, 1].Value = "Parameters Chosen";

            // Add paramaters

            // Review year
            sheet.Cells[rowIndex, 1].Value = "Review Year";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            if (searchAllYears == true)
            {
                sheet.Cells[rowIndex, 2].Value = "All";
                rowIndex++;
            }
            else
            {
                foreach (var item in reviewYears)
                {
                    sheet.Cells[rowIndex, 2].Value = item;
                    rowIndex++;
                }
            }

            // Review period
            sheet.Cells[rowIndex, 1].Value = "Review Period";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            if (searchAllPeriods == true)
            {
                sheet.Cells[rowIndex, 2].Value = "All";
                rowIndex++;
            }
            else
            {
                foreach (var item in periodsChosen)
                {
                    sheet.Cells[rowIndex, 2].Value = item.Description;
                    rowIndex++;
                }
            }

            // Submitted
            sheet.Cells[rowIndex, 1].Value = "Submitted";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            sheet.Cells[rowIndex, 2].Value = submitted == 0 ? "Yes" : submitted == 1 ? "No" : "All";

            // Locked
            rowIndex++;
            sheet.Cells[rowIndex, 1].Value = "Locked";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            sheet.Cells[rowIndex, 2].Value = locked == 0 ? "Yes" : locked == 1 ? "No" : "All";

            // Employee has scorecard
            //rowIndex++;
            //sheet.Cells[rowIndex, 1].Value = "Employee has score card";
            //sheet.Cells[rowIndex, 2].Value = employeeHasScorecard == 0 ? "Yes" : employeeHasScorecard == 1 ? "No" : "All";

            // Employees
            rowIndex++;
            sheet.Cells[rowIndex, 1].Value = "Employee";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            if (employees == null)
            {
                sheet.Cells[rowIndex, 2].Value = "All";
                rowIndex++;
            }
            else
            {
                foreach (var item in employeesChosen)
                {
                    sheet.Cells[rowIndex, 2].Value = item.FirstName + " " + item.Surname;
                    rowIndex++;
                }
            }

            // Clients
            sheet.Cells[rowIndex, 1].Value = "Client";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            if (clients == null)
            {
                sheet.Cells[rowIndex, 2].Value = "All";
                rowIndex++;
            }
            else
            {
                foreach (var item in clientsChosen)
                {
                    sheet.Cells[rowIndex, 2].Value = item.EntityName;
                    rowIndex++;
                }
            }

            // Line Managers
            sheet.Cells[rowIndex, 1].Value = "Line Manager";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            if (lineManagers == null)
            {
                sheet.Cells[rowIndex, 2].Value = "All";
                rowIndex++;
            }
            else
            {
                foreach (var item in lineManagersChosen)
                {
                    sheet.Cells[rowIndex, 2].Value = item.FirstName + " " + item.Surname;
                    rowIndex++;
                }
            }

            // Evaluators
            sheet.Cells[rowIndex, 1].Value = "Evaluator";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            if (evaluators == null)
            {
                sheet.Cells[rowIndex, 2].Value = "All";
                rowIndex++;
            }
            else
            {
                foreach (var item in evaluatorsChosen)
                {
                    sheet.Cells[rowIndex, 2].Value = item.FirstName + " " + item.Surname;
                    rowIndex++;
                }
            }

            // Score Card
            sheet.Cells[rowIndex, 1].Value = "Score Card";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            if (scorecards == null)
            {
                sheet.Cells[rowIndex, 2].Value = "All";
                rowIndex++;
            }
            else
            {
                foreach (var item in scorecardsChosen)
                {
                    sheet.Cells[rowIndex, 2].Value = item.ScorecardName;
                    rowIndex++;
                }
            }
        }

        // New scorecard reports:
        public byte[] GenerateScorecardStatusSummary(bool searchAllYears, string[] reviewYears,
            bool searchAllPeriods, Guid[] reviewPeriods, int submitted, int locked,
            int employeeHasScorecard, Guid[] employees, Guid[] clients, Guid[] lineManagers,
            Guid[] evaluators, Guid[] scorecards, out string fileName)
        {
            Authenticate(PrivilegeType.ReportGenerationScoreCard);

            var scorecardSummary = DataContext.ExecuteScorecardSummaryProcedure(
                    searchAllYears ? "All" : string.Join(",", reviewYears),
                    searchAllPeriods ? "All" : string.Join(",", reviewPeriods),
                    submitted, locked,
                    employeeHasScorecard,
                    employees == null ? "All" : string.Join(",", employees),
                    clients == null ? "All" : string.Join(",", clients),
                    lineManagers == null ? "All" : string.Join(",", lineManagers),
                    evaluators == null ? "All" : string.Join(",", evaluators),
                    scorecards == null ? "All" : string.Join(",", scorecards));

            // Build Param list

            using (var pck = new ExcelPackage())
            {
                //Create the worksheet
                var sheets = new List<ExcelWorksheet>();
                var sheet = pck.Workbook.Worksheets.Add("Performance Management Status");
                sheets.Add(sheet);

                int rowIndex = 1;

                printUserSelectionHeader("Performance Management Status", searchAllYears, reviewYears, searchAllPeriods, reviewPeriods, submitted, locked,
                            employeeHasScorecard, employees, clients, lineManagers,
                            evaluators, scorecards, ref rowIndex, ref sheet);

                rowIndex++;

                //                var repEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0).AddDays(1);
                //                var repStartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

                // Start main table
                using (var rng = sheet.Cells[rowIndex, 1, rowIndex, 9])
                {
                    rng.Style.Font.Bold = true;
                }

                sheet.Cells[rowIndex, 1].Value = "Employee Name";
                sheet.Cells[rowIndex, 1].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[rowIndex, 2].Value = "Scorecard Name";
                sheet.Cells[rowIndex, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[rowIndex, 3].Value = "Review Year";
                sheet.Cells[rowIndex, 3].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[rowIndex, 4].Value = "Period";
                sheet.Cells[rowIndex, 4].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[rowIndex, 5].Value = "Line Manager";
                sheet.Cells[rowIndex, 5].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[rowIndex, 6].Value = "Evaluator Name";
                sheet.Cells[rowIndex, 6].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[rowIndex, 7].Value = "Date Created";
                sheet.Cells[rowIndex, 7].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[rowIndex, 8].Value = "Submitted";
                sheet.Cells[rowIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[rowIndex, 9].Value = "Locked";
                sheet.Cells[rowIndex, 9].Style.Border.BorderAround(ExcelBorderStyle.Medium);

                rowIndex++;
                foreach (var scorecard in scorecardSummary)
                {
                    string startPeriod = scorecard.StartDate.ToString("yyyy/MM/dd");
                    string endPeriod = scorecard.EndDate.ToString("yyyy/MM/dd");
                    int reviewYear = scorecard.ReviewYear.Value;

                    // Variable Scorecard Logic
                    if (scorecard.ScorecardVariableStart != null)
                    {
                        // Was based on a variable scorecard template period
                        startPeriod = scorecard.ScorecardVariableStart.Value.ToString("yyyy/MM/dd");
                        endPeriod = scorecard.ScorecardVariableEnd.Value.ToString("yyyy/MM/dd");
                        reviewYear = scorecard.ScorecardVariableYear.Value;
                    }

                    sheet.Cells[rowIndex, 1].Value = scorecard.EmployeeName;
                    sheet.Cells[rowIndex, 2].Value = scorecard.ScorecardName;
                    sheet.Cells[rowIndex, 3].Value = reviewYear;
                    sheet.Cells[rowIndex, 4].Value = startPeriod + " - " + endPeriod;
                    sheet.Cells[rowIndex, 5].Value = scorecard.LineManagerName;
                    sheet.Cells[rowIndex, 6].Value = scorecard.EvaluatorFirstName + " " + scorecard.EvaluatorSurname;
                    sheet.Cells[rowIndex, 7].Value = scorecard.DateCreated == null ? "" : scorecard.DateCreated.Value.ToString("yyyy/MM/dd");
                    sheet.Cells[rowIndex, 8].Value = scorecard.Submitted;
                    sheet.Cells[rowIndex, 9].Value = scorecard.locked;
                    //                    sheet.Cells[2, 2].Value = string.Format("{0}", DateTime.Now. + " " + DateTime.Now.ToLongTimeString());

                    rowIndex++;
                }

                AutoWidthColumns(ref sheet);

                fileName = "ScorecardStatusSummary.xlsx";

                return pck.GetAsByteArray();
            }
        }

        private void printScorecardHeading(int detailLevel, ref int rowIndex, ref ExcelWorksheet sheet)
        {
            sheet.Cells[rowIndex, 1].Value = "Detail Level";
            sheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            if (detailLevel == 0)
                sheet.Cells[rowIndex, 2].Value = "Final Combined";
            if (detailLevel == 1)
                sheet.Cells[rowIndex, 2].Value = "Detailed";
            if (detailLevel == 2)
                sheet.Cells[rowIndex, 2].Value = "Final Combined And Detailed";

            rowIndex++; rowIndex++;
            sheet.Cells[rowIndex, 1].Value = "Employee";
            sheet.Cells[rowIndex, 2].Value = "Group";
            sheet.Cells[rowIndex, 3].Value = "Designation (at Report Date)";
            sheet.Cells[rowIndex, 4].Value = "Detail Level";
            sheet.Cells[rowIndex, 5].Value = "Evaluator";
            using (var rng = sheet.Cells[rowIndex, 1, rowIndex, 5])
            {
                rng.Style.Font.Bold = true;
                rng.Style.WrapText = true;
            }
            rowIndex++;
        }

        public byte[] GenerateScorecardFinalCombined(bool searchAllYears, string[] reviewYears, bool searchAllPeriods,
            Guid[] scorecardTemplatePeriodsIds, int detailLevel, int scoreCardStatus, int locked,
            Guid[] employees, Guid[] clients, Guid[] lineManagers, Guid[] evaluators, Guid scorecardTemplateId,
            out string fileName)

        //        public byte[] GenerateScorecardFinalCombined(Guid scorecardTemplateId, Guid[] scorecardTemplatePeriodsIds,
        //            Guid employeeId, Int32 scoreCardStatus, out string fileName)
        {
            Authenticate(PrivilegeType.ReportGenerationScoreCard);

            using (var pck = new ExcelPackage())
            {
                //Create the worksheet
                var sheets = new List<ExcelWorksheet>();
                String MainsheetTitle = scoreCardStatus == 1 ? "Unsubmitted Scorecard" : "Submitted Scorecard";
                ExcelWorksheet Mainsheet = null;
                ExcelWorksheet Subsheet = null;
                int subRowIndex = 1;
                int unsubRowIndex = 1;

                Mainsheet = pck.Workbook.Worksheets.Add(MainsheetTitle);
                sheets.Add(Mainsheet);
                printUserSelectionHeader("Scorecard(s)", searchAllYears, reviewYears, searchAllPeriods, scorecardTemplatePeriodsIds, scoreCardStatus, locked,
                        2, employees, clients, lineManagers, evaluators, new Guid[] { scorecardTemplateId }, ref subRowIndex, ref Mainsheet);
                printScorecardHeading(detailLevel, ref subRowIndex, ref Mainsheet);

                if (scoreCardStatus == 2)
                {
                    Subsheet = pck.Workbook.Worksheets.Add("Unsubmitted Scorecard");
                    sheets.Add(Subsheet);
                    printUserSelectionHeader("Unsubmitted Scorecard(s)", searchAllYears, reviewYears, searchAllPeriods, scorecardTemplatePeriodsIds, scoreCardStatus, locked,
                    2, employees, clients, lineManagers, evaluators, new Guid[] { scorecardTemplateId }, ref unsubRowIndex, ref Subsheet);
                    printScorecardHeading(detailLevel, ref unsubRowIndex, ref Subsheet);
                }

                var scorecardTemplate = DataContext.ScorecardTemplateSet.Include(s => s.ScorecardTemplateItems).Where(t => t.Id == scorecardTemplateId).Single();
                var scorecardPeriods = DataContext.ScorecardTemplatePeriodSet.Where(p => p.ScorecardTemplateId == scorecardTemplateId);

                if ((!searchAllPeriods) && scorecardTemplatePeriodsIds != null && scorecardTemplatePeriodsIds.Length > 0)
                    scorecardPeriods = scorecardPeriods.Where(p => scorecardTemplatePeriodsIds.Contains(p.Id));

                if ((!searchAllYears) && reviewYears != null && reviewYears.Length > 0)
                    scorecardPeriods = scorecardPeriods.Where(p => reviewYears.Contains(p.ReviewYear.ToString()));

                scorecardPeriods = scorecardPeriods.OrderBy(p => p.ReportSortOrder);

                int keyIndex = 1;
                ExcelWorksheet keySheet = pck.Workbook.Worksheets.Add("Score Card Key");
                printScoreCardTemplateKey(scorecardTemplate, ref keyIndex, ref keySheet);

                IList<Guid> tjdEmployees = null;
                /* Determine employee set for scorecards */
                if ((clients != null && clients.Length > 0) || (lineManagers != null && lineManagers.Length > 0))
                {
                    var tjd = DataContext.TeamJobDesignationSet.Where(t => t.StartDate <= DateTime.Now && t.EndDate >= DateTime.Now);
                    if (clients != null && clients.Length > 0)
                        tjd = tjd.Where(t => clients.Contains(t.ClientId));
                    if (lineManagers != null && lineManagers.Length > 0)
                        tjd = tjd.Where(t => lineManagers.Contains((Guid)t.LineLeaderId));

                    tjdEmployees = tjd.Select(t => t.UserAccountId).Distinct().ToList();
                }

                IQueryable<UserIdentity> employeesList = DataContext.UserIdentitySet;
                if (tjdEmployees != null && employees.Length < 1)
                {
                    employeesList = DataContext.UserIdentitySet.Where(e => tjdEmployees.Contains(e.Id));
                }

                if (employees != null && employees.Length > 0)
                {
                    employeesList = DataContext.UserIdentitySet.Where(e => employees.Contains(e.Id));
                }

                //                if (employeeId != null && employeeId.ToString() != "00000000-0000-0000-0000-000000000000")
                //                    employees = employees.Where(u => u.Id == employeeId);

                foreach (var employee in employeesList.OrderBy(e => e.FirstName).ToList())
                {
                    bool status = scoreCardStatus == 0 ? true : scoreCardStatus == 1 ? false : false;
                    if (scoreCardStatus == 2)
                    {
                        AddEmployeeScorecardToSheet(employee, true, scorecardPeriods, scorecardTemplate, detailLevel, locked, evaluators, ref subRowIndex, ref Mainsheet);
                        AddEmployeeScorecardToSheet(employee, false, scorecardPeriods, scorecardTemplate, detailLevel, locked, evaluators, ref unsubRowIndex, ref Subsheet);
                    }
                    else
                    {
                        AddEmployeeScorecardToSheet(employee, status, scorecardPeriods, scorecardTemplate, detailLevel, locked, evaluators, ref subRowIndex, ref Mainsheet);
                    }
                }

                if (subRowIndex == 1)
                    Mainsheet.Cells[1, 1].Value = "No Scorecard data captured or marked as completed!";

                AutoWidthColumns(ref Mainsheet);

                if (scoreCardStatus == 2)
                    AutoWidthColumns(ref Subsheet);

                fileName = "ScoreCard.xlsx";

                return pck.GetAsByteArray();
            }
        }

        private void AddEmployeeScorecardToSheet(UserIdentity employee, bool status, IQueryable<ScorecardTemplatePeriod> scorecardPeriods,
                                ScorecardTemplate scorecardTemplate, int detailLevel, int locked, Guid[] evaluators,
                                ref int rowIndex, ref ExcelWorksheet sheet)
        {
            var scorecardsQ = DataContext.ScorecardSet.Include(s => s.ScorecardRecords)
                                                 .Where(s => s.EmployeeId == employee.Id)
                                                 .Where(s => (s.Completed == status))
                                                 .Where(s => scorecardPeriods.Select(sp => sp.Id).Contains(s.ScorecardTemplatePeriodId));

            if (locked == 0)
                scorecardsQ = scorecardsQ.Where(s => s.locked);

            if (locked == 1)
                scorecardsQ = scorecardsQ.Where(s => !s.locked);

            if (evaluators != null && evaluators.Length > 0)
                scorecardsQ = scorecardsQ.Where(s => evaluators.Contains(s.EvaluatorId));

            var scorecards = scorecardsQ.ToList();

            if (scorecards.Count > 0)
            {
                var teamJob = DataContext.TeamJobDesignationSet.Where(t => t.UserAccountId == employee.Id && t.StartDate <= DateTime.Now && t.EndDate >= DateTime.Now).FirstOrDefault();
                var group = teamJob == null ? "" : teamJob.Client == null ? "" : teamJob.Client.EntityName;
                if (detailLevel != 1)
                    GenerateScorecardSummary(scorecardTemplate, scorecardPeriods.ToList(), scorecards, employee, teamJob.JobDesignation, group, null, ref rowIndex, ref sheet);
                if (detailLevel != 0)
                    foreach (var evaluator in scorecards.Select(s => s.Evaluator).OrderBy(s => s.FirstName).Distinct().ToList())
                    {
                        var scorecardsForEmployeeByEvaluatorAccrossPeriods = scorecards.Where(s => s.EvaluatorId == evaluator.Id).ToList();
                        GenerateScorecardSummary(scorecardTemplate, scorecardPeriods.ToList(), scorecardsForEmployeeByEvaluatorAccrossPeriods, employee, teamJob == null ? "" : teamJob.JobDesignation, group, evaluator, ref rowIndex, ref sheet);
                    }
            }
        }

        public byte[] GenerateScorecardEmployeeSummary(Guid scorecardTemplateId, Guid[] scorecardTemplatePeriodsIds,
            Guid employeeId, Int32 scoreCardStatus, out string fileName)
        {
            Authenticate(PrivilegeType.ReportGenerationScoreCard);

            using (var pck = new ExcelPackage())
            {
                //Create the worksheet
                var sheets = new List<ExcelWorksheet>();
                String MainsheetTitle = scoreCardStatus == 1 ? "Unsubmitted Scorecard" : "Submitted Scorecard";
                ExcelWorksheet Mainsheet = null;
                ExcelWorksheet Subsheet = null;

                Mainsheet = pck.Workbook.Worksheets.Add(MainsheetTitle);
                sheets.Add(Mainsheet);
                if (scoreCardStatus == 2)
                {
                    Subsheet = pck.Workbook.Worksheets.Add("Unsubmitted Scorecard");
                    sheets.Add(Subsheet);
                }

                var scorecardTemplate = DataContext.ScorecardTemplateSet.Include(s => s.ScorecardTemplateItems).Where(t => t.Id == scorecardTemplateId).Single();
                var scorecardPeriods = DataContext.ScorecardTemplatePeriodSet.Where(p => p.ScorecardTemplateId == scorecardTemplateId);

                if (scorecardTemplatePeriodsIds != null && scorecardTemplatePeriodsIds.Length > 0)
                {
                    scorecardPeriods = scorecardPeriods.Where(p => scorecardTemplatePeriodsIds.Contains(p.Id));
                }
                scorecardPeriods = scorecardPeriods.OrderBy(p => p.StartDate);

                var employees = DataContext.UserIdentitySet.Where(u => u.Active == true);
                if (employeeId != null && employeeId.ToString() != "00000000-0000-0000-0000-000000000000")
                    employees = employees.Where(u => u.Id == employeeId);

                int subRowIndex = 1;
                int unsubRowIndex = 1;
                foreach (var employee in employees.OrderBy(e => e.FirstName).ToList())
                {
                    bool status = scoreCardStatus == 0 ? true : scoreCardStatus == 1 ? false : false;
                    List<Scorecard> scorecards = null;
                    List<Scorecard> submittedScorecards = null;
                    List<Scorecard> unsubmittedScorecards = null;
                    if (scoreCardStatus == 2)
                    {
                        submittedScorecards = DataContext.ScorecardSet.Include(s => s.ScorecardRecords)
                                                             .Where(s => s.EmployeeId == employee.Id)
                                                             .Where(s => (s.Completed == true))
                                                             .Where(s => scorecardPeriods.Select(sp => sp.Id).Contains(s.ScorecardTemplatePeriodId))
                                                             .ToList();

                        unsubmittedScorecards = DataContext.ScorecardSet.Include(s => s.ScorecardRecords)
                                                             .Where(s => s.EmployeeId == employee.Id)
                                                             .Where(s => (s.Completed == false))
                                                             .Where(s => scorecardPeriods.Select(sp => sp.Id).Contains(s.ScorecardTemplatePeriodId))
                                                             .ToList();

                        var subEvaluators = submittedScorecards.Select(s => s.Evaluator).OrderBy(s => s.FirstName).Distinct().ToList();
                        foreach (var evaluator in subEvaluators)
                        {
                            var scorecardsForEmployeeByEvaluatorAccrossPeriods = submittedScorecards.Where(s => s.EvaluatorId == evaluator.Id).ToList();
                            GenerateScorecard(scorecardTemplate, scorecardPeriods.ToList(), scorecardsForEmployeeByEvaluatorAccrossPeriods, employee, evaluator, ref subRowIndex, ref Mainsheet);
                        }

                        var unsubEvaluators = unsubmittedScorecards.Select(s => s.Evaluator).OrderBy(s => s.FirstName).Distinct().ToList();
                        foreach (var evaluator in unsubEvaluators)
                        {
                            var scorecardsForEmployeeByEvaluatorAccrossPeriods = unsubmittedScorecards.Where(s => s.EvaluatorId == evaluator.Id).ToList();
                            GenerateScorecard(scorecardTemplate, scorecardPeriods.ToList(), scorecardsForEmployeeByEvaluatorAccrossPeriods, employee, evaluator, ref unsubRowIndex, ref Subsheet);
                        }
                    }
                    else
                    {
                        scorecards = DataContext.ScorecardSet.Include(s => s.ScorecardRecords)
                                                             .Where(s => s.EmployeeId == employee.Id)
                                                             .Where(s => (s.Completed == status))
                                                             .Where(s => scorecardPeriods.Select(sp => sp.Id).Contains(s.ScorecardTemplatePeriodId))
                                                             .ToList();

                        var evaluators = scorecards.Select(s => s.Evaluator).OrderBy(s => s.FirstName).Distinct().ToList();
                        foreach (var evaluator in evaluators)
                        {
                            var scorecardsForEmployeeByEvaluatorAccrossPeriods = scorecards.Where(s => s.EvaluatorId == evaluator.Id).ToList();
                            GenerateScorecard(scorecardTemplate, scorecardPeriods.ToList(), scorecardsForEmployeeByEvaluatorAccrossPeriods, employee, evaluator, ref subRowIndex, ref Mainsheet);
                        }
                    }
                }

                if (subRowIndex == 1)
                    Mainsheet.Cells[1, 1].Value = "No Scorecard data captured or marked as completed!";

                AutoWidthColumns(ref Mainsheet);

                if (scoreCardStatus == 2)
                    AutoWidthColumns(ref Subsheet);

                fileName = "ScoreCard.xlsx";

                return pck.GetAsByteArray();
            }
        }

        private void GenerateScorecardSummary(ScorecardTemplate tempate, List<ScorecardTemplatePeriod> periods, List<Scorecard> scorecards, UserIdentity employee, String team, String group, UserIdentity evaluator, ref int rowIndex, ref ExcelWorksheet sheet)
        {
            int numPeriods = periods.Count;
            int fo = 5;

            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);
            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);

            //Draw border around scorecard
            using (var rng = sheet.Cells[rowIndex, fo + 1, rowIndex + tempate.ScorecardTemplateItems.Count + 10, fo + numPeriods * 4 + 7])
            {
                rng.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }
            //Heading
            using (var rng = sheet.Cells[rowIndex + 2, fo + 1, rowIndex + 2, fo + numPeriods * 2 + 6])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to light blue
            }
            //Heading
            using (var rng = sheet.Cells[rowIndex + 3, fo + 1, rowIndex + 3, fo + numPeriods * 2 + 6])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200)); //Set color to grey
                rng.Style.Font.Color.SetColor(Color.White);
            }

            //Total row
            using (var rng = sheet.Cells[rowIndex + 6 + tempate.ScorecardTemplateItems.Count, fo + 1, rowIndex + 6 + tempate.ScorecardTemplateItems.Count, fo + numPeriods * 2 + 6])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to light blue
            }

            //Draw border around each period
            for (var i = 0; i < numPeriods; i++)
            {
                //Border
                using (var rng = sheet.Cells[rowIndex + 1, fo + i * 2 + 2, rowIndex + tempate.ScorecardTemplateItems.Count + 6, fo + i * 2 + 3])
                    rng.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }

            #region 1st Row

            printFilterColumn(employee, evaluator, team, group, rowIndex, ref sheet);
            sheet.Cells[rowIndex, 1 + fo].Value = string.Format("Employee: {0} {1}", employee.FirstName, employee.Surname);
            sheet.Cells[rowIndex, 1 + fo, rowIndex, 2 + fo].Merge = true;
            if (evaluator == null)
                sheet.Cells[rowIndex, 3 + fo].Value = string.Format("{0}", "FINAL SCORE CARD");
            else
                sheet.Cells[rowIndex, 3 + fo].Value = string.Format("Evaluator: {0} {1}", evaluator.FirstName, evaluator.Surname);
            sheet.Cells[rowIndex, 3 + fo, rowIndex, 4 + fo].Merge = true;
            rowIndex++;

            #endregion 1st Row

            #region 2nd Row, 3rd Row (blank), 4th Row, 5th Row

            printFilterColumn(employee, evaluator, team, group, rowIndex, ref sheet);
            sheet.Cells[rowIndex, 1 + fo].Value = string.Format("Score Card: {0}", tempate.ScorecardName);
            int colIndex = 2 + fo;
            int periodIndex = 8 + fo + numPeriods * 2;
            foreach (var period in periods)
            {
                sheet.Cells[rowIndex, colIndex].Value = string.Format("Period: {0}", period.Description);
                using (var rng = sheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1])
                {
                    rng.Style.Font.Bold = true;
                    rng.Merge = true;
                }

                sheet.Cells[rowIndex, periodIndex].Value = string.Format("Evaluator Comment {0}", period.Description);
                sheet.Cells[rowIndex, periodIndex + 1].Value = string.Format("Employee Comment {0}", period.Description);
                using (var rng = sheet.Cells[rowIndex, periodIndex, rowIndex + tempate.ScorecardTemplateItems.Count + 9, periodIndex + 1])
                    rng.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                using (var rng = sheet.Cells[rowIndex, periodIndex, rowIndex, periodIndex + 1])
                    rng.Style.Font.Bold = true;

                sheet.Cells[rowIndex + tempate.ScorecardTemplateItems.Count + 7, periodIndex].Value = string.Format("Evaluator Comment");
                sheet.Cells[rowIndex + tempate.ScorecardTemplateItems.Count + 7, periodIndex + 1].Value = string.Format("Employee Comment");
                periodIndex++;
                periodIndex++;

                sheet.Cells[rowIndex + 2, colIndex + 1].Value = string.Format("Score");

                colIndex++; colIndex++;
            }
            sheet.Cells[rowIndex + 2, fo + 2 + numPeriods * 2].Value = string.Format("E");
            sheet.Cells[rowIndex + 3, fo + 2 + numPeriods * 2].Value = string.Format("{0}%", tempate.ExcellentWeight);
            sheet.Cells[rowIndex + 2, fo + 3 + numPeriods * 2].Value = string.Format("A");
            sheet.Cells[rowIndex + 3, fo + 3 + numPeriods * 2].Value = string.Format("{0}%", tempate.AdequateWeight);
            sheet.Cells[rowIndex + 2, fo + 4 + numPeriods * 2].Value = string.Format("I");
            sheet.Cells[rowIndex + 3, fo + 4 + numPeriods * 2].Value = string.Format("{0}%", tempate.InadequateWeight);
            sheet.Cells[rowIndex + 2, fo + 5 + numPeriods * 2].Value = string.Format("Wt.");
            sheet.Cells[rowIndex + 3, fo + 5 + numPeriods * 2].Value = string.Format("%");
            sheet.Cells[rowIndex + 2, fo + 6 + numPeriods * 2].Value = string.Format("YTD Score");

            //            rowIndex++; rowIndex++; rowIndex++; rowIndex++; rowIndex++;
            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);
            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);
            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);
            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);
            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);

            #endregion 2nd Row, 3rd Row (blank), 4th Row, 5th Row

            decimal[] scoretotals = new decimal[numPeriods];
            decimal? totE = 0;
            decimal? totA = 0;
            decimal? totI = 0;
            decimal? totWeight = 0;
            decimal? totScore = 0;

            #region scored items

            foreach (var templateItem in tempate.ScorecardTemplateItems.OrderBy(t => t.Order))
            {
                printFilterColumn(employee, evaluator, team, group, rowIndex, ref sheet);
                sheet.Cells[rowIndex, fo + 1].Value = templateItem.Description;
                if (templateItem.ScorecardScoring != 0)
                    sheet.Cells[rowIndex, fo + 1].Value += " / " + Convert.ToInt16(templateItem.Maximum);
                totWeight += templateItem.Weight;
                colIndex = 2;

                #region for each period

                decimal? lineTotE = 0;
                decimal? lineTotA = 0;
                decimal? lineTotI = 0;
                decimal? totManual = 0;
                decimal? lineTotal = 0;
                decimal? lineTotalCount = 0;
                int completedPeriodCount = 0;
                periodIndex = 0;

                foreach (var period in periods)
                {
                    ScorecardRecord scoreRecord = null;
                    decimal? score = 0;
                    int scoreCount = 0;
                    foreach (var scorecard in scorecards.Where(s => s.ScorecardTemplatePeriod.Id == period.Id))
                    {
                        scoreRecord = scorecard.ScorecardRecords.Where(s => s.ScorecardTemplateItemId == templateItem.Id).FirstOrDefault();
                        scoreCount++;

                        #region score item captured

                        if (scoreRecord != null) // if value captured
                        {
                            completedPeriodCount++;
                            var scoreItem = scoreRecord;

                            #region EAI score

                            if (templateItem.ScorecardScoring == 0)
                            {
                                if (scoreItem.Rating == ScorecardScoreType.E)
                                {
                                    score += templateItem.Weight * tempate.ExcellentWeight / 100;
                                    lineTotE++;
                                }
                                if (scoreItem.Rating == ScorecardScoreType.A)
                                {
                                    score += templateItem.Weight * tempate.AdequateWeight / 100;
                                    lineTotA++;
                                }
                                if (scoreItem.Rating == ScorecardScoreType.I)
                                {
                                    score += templateItem.Weight * tempate.InadequateWeight / 100;
                                    lineTotI++;
                                }
                                sheet.Cells[rowIndex, fo + 2 + periodIndex * 2].Value += " " + scoreItem.Rating.ToString();
                            }

                            #endregion EAI score

                            #region Manual score

                            else
                            {
                                if (scoreItem.Value == null) scoreItem.Value = 0;
                                sheet.Cells[rowIndex, fo + 2 + periodIndex * 2].Value += " " + Convert.ToInt16(scoreItem.Value); // + " / " + Convert.ToInt16(templateItem.Maximum);
                                                                                                                                 //Walter stated that the score entered is already the weited score so he does not want to apply weight here
                                                                                                                                 //score = Math.Round(Convert.ToDecimal(scoreItem.Value / templateItem.Maximum * templateItem.Weight),2);
                                                                                                                                 // After discusisons desided on direct approuch
                                score += scoreItem.Value;
                                totManual += scoreItem.Value;
                            }

                            #endregion Manual score

                            #region ScoreItem Comment

                            sheet.Cells[rowIndex, (periodIndex + numPeriods) * 2 + 8 + fo].Value += scorecard.Evaluator.FirstName + " " + scorecard.Evaluator.Surname + " - " + ConvertHtmlToText(scoreRecord.EvaluatorHtmlComment) + "\n";
                            sheet.Cells[rowIndex, (periodIndex + numPeriods) * 2 + 9 + fo].Value += ConvertHtmlToText(scoreRecord.EmployeeHtmlComment) + "\n";

                            #endregion ScoreItem Comment
                        }
                    }
                    if (scoreCount > 0)
                    {
                        scoretotals[periodIndex] += (decimal)(score / scoreCount);
                        lineTotal += (decimal)(score / scoreCount);
                        lineTotalCount++;
                        sheet.Cells[rowIndex, fo + 3 + periodIndex * 2].Value = score / scoreCount;
                        sheet.Cells[rowIndex, fo + 3 + periodIndex * 2].Style.Numberformat.Format = "0.00";
                    }

                    #endregion score item captured

                    periodIndex++;
                }

                #endregion for each period

                #region line totals for period

                if (completedPeriodCount > 0)
                {
                    sheet.Cells[rowIndex, fo + 5 + numPeriods * 2].Value = templateItem.Weight;
                    if (templateItem.ScorecardScoring == 0)
                    {
                        var score = lineTotal / lineTotalCount;
                        //                        var score = templateItem.Weight * (lineTotE * tempate.ExcellentWeight + lineTotA * tempate.AdequateWeight + lineTotI * tempate.InadequateWeight) / (completedPeriodCount) / 100;
                        sheet.Cells[rowIndex, fo + 2 + numPeriods * 2].Value = lineTotE;
                        sheet.Cells[rowIndex, fo + 3 + numPeriods * 2].Value = lineTotA;
                        sheet.Cells[rowIndex, fo + 4 + numPeriods * 2].Value = lineTotI;
                        sheet.Cells[rowIndex, fo + 6 + numPeriods * 2].Value = score;
                        sheet.Cells[rowIndex, fo + 6 + numPeriods * 2].Style.Numberformat.Format = "0.00";
                        sheet.Cells[rowIndex, fo + 7 + numPeriods * 2].Style.WrapText = true;
                        totE += lineTotE;
                        totA += lineTotA;
                        totI += lineTotI;
                    }
                    else
                    {
                        sheet.Cells[rowIndex, fo + 2 + numPeriods * 2].Value = "n/a";
                        sheet.Cells[rowIndex, fo + 3 + numPeriods * 2].Value = "n/a";
                        sheet.Cells[rowIndex, fo + 4 + numPeriods * 2].Value = "n/a";
                        sheet.Cells[rowIndex, fo + 6 + numPeriods * 2].Value = lineTotal / lineTotalCount;
                        //                        sheet.Cells[rowIndex, fo + 6 + numPeriods * 2].Value = totManual / completedPeriodCount;
                        sheet.Cells[rowIndex, fo + 6 + numPeriods * 2].Style.Numberformat.Format = "0.00";
                        //  sheet.Cells[rowIndex, fo + 7 + numPeriods * 2].Value = string.Format("{0}", templateItem.ManualDefinition);
                    }
                }

                #endregion line totals for period

                rowIndex++;
            }

            #endregion scored items

            #region line totals for scorecard

            printFilterColumn(employee, evaluator, team, group, rowIndex, ref sheet);

            var totalCount = 0;
            sheet.Cells[rowIndex, fo + 1].Value = "SUB TOTAL";
            for (var i = 0; i < numPeriods; i++)
            {
                if (scoretotals[i] != 0)
                {
                    totalCount++;
                    totScore += scoretotals[i];
                    sheet.Cells[rowIndex, fo + i * 2 + 3].Value = scoretotals[i];
                    sheet.Cells[rowIndex, fo + i * 2 + 3].Style.Numberformat.Format = "0.00";
                }
            }
            sheet.Cells[rowIndex, fo + 2 + numPeriods * 2].Value = totE;
            sheet.Cells[rowIndex, fo + 3 + numPeriods * 2].Value = totA;
            sheet.Cells[rowIndex, fo + 4 + numPeriods * 2].Value = totI;
            sheet.Cells[rowIndex, fo + 5 + numPeriods * 2].Value = totWeight;

            if (totalCount > 0)
            {
                sheet.Cells[rowIndex, fo + 6 + numPeriods * 2].Value = totScore / totalCount;
            }
            else
            {
                sheet.Cells[rowIndex, fo + 6 + numPeriods * 2].Value = 0;
            }

            sheet.Cells[rowIndex, fo + 6 + numPeriods * 2].Style.Numberformat.Format = "0.00";

            //            sheet.Cells[rowIndex, fo + 6 + numPeriods * 2].Value = Math.Round(Convert.ToDecimal(totScore), 2);

            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);
            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);

            #endregion line totals for scorecard

            #region Evaluator & Employee Comments

            //            sheet.Cells[rowIndex, fo + 1].Value = "Period";
            //            sheet.Cells[rowIndex, fo + 2].Value = "Evaluator Comment (General)";
            //            sheet.Cells[rowIndex, fo + 2, rowIndex, fo + 4].Merge = true;
            //            sheet.Cells[rowIndex, fo + 5].Value = "Employee Comments (General)";
            //            sheet.Cells[rowIndex, fo + 5, rowIndex, fo + 7].Merge = true;
            //Heading
            using (var rng = sheet.Cells[rowIndex, fo + 2 * numPeriods + 8, rowIndex, fo + 4 * numPeriods + 7])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200)); //Set color to grey
                rng.Style.Font.Color.SetColor(Color.White);
            }

            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);
            periodIndex = 0;
            foreach (var period in periods)
            {
                sheet.Cells[rowIndex, (periodIndex + numPeriods) * 2 + 8 + fo].Style.WrapText = true;
                sheet.Cells[rowIndex, (periodIndex + numPeriods) * 2 + 9 + fo].Style.WrapText = true;
                foreach (var scorecard in scorecards.Where(s => s.ScorecardTemplatePeriodId == period.Id))
                {
                    sheet.Cells[rowIndex, (periodIndex + numPeriods) * 2 + 8 + fo].Value += scorecard.Evaluator.FirstName + " " + scorecard.Evaluator.Surname + " - " + ConvertHtmlToText(scorecard.EvaluatorMessage) + "\n";
                    sheet.Cells[rowIndex, (periodIndex + numPeriods) * 2 + 9 + fo].Value += ConvertHtmlToText(scorecard.EmployeeMessage) + "\n";
                }
                periodIndex++;
            }

            #endregion Evaluator & Employee Comments

            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);
            printFilterColumn(employee, evaluator, team, group, rowIndex++, ref sheet);
        }

        private void printFilterColumn(UserIdentity employee, UserIdentity evaluator, String team, String group, int index, ref ExcelWorksheet sheet)
        {
            sheet.Cells[index, 1].Value = string.Format("{0} {1}", employee.FirstName, employee.Surname);
            sheet.Cells[index, 2].Value = string.Format("{0}", group);
            sheet.Cells[index, 3].Value = string.Format("{0}", team);

            if (evaluator != null)
            {
                sheet.Cells[index, 4].Value = string.Format("{0}", "Detailed");
                sheet.Cells[index, 5].Value = string.Format("{0} {1}", evaluator.FirstName, evaluator.Surname);
            }
            else
            {
                sheet.Cells[index, 4].Value = string.Format("{0}", "Final Combined");
                sheet.Cells[index, 5].Value = string.Format("{0}", "(Multiple)");
            }
        }

        #region depricated

        /* Old Implementation - Deprecated */

        private void GenerateScorecard(ScorecardTemplate tempate, List<ScorecardTemplatePeriod> periods, List<Scorecard> scorecards, UserIdentity employee, UserIdentity evaluator, ref int rowIndex, ref ExcelWorksheet sheet)
        {
            int numPeriods = periods.Count;

            //Draw border around scorecard
            using (var rng = sheet.Cells[rowIndex, 1, rowIndex + tempate.ScorecardTemplateItems.Count + scorecards.Count + 9, numPeriods * 2 + 8])
            {
                rng.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            }
            //Heading
            using (var rng = sheet.Cells[rowIndex + 2, 1, rowIndex + 2, numPeriods * 2 + 7])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to light blue
            }
            //Heading
            using (var rng = sheet.Cells[rowIndex + 3, 1, rowIndex + 3, numPeriods * 2 + 7])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200)); //Set color to grey
                rng.Style.Font.Color.SetColor(Color.White);
            }

            //Total row
            using (var rng = sheet.Cells[rowIndex + 6 + tempate.ScorecardTemplateItems.Count, 1, rowIndex + 6 + tempate.ScorecardTemplateItems.Count, numPeriods * 2 + 7])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to light blue
            }

            //Draw border around each period
            for (var i = 0; i < numPeriods; i++)
                //Border
                using (var rng = sheet.Cells[rowIndex + 1, i * 2 + 2, rowIndex + tempate.ScorecardTemplateItems.Count + 6, i * 2 + 3])
                {
                    rng.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                }

            #region 1st Row

            sheet.Cells[rowIndex, 1].Value = string.Format("Employee: {0} {1}", employee.FirstName, employee.Surname);
            sheet.Cells[rowIndex, 1, rowIndex, 2].Merge = true;
            sheet.Cells[rowIndex, 3].Value = string.Format("Evaluator: {0} {1}", evaluator.FirstName, evaluator.Surname);
            sheet.Cells[rowIndex, 3, rowIndex, 4].Merge = true;
            rowIndex++;

            #endregion 1st Row

            #region 2nd Row, 3rd Row (blank), 4th Row, 5th Row

            sheet.Cells[rowIndex, 1].Value = string.Format("Score Card: {0}", tempate.ScorecardName);
            int colIndex = 2;
            foreach (var period in periods)
            {
                sheet.Cells[rowIndex, colIndex].Value = string.Format("Period: {0}", period.Description);
                using (var rng = sheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1])
                {
                    rng.Style.Font.Bold = true;
                    rng.Merge = true;
                }
                sheet.Cells[rowIndex + 2, colIndex + 1].Value = string.Format("Score");
                colIndex++; colIndex++;
            }
            sheet.Cells[rowIndex + 2, 2 + numPeriods * 2].Value = string.Format("E");
            sheet.Cells[rowIndex + 3, 2 + numPeriods * 2].Value = string.Format("{0}%", tempate.ExcellentWeight);
            sheet.Cells[rowIndex + 2, 3 + numPeriods * 2].Value = string.Format("A");
            sheet.Cells[rowIndex + 3, 3 + numPeriods * 2].Value = string.Format("{0}%", tempate.AdequateWeight);
            sheet.Cells[rowIndex + 2, 4 + numPeriods * 2].Value = string.Format("I");
            sheet.Cells[rowIndex + 3, 4 + numPeriods * 2].Value = string.Format("{0}%", tempate.InadequateWeight);
            sheet.Cells[rowIndex + 2, 5 + numPeriods * 2].Value = string.Format("Wt.");
            sheet.Cells[rowIndex + 3, 5 + numPeriods * 2].Value = string.Format("%");
            sheet.Cells[rowIndex + 2, 6 + numPeriods * 2].Value = string.Format("YTD Score");

            rowIndex++; rowIndex++; rowIndex++; rowIndex++; rowIndex++;

            #endregion 2nd Row, 3rd Row (blank), 4th Row, 5th Row

            decimal[] scoretotals = new decimal[numPeriods];
            decimal? totE = 0;
            decimal? totA = 0;
            decimal? totI = 0;
            decimal? totWeight = 0;
            decimal? totScore = 0;

            #region scored items

            foreach (var templateItem in tempate.ScorecardTemplateItems.OrderBy(t => t.Order))
            {
                sheet.Cells[rowIndex, 1].Value = templateItem.Description;
                totWeight += templateItem.Weight;
                colIndex = 2;

                #region for each period

                decimal? lineTotE = 0;
                decimal? lineTotA = 0;
                decimal? lineTotI = 0;
                decimal? totManual = 0;
                int completedPeriodCount = 0;
                int periodIndex = 0;
                foreach (var period in periods)
                {
                    ScorecardRecord scoreRecord = null;
                    var scorecard = scorecards.Where(s => s.ScorecardTemplatePeriod.Id == period.Id).FirstOrDefault();
                    if (scorecard != null)
                        scoreRecord = scorecard.ScorecardRecords.Where(s => s.ScorecardTemplateItemId == templateItem.Id).FirstOrDefault();

                    decimal? score = 0;

                    #region score item captured

                    if (scoreRecord != null) // if value captured
                    {
                        completedPeriodCount++;
                        var scoreItem = scoreRecord;

                        #region EAI score

                        if (templateItem.ScorecardScoring == 0)
                        {
                            if (scoreItem.Rating == ScorecardScoreType.E)
                            {
                                score = templateItem.Weight * tempate.ExcellentWeight / 100;
                                lineTotE++;
                            }
                            if (scoreItem.Rating == ScorecardScoreType.A)
                            {
                                score = templateItem.Weight * tempate.AdequateWeight / 100;
                                lineTotA++;
                            }
                            if (scoreItem.Rating == ScorecardScoreType.I)
                            {
                                score = templateItem.Weight * tempate.InadequateWeight / 100;
                                lineTotI++;
                            }
                            sheet.Cells[rowIndex, 2 + periodIndex * 2].Value = scoreItem.Rating.ToString();
                        }

                        #endregion EAI score

                        #region Manual score

                        else
                        {
                            if (scoreItem.Value == null) scoreItem.Value = 0;
                            sheet.Cells[rowIndex, 2 + periodIndex * 2].Value = Convert.ToInt16(scoreItem.Value) + " / " + Convert.ToInt16(templateItem.Maximum);
                            //Walter stated that the score entered is already the weited score so he does not want to apply weight here
                            //score = Math.Round(Convert.ToDecimal(scoreItem.Value / templateItem.Maximum * templateItem.Weight),2);
                            // After discusisons desided on direct approuch
                            score = scoreItem.Value;
                            totManual += score;
                        }
                        scoretotals[periodIndex] += (decimal)score;
                        sheet.Cells[rowIndex, 3 + periodIndex * 2].Value = score;

                        #endregion Manual score
                    }

                    #endregion score item captured

                    periodIndex++;
                }

                #endregion for each period

                #region line totals for period

                if (completedPeriodCount > 0)
                {
                    sheet.Cells[rowIndex, 5 + numPeriods * 2].Value = templateItem.Weight;
                    if (templateItem.ScorecardScoring == 0)
                    {
                        var score = templateItem.Weight * (lineTotE * tempate.ExcellentWeight + lineTotA * tempate.AdequateWeight + lineTotI * tempate.InadequateWeight) / (completedPeriodCount) / 100;
                        sheet.Cells[rowIndex, 2 + numPeriods * 2].Value = lineTotE;
                        sheet.Cells[rowIndex, 3 + numPeriods * 2].Value = lineTotA;
                        sheet.Cells[rowIndex, 4 + numPeriods * 2].Value = lineTotI;
                        sheet.Cells[rowIndex, 6 + numPeriods * 2].Value = Math.Round(Convert.ToDecimal(score), 2);
                        sheet.Cells[rowIndex, 7 + numPeriods * 2].Style.WrapText = true;
                        sheet.Cells[rowIndex, 7 + numPeriods * 2].Value = string.Format("[E] {0}\n[A] {1}\n[I] {2}", templateItem.ExcellentDefinition, templateItem.AdequateDefinition, templateItem.InadequateDefinition);
                        totE += lineTotE;
                        totA += lineTotA;
                        totI += lineTotI;
                        totScore += score;
                    }
                    else
                    {
                        sheet.Cells[rowIndex, 2 + numPeriods * 2].Value = "n/a";
                        sheet.Cells[rowIndex, 3 + numPeriods * 2].Value = "n/a";
                        sheet.Cells[rowIndex, 4 + numPeriods * 2].Value = "n/a";
                        sheet.Cells[rowIndex, 6 + numPeriods * 2].Value = Math.Round(Convert.ToDecimal(totManual / (completedPeriodCount)), 2);
                        sheet.Cells[rowIndex, 7 + numPeriods * 2].Value = string.Format("{0}", templateItem.ManualDefinition);
                        totScore += totManual / (completedPeriodCount);
                    }
                }

                #endregion line totals for period

                rowIndex++;
            }

            #endregion scored items

            #region line totals for scorecard

            sheet.Cells[rowIndex, 1].Value = "SUB TOTAL";
            for (var i = 0; i < numPeriods; i++)
            {
                sheet.Cells[rowIndex, i * 2 + 3].Value = scoretotals[i];
            }
            sheet.Cells[rowIndex, 2 + numPeriods * 2].Value = totE;
            sheet.Cells[rowIndex, 3 + numPeriods * 2].Value = totA;
            sheet.Cells[rowIndex, 4 + numPeriods * 2].Value = totI;
            sheet.Cells[rowIndex, 5 + numPeriods * 2].Value = totWeight;
            sheet.Cells[rowIndex, 6 + numPeriods * 2].Value = Math.Round(Convert.ToDecimal(totScore), 2);

            rowIndex++;
            rowIndex++;

            #endregion line totals for scorecard

            #region Evaluator & Employee Comments

            sheet.Cells[rowIndex, 1].Value = "Period";
            sheet.Cells[rowIndex, 2].Value = "Evaluator Comment";
            sheet.Cells[rowIndex, 2, rowIndex, 4].Merge = true;
            sheet.Cells[rowIndex, 5].Value = "Employee Comments";
            sheet.Cells[rowIndex, 5, rowIndex, 7].Merge = true;
            //Heading
            using (var rng = sheet.Cells[rowIndex, 1, rowIndex, 7])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200)); //Set color to grey
                rng.Style.Font.Color.SetColor(Color.White);
            }

            foreach (var period in periods)
            {
                var scorecard = scorecards.Where(s => s.ScorecardTemplatePeriodId == period.Id).FirstOrDefault();
                if (scorecard != null)
                {
                    rowIndex++;
                    sheet.Cells[rowIndex, 2, rowIndex, 4].Merge = true;
                    sheet.Cells[rowIndex, 5, rowIndex, 7].Merge = true;
                    sheet.Cells[rowIndex, 1].Value = period.Description;

                    sheet.Cells[rowIndex, 2].Value = scorecard.EvaluatorMessage;
                    sheet.Cells[rowIndex, 5].Value = scorecard.EmployeeMessage;
                }
            }

            #endregion Evaluator & Employee Comments

            rowIndex++;
            rowIndex++;
            rowIndex++;
            rowIndex++;
        }

        #endregion depricated

        private void GenerateMainScorecardSummary(Guid[] scorecardTemplatePeriodsIds, ExcelWorksheet sheet,
            ScorecardTemplate scorecardTemplate, UserIdentity employee, IEnumerable<UserAccount> evaluatorsList,
            ICollection<ProjectAllocationModel> scorecardItemsForEmployee)
        {
            throw new NotImplementedException();
        }

        private decimal CalculateScoreRatio(decimal ratio, decimal quantity)
        {
            return ratio * quantity;
        }

        #endregion Scorecards

        #region Timesheet Additional Methods

        private static void SetupProjectSheet(
            ExcelPackage excelPackage,
            IQueryable<Project> result,
            out List<ProjectTimesheetSummaryModel> models,
            out ExcelWorksheet sheet, bool showHeaders = true, bool addSheet = true)
        {
            var returnModel = result.Select(a => new ProjectTimesheetSummaryModel
            {
                Id = a.Id,
                ProjectLeadId = a.ProjectLeadId,
                ProjectName = a.ProjectName,
                Billable = a.Billable
            }).ToList();
            models = returnModel;

            if (addSheet)
            {
                var returnSheet = excelPackage.Workbook.Worksheets.Add("Projects");
                returnSheet.Cells[1, 1].LoadFromCollection(returnModel, showHeaders);

                if (showHeaders)
                    SetHeaderBackground(ref returnSheet, typeof(ProjectTimesheetSummaryModel).GetProperties().Length);

                AutoWidthColumns(ref returnSheet);
                HideColumns(ref returnSheet, typeof(ProjectTimesheetSummaryModel));

                sheet = returnSheet;
            }
            else
            {
                sheet = null;
            }
        }

        private static void SetupTimesheetEntrySheetStoreProc(
            ExcelPackage excelPackage,
            List<TimesheetReportProcedureModel> result,
            out List<TimesheetSummaryEntryModel> models,
            out ExcelWorksheet sheet, bool showHeaders = true, bool addSheet = true)
        {
            var returnModel = result.Select(a => new TimesheetSummaryEntryModel
            {
                ProjectId = a.ProjectIdGuid,
                UserAccountId = a.UserAccountIdGuid,
                Hours = a.Hours,
                Cost = a.Cost,
                Client = a.Client
            }).ToList();
            models = returnModel;

            if (addSheet)
            {
                var returnSheet = excelPackage.Workbook.Worksheets.Add("Timesheet Entries");
                returnSheet.Cells[1, 1].LoadFromCollection(returnModel, showHeaders);

                if (showHeaders)
                    SetHeaderBackground(ref returnSheet, typeof(TimesheetSummaryEntryModel).GetProperties().Length);

                AutoWidthColumns(ref returnSheet);
                HideColumns(ref returnSheet, typeof(TimesheetSummaryEntryModel));

                sheet = returnSheet;
            }
            else
            {
                sheet = null;
            }
        }

        private static void SetupTimesheetEntrySheet(
            ExcelPackage excelPackage,
            IQueryable<TimesheetEntry> result,
            out List<TimesheetSummaryEntryModel> models,
            out ExcelWorksheet sheet, bool showHeaders = true, bool addSheet = true)
        {
            var returnModel = result.Select(a => new TimesheetSummaryEntryModel
            {
                Id = a.Id,
                ProjectId = a.ProjectId,
                SubProjectId = a.SubProjectId,
                UserAccountId = a.UserAccountId,
                TeamId = a.TeamId,
                ActivityId = a.ActivityId,
                Comments = a.Comments,
                Hours = a.Hours,
                DateEntry = a.DateEntry,
                //Client = ClientEntityData!!!
            }).ToList();
            models = returnModel;

            if (addSheet)
            {
                var returnSheet = excelPackage.Workbook.Worksheets.Add("Timesheet Entries");
                returnSheet.Cells[1, 1].LoadFromCollection(returnModel, showHeaders);

                if (showHeaders)
                    SetHeaderBackground(ref returnSheet, typeof(TimesheetSummaryEntryModel).GetProperties().Length);

                AutoWidthColumns(ref returnSheet);
                HideColumns(ref returnSheet, typeof(TimesheetSummaryEntryModel));

                sheet = returnSheet;
            }
            else
            {
                sheet = null;
            }
        }

        #endregion Timesheet Additional Methods

        #region Static Report Actions

        private static void SetHeaderBackground(ref ExcelWorksheet sheet, int totalColumns)
        {
            using (var rng = sheet.Cells[1, 1, 1, totalColumns])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }
        }

        private static void AutoWidthColumns(ref ExcelWorksheet sheet)
        {
            if (sheet.Dimension != null)
            {
                for (var i = 1; i <= sheet.Dimension.End.Column; i++)
                {
                    if (!sheet.Column(i).Hidden)
                    {
                        sheet.Column(i).AutoFit();
                        sheet.Column(i).Width += 4;
                    }
                }
            }
        }

        private static void HideColumns(ref ExcelWorksheet sheet, Type type)
        {
            foreach (var attribute in type
                .GetProperties()
                .Select(property =>
                    (ReportHiddenColumn)Attribute.GetCustomAttribute(property, typeof(ReportHiddenColumn)))
                .Where(attribute => attribute != null))
                sheet.Column(attribute.ColumnNumber).Hidden = true;
        }

        #endregion Static Report Actions
    }
}