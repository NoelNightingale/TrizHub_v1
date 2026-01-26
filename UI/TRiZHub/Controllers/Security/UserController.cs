#region Usings

using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ContactData;
using TRiZHub.BL.Entities.OfficeEquipmentData;
using TRiZHub.BL.Entities.PersonalInformationData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.TeamJobDesignationData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.ClientEntityData;
using TRiZHub.BL.Provider.ContactData;
using TRiZHub.BL.Provider.Email;
using TRiZHub.BL.Provider.OfficeEquipmentData;
using TRiZHub.BL.Provider.PersonalInformationData;
using TRiZHub.BL.Provider.ScorecardData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Provider.TeamData;
using TRiZHub.BL.Provider.TeamJobDesignationData;
using TRiZHub.BL.Provider.TravelInformationData;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.ClientModels;
using TRiZHub.Models.ContactModels;
using TRiZHub.Models.OfficeEquipmentModels;
using TRiZHub.Models.PersonalInformationModels;
using TRiZHub.Models.SecurityData;
using TRiZHub.Models.TeamJobDesignationModels;

#endregion Usings

namespace TRiZHub.Controllers.Security
{
    [Authorize]
    [NoCache]
    public class UserController : TCRControllerBase
    {
        /// <summary>
        /// Retrieve full list of Active Users ordered by Firstname, to be used in Scorecard Employee dropdown
        /// </summary>
        [HttpGet]
        public List<UserDropdownModel> UserScorecardEmployeeFilterDropdown()
        {
            return SecurityProvider.GetUserAccountList()
                .Where(a => a.Id != CurrentUser.Id && a.Active)
                .Select(a =>
                    new UserDropdownModel
                    {
                        Id = a.Id,
                        Firstname = a.FirstName,
                        Surname = a.Surname,
                        AccountName = a.AccountName
                    }).OrderBy(a => a.Firstname).ThenBy(a => a.Surname)
                .ToList();
        }

        /// <summary>
        /// Retrieve full list of Active Users ordered by Firstname, to be used in Scorecard Evaluator dropdown
        /// </summary>
        [HttpGet]
        public List<UserDropdownModel> UserScorecardEvaluatorFilterDropdown()
        {
            return SecurityProvider.GetUserAccountList()
                .Where(a => a.Id == CurrentUser.Id)
                .Select(a =>
                    new UserDropdownModel
                    {
                        Id = a.Id,
                        Firstname = a.FirstName,
                        Surname = a.Surname,
                        AccountName = a.AccountName,
                    }).OrderBy(a => a.Firstname).ThenBy(a => a.Surname)
                .ToList();
        }

        /// <summary>
        /// Retrieve full list of Users that are evaluators ordered by Firstname, to be used in Scorecard Reports dropdown
        /// </summary>
        [HttpGet]
        public List<UserDropdownModel> UserScorecardEvaluatorsDropdown()
        {
            var evaluators = ScorecardProvider.GetAllScorecardEvaluators();
            return evaluators.Select(a =>
                    new UserDropdownModel
                    {
                        Id = a.Evaluator.Id,
                        Firstname = a.Evaluator.FirstName,
                        Surname = a.Evaluator.Surname,
                        AccountName = a.Evaluator.Active.ToString()
                    }).OrderBy(a => a.Firstname).ThenBy(a => a.Surname)
                .ToList();
        }

        /// <summary>
        /// Retrieve full list of Users that are line managers ordered by Firstname, to be used in Scorecard Reports dropdown
        /// </summary>
        [HttpGet]
        public List<UserDropdownModel> UserScorecardLineManagersDropdown()
        {
            var jobDesignations = TeamJobDesignationProvider.TeamJobDesignationtLineLeadFilterListAll();
            return jobDesignations.Select(a =>
                    new UserDropdownModel
                    {
                        Id = a.LineLeader.Id,
                        Firstname = a.LineLeader.FirstName,
                        Surname = a.LineLeader.Surname,
                        AccountName = a.LineLeader.Active.ToString()
                    }).OrderBy(a => a.Firstname).ThenBy(a => a.Surname)
                .ToList();
        }

        #region Ctor

        private SecurityProvider SecurityProvider { get; }
        private ContactProvider ContactProvider { get; }
        private IEmailProvider EmailProvider { get; }
        private IAppSettings AppSettings { get; }
        private TravelInformationProvider TravelInformationProvider { get; }
        private OfficeEquipmentProvider OfficeEquipmentProvider { get; }
        private PersonalInformationProvider PersonalInformationProvider { get; }
        private TeamJobDesignationProvider TeamJobDesignationProvider { get; }
        private ScorecardProvider ScorecardProvider { get; }
        private EmployerProvider EmployerProvider { get; }

        public UserController()
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            ContactProvider = new ContactProvider(Context, CurrentUser);
            EmailProvider = new EmailProvider(Context, AppSettings);
            AppSettings = new AppSettings(Context);
            TravelInformationProvider = new TravelInformationProvider(Context, CurrentUser);
            OfficeEquipmentProvider = new OfficeEquipmentProvider(Context, CurrentUser);
            PersonalInformationProvider = new PersonalInformationProvider(Context, CurrentUser);
            TeamJobDesignationProvider = new TeamJobDesignationProvider(Context, CurrentUser);
            ScorecardProvider = new ScorecardProvider(Context, CurrentUser);
            EmployerProvider = new EmployerProvider(Context, CurrentUser);
        }

        public UserController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            ContactProvider = new ContactProvider(Context, CurrentUser);
            EmailProvider = new EmailProvider(Context, AppSettings);
            AppSettings = new AppSettings(Context);
            TravelInformationProvider = new TravelInformationProvider(Context, CurrentUser);
            OfficeEquipmentProvider = new OfficeEquipmentProvider(Context, CurrentUser);
            PersonalInformationProvider = new PersonalInformationProvider(Context, CurrentUser);
            TeamJobDesignationProvider = new TeamJobDesignationProvider(Context, CurrentUser);
        }

        #endregion Ctor

        #region User

        /// <summary>
        /// First Register of User
        /// </summary>
        [HttpPost]
        public UserEditModel SignUp(UserEditModel model)
        {
            try
            {
                CheckModelState();

                var userIdentity = SecurityProvider.SignUp(model.Account, model.FirstName, model.Surname, null);

                var user = new UserEditModel
                {
                    Id = userIdentity.Id,
                    Account = userIdentity.AccountName,
                    FirstName = userIdentity.FirstName,
                    Surname = userIdentity.Surname
                };

                return user;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Save basic detail of user
        /// </summary>
        [HttpPost]
        public UserEditModel UserSave(UserEditModel model)
        {
            try
            {
                CheckModelState();

                var existingUser = SecurityProvider.GetUserList().SingleOrDefault(u => u.Id == model.Id);

                if (existingUser == null)
                    throw new SecurityException(
                        "User needs to go thru the Register actions... Cannot create a user from here.");

                UserIdentity userIdentity = null;
                if (SecurityProvider.UserIsAllowed(PrivilegeType.RoleMaintenance))
                {
                    userIdentity = SecurityProvider.SaveUser(
                        model.Id.Value, model.Account, model.FirstName,
                        model.Surname, model.RoleList.Where(a => a.Selected).Select(a => a.RoleId).ToList());
                }
                else
                {
                    userIdentity = SecurityProvider.SaveUser(
                       model.Id.Value, model.Account, model.FirstName, 
                       model.Surname, new List<Guid>(), false);
                }

                var user = new UserEditModel
                {
                    Id = userIdentity.Id,
                    Account = userIdentity.AccountName,
                    FirstName = userIdentity.FirstName,
                    Surname = userIdentity.Surname
                };

                if (userIdentity is UserAccount)
                {
                    user.RoleList = (userIdentity as UserAccount).Roles.Select(a => new UserRoleModel
                    {
                        RoleId = a.Id,
                        RoleName = a.RoleName,
                        Selected = false
                    }).ToList();
                }

                return user;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve full user list, can be order in any way, and filters can be applied
        /// </summary>
        [HttpPost]
        public GridResultModel<UserGridModel> UserGrid(GridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = SecurityProvider.GetUserList().Select(a => new UserGridModel
            {
                Id = a.Id,
                Account = a.AccountName,
                FirstName = a.FirstName,
                Surname = a.Surname,
                Active = a.Active,
                IsAdmin = a.IsSystemAdmin
                
            }).ToList();

            var employers = EmployerProvider.GetUserEmployer(filteredQuery.Select(u => u.Id.Value).ToList());

            // Set Employer
            foreach (var item in filteredQuery)
            {
                item.Employer = "Unassigned";

                foreach (var e in employers)
                {
                    if (item.Id == e.Key)
                    {
                        item.Employer = e.Value;
                        break;
                    }
                }
            }

            if (model.Searchfor != "null")
            {
                filteredQuery = filteredQuery.Where(r => r.Account.Contains(model.Searchfor) || r.Account.ToLower().Contains(model.Searchfor.ToLower())
                                                         || r.FirstName != null && r.FirstName.Contains(model.Searchfor) || r.FirstName != null && r.FirstName.ToLower().Contains(model.Searchfor.ToLower())
                                                         || r.Surname != null && r.Surname.Contains(model.Searchfor) || r.Surname != null && r.Surname.ToLower().Contains(model.Searchfor.ToLower())
                                                         || r.Employer.Contains(model.Searchfor) || r.Employer.ToLower().Contains(model.Searchfor.ToLower())
                    ).ToList();
            }

            if (!model.ShowInactive)
            {
                filteredQuery = filteredQuery.Where(r => r.Active == true).ToList();
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.Account).ToList(); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            //Setup sort order
            switch (model.SortOrder)
            {
                case "ASC":
                    switch (model.SortKey)
                    {
                        case "account":
                            filteredQuery = filteredQuery.OrderBy(r => r.Account).ToList();
                            break;

                        case "firstname":
                            filteredQuery = filteredQuery.OrderBy(r => r.FirstName).ThenBy(r => r.Surname).ToList();
                            break;

                        case "surname":
                            filteredQuery = filteredQuery.OrderBy(r => r.Surname).ThenBy(r => r.FirstName).ToList();
                            break;

                        case "employer":
                            filteredQuery = filteredQuery.OrderBy(r => r.Employer).ThenBy(r => r.FirstName).ToList();
                            break;
                    }
                    break;

                case "DESC":
                    switch (model.SortKey)
                    {
                        case "account":
                            filteredQuery = filteredQuery.OrderByDescending(r => r.Account).ToList();
                            break;

                        case "firstname":
                            filteredQuery = filteredQuery.OrderByDescending(r => r.FirstName).ThenByDescending(r => r.Surname).ToList();
                            break;

                        case "surname":
                            filteredQuery = filteredQuery.OrderByDescending(r => r.Surname).ThenByDescending(r => r.FirstName).ToList();
                            break;

                        case "employer":
                            filteredQuery = filteredQuery.OrderByDescending(r => r.Employer).ThenByDescending(r => r.FirstName).ToList();
                            break;
                    }
                    break;
            }

            //setup paging
            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value).ToList();

            return new GridResultModel<UserGridModel>(filteredQuery, totalNumberOfRecords);
        }

        /// <summary>
        /// Get single user based on id
        /// </summary>
        [HttpGet]
        public UserEditModel UserGet(Guid? id)
        {
            try
            {
                var model = new UserEditModel();
                var filteredQuery = SecurityProvider.GetUserList().Where(a => a.Id == id)
                    .Select(a => new UserEditModel
                    {
                        Id = a.Id,
                        Account = a.AccountName,
                        FirstName = a.FirstName,
                        Surname = a.Surname
                    });

                model = filteredQuery.Single();
                try
                {
                    model.RoleList = SecurityProvider.GetRoles().Select(a => new UserRoleModel
                    {
                        RoleId = a.Id,
                        RoleName = a.RoleName,
                        Selected = false
                    }).ToList();

                    var selectedRoles = SecurityProvider.GetUserRoles(id.Value).Select(a => a.Id).ToArray();

                    foreach (var itm in model.RoleList.Where(a => selectedRoles.Contains(a.RoleId)).ToList())
                    {
                        itm.Selected = true;
                    }
                }
                catch
                {
                }

                return model;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion User

        #region Activation/Deactivation

        /// <summary>
        /// Activate user based on id
        /// </summary>
        [HttpPost]
        public UserEditModel ActivateUser(KeyValueModel item)
        {
            try
            {
                SecurityProvider.ActivateAccount(item.Id);
                return null;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Deactivate user based on id
        /// </summary>
        [HttpPost]
        public UserEditModel DeactivateUser(KeyValueModel item)
        {
            try
            {
                SecurityProvider.DeactivateAccount(item.Id);
                return null;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion Activation/Deactivation

        #region Emergency Contact

        /// <summary>
        /// Save Emergency Contact info linked to a User
        /// </summary>
        [HttpPost]
        public EmergencyContactEditModel EmergencyContactSave(EmergencyContactEditModel model)
        {
            try
            {
                CheckModelState();

                var emergencyContact = ContactProvider.SaveEmergancyContact(model.Id,
                    model.UserAccountId, model.Name, model.Surname,
                    model.Relationship, model.CellphoneNumber, model.LandLineNumber);

                var emergencyContactReturn = new EmergencyContactEditModel
                {
                    Id = emergencyContact.Id,
                    UserAccountId = emergencyContact.UserAccountId,
                    Name = emergencyContact.Name,
                    Surname = emergencyContact.Surname,
                    Relationship = emergencyContact.Relationship,
                    CellphoneNumber = emergencyContact.CellphoneNumber,
                    LandLineNumber = emergencyContact.LandLineNumber
                };

                return emergencyContactReturn;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Get Single Emergency Contact based on id
        /// </summary>
        [HttpGet]
        public EmergencyContactEditModel EmergencyContactGet(Guid? id)
        {
            try
            {
                var emergencyContact = ContactProvider.GetEmergancyContact(id.Value);

                if (emergencyContact == null)
                    emergencyContact = new EmergancyContact();

                var model = new EmergencyContactEditModel
                {
                    Id = emergencyContact.Id,
                    UserAccountId = emergencyContact.UserAccountId,
                    Name = emergencyContact.Name,
                    Surname = emergencyContact.Surname,
                    Relationship = emergencyContact.Relationship,
                    CellphoneNumber = emergencyContact.CellphoneNumber,
                    LandLineNumber = emergencyContact.LandLineNumber
                };
                return model;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Single Emergency contact
        /// </summary>
        [HttpPost]
        public void EmergencyContactDelete(EmergencyContactEditModel model)
        {
            try
            {
                ContactProvider.DeleteEmergencyContact(model.Id ?? Guid.Empty);
            }
            catch (ContactException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve full EmergencyContact list for a user
        /// </summary>
        [HttpPost]
        public GridResultModel<EmergencyContactGridModel> EmergencyContactGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var user = SecurityProvider.GetUserList().Where(a => a.Id == model.Id.Value).Single();

            var filteredQuery = ContactProvider.EmergencyContactFilterList(model.Id ?? Guid.Empty)
                .Select(a => new EmergencyContactGridModel
                {
                    Id = a.Id,
                    UserAccountId = a.UserAccountId,
                    Account = user.AccountName,
                    FirstName = a.Name,
                    Surname = a.Surname,
                    Name = a.Name,
                    Relationship = a.Relationship,
                    CellphoneNumber = a.CellphoneNumber,
                    LandLineNumber = a.LandLineNumber,
                });

            var totalNumberOfRecords = filteredQuery.Count();

            filteredQuery = filteredQuery.OrderBy(a => a.Name);

            var returnList = filteredQuery.Skip(0).Take(12).ToList();

            return new GridResultModel<EmergencyContactGridModel>(returnList, totalNumberOfRecords);
        }

        #endregion Emergency Contact

        /// <summary>
        /// This call expects a full Import list on excel format of officeEquiment/Asset Register entries
        /// </summary>
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> UploadImportOfficeEquiment()
        {
            //List<OfficeEquipmentEditModel> exceptionList = new List<OfficeEquipmentEditModel>();
            List<String> exceptionList = new List<String>();
            Int64 totalCount = 0;
            Int64 successCount = 0;

            ExcelPackage officeEquimentPackage = null;
            if (Request.Content.IsMimeMultipartContent())
            {
                var streamProvider = new MultipartMemoryStreamProvider();

                await Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);

                    // drop curent table
                    OfficeEquipmentProvider.DropOfficeEquipment();

                    List<OfficeEquipmentEditModel> assetRegisterList = new List<OfficeEquipmentEditModel>();
                    foreach (HttpContent cntn in streamProvider.Contents)
                    {
                        officeEquimentPackage = new ExcelPackage(cntn.ReadAsStreamAsync().Result);
                        ExcelWorksheet officeEquimentSheet = officeEquimentPackage.Workbook.Worksheets[1];
                        var start = officeEquimentSheet.Dimension.Start;
                        var end = officeEquimentSheet.Dimension.End;

                        if (!Context.IsTransactionActive())
                            Context.BeginTransaction();

                        for (int row = (start.Row + 1); row <= end.Row; row++)
                        { // Row by row...
                            try
                            {
                                totalCount++;

                                if (officeEquimentSheet.Cells[row, 3].Text == "")
                                {
                                    exceptionList.Add("Row " + row.ToString() + "'s Asset Type is missing.");
                                }
                                else if (officeEquimentSheet.Cells[row, 5].Text == "")
                                {
                                    exceptionList.Add("Row " + row.ToString() + "'s Supplier Name is missing.");
                                }
                                else if (officeEquimentSheet.Cells[row, 6].Text == "")
                                {
                                    exceptionList.Add("Row " + row.ToString() + "'s Serial Number is missing.");
                                }
                                else if (officeEquimentSheet.Cells[row, 7].Text == "")
                                {
                                    exceptionList.Add("Row " + row.ToString() + "'s Cost is missing.");
                                }
                                else if (officeEquimentSheet.Cells[row, 8].Text == "")
                                {
                                    exceptionList.Add("Row " + row.ToString() + "'s Purchase Date is missing.");
                                }
                                else if (officeEquimentSheet.Cells[row, 9].Text == "")
                                {
                                    exceptionList.Add("Row " + row.ToString() + "'s Invoice Number is missing.");
                                }
                                else if (officeEquimentSheet.Cells[row, 10].Text == "")
                                {
                                    exceptionList.Add("Row " + row.ToString() + "'s Assigned Date is missing.");
                                }
                                else if (officeEquimentSheet.Cells[row, 12].Text == "")
                                {
                                    exceptionList.Add("Row " + row.ToString() + "'s Asset Register Code is missing.");
                                }
                                else
                                {
                                    // Check Date and Numeric values are coorect format
                                    bool parseFailed = false;
                                    decimal costOut;
                                    if (!Decimal.TryParse(officeEquimentSheet.Cells[row, 7].Text, out costOut))
                                    {
                                        parseFailed = true;
                                        exceptionList.Add("Row " + row.ToString() + "'s Cost is wrong format for price (Should be 0000.00).");
                                    }
                                    else
                                    {
                                        try
                                        {
                                            DateTime.Parse(officeEquimentSheet.Cells[row, 8].Text);
                                        }
                                        catch
                                        {
                                            exceptionList.Add("Row " + row.ToString() + "'s Purchase Date is wrong format for date (Should be dd/mm/yyyy).");
                                            parseFailed = true;
                                        }

                                        if (!parseFailed)
                                        {
                                            try
                                            {
                                                DateTime.Parse(officeEquimentSheet.Cells[row, 8].Text);
                                            }
                                            catch
                                            {
                                                exceptionList.Add("Row " + row.ToString() + "'s Purchase Date is wrong format for date (Should be dd/mm/yyyy).");
                                                parseFailed = true;
                                            }
                                        }
                                    }

                                    if (!parseFailed)
                                    {
                                        try
                                        {
                                            var officeEquipementReturn = new OfficeEquipmentEditModel();
                                            officeEquipementReturn.UserAccountId = System.Guid.Parse(officeEquimentSheet.Cells[row, 1].Text);
                                            officeEquipementReturn.Type = officeEquimentSheet.Cells[row, 3].Text;
                                            officeEquipementReturn.Model = officeEquimentSheet.Cells[row, 4].Text;
                                            officeEquipementReturn.SupplierName = officeEquimentSheet.Cells[row, 5].Text;
                                            officeEquipementReturn.SerialNumber = officeEquimentSheet.Cells[row, 6].Text;
                                            officeEquipementReturn.Cost = (officeEquimentSheet.Cells[row, 7].Text == "") ? 0 : Convert.ToDecimal(officeEquimentSheet.Cells[row, 7].Text);
                                            officeEquipementReturn.PurchaseDate = (officeEquimentSheet.Cells[row, 8].Text == "") ? DateTime.Now : DateTime.Parse(officeEquimentSheet.Cells[row, 8].Text);
                                            officeEquipementReturn.InvoiceNumber = officeEquimentSheet.Cells[row, 9].Text;
                                            officeEquipementReturn.AssignedDate = (officeEquimentSheet.Cells[row, 10].Text == "") ? (DateTime?)null : DateTime.Parse(officeEquimentSheet.Cells[row, 10].Text);
                                            officeEquipementReturn.ReturnDate = (officeEquimentSheet.Cells[row, 11].Text == "") ? (DateTime?)null : DateTime.Parse(officeEquimentSheet.Cells[row, 11].Text);
                                            officeEquipementReturn.AssetRegister = officeEquimentSheet.Cells[row, 12].Text;
                                            officeEquipementReturn.IsAccountingItem = (officeEquimentSheet.Cells[row, 13].Text.ToUpper().Equals("YES")) ? true : false;
                                            officeEquipementReturn.Notes = officeEquimentSheet.Cells[row, 14].Text;
                                            assetRegisterList.Add(officeEquipementReturn);

                                            OfficeEquipmentProvider.SaveOfficeEquipemnt(new Guid(), officeEquipementReturn.UserAccountId,
                                                officeEquipementReturn.Type, officeEquipementReturn.Model, officeEquipementReturn.SupplierName,
                                                officeEquipementReturn.SerialNumber, officeEquipementReturn.Cost, officeEquipementReturn.PurchaseDate,
                                                officeEquipementReturn.InvoiceNumber, officeEquipementReturn.AssignedDate, officeEquipementReturn.ReturnDate,
                                                officeEquipementReturn.AssetRegister, officeEquipementReturn.Notes, officeEquipementReturn.IsAccountingItem);
                                            successCount++;
                                        }
                                        catch (Exception exp)
                                        {
                                            exceptionList.Add("Row  " + row.ToString() + " failed for unknown reason.");
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                exceptionList.Add("Row  " + row.ToString() + " failed with following error." + e.Message);
                            }
                        }
                        Context.CommitTransaction();
                    }
                });
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request content is not properly formatted"));
            }

            if (exceptionList == null)
            {
                exceptionList.Insert(0, "Imported " + successCount.ToString() + "assets successfully.");
                return Ok(exceptionList);
            }
            else
            {
                Int64 errorCount = exceptionList.Count;
                exceptionList.Insert(0, "Successfully Imported " + successCount.ToString() + " out of " + totalCount.ToString() + " assets.");
                exceptionList.Insert(1, "Error Imported " + errorCount.ToString() + " out of " + totalCount.ToString() + " assets.");
                return Ok(exceptionList);
            }
        }

        #region Office Equipemnt

        /// <summary>
        /// Save single officeequipment entry
        /// </summary>
        [HttpPost]
        public OfficeEquipmentEditModel OfficeEquipmentSave(OfficeEquipmentEditModel model)
        {
            try
            {
                CheckModelState();

                var officeEquipement = OfficeEquipmentProvider.SaveOfficeEquipemnt(model.Id,
                  model.UserAccountId,
                  model.Type,
                  model.Model,
                  model.SupplierName,
                  model.SerialNumber,
                  model.Cost,
                  model.PurchaseDate.ToLocalTime(),
                  model.InvoiceNumber,
                  (model.AssignedDate == null ? model.AssignedDate : model.AssignedDate.Value.ToLocalTime()),
                  (model.ReturnDate == null ? model.ReturnDate : model.ReturnDate.Value.ToLocalTime()),
                  model.AssetRegister,
                  model.Notes,
                  model.IsAccountingItem);

                var officeEquipementReturn = new OfficeEquipmentEditModel
                {
                    Id = officeEquipement.Id,
                    UserAccountId = officeEquipement.UserAccountId,
                    Type = officeEquipement.Type,
                    Model = officeEquipement.Model,
                    SupplierName = officeEquipement.SupplierName,
                    SerialNumber = officeEquipement.SerialNumber,
                    Cost = officeEquipement.Cost,
                    PurchaseDate = officeEquipement.PurchaseDate,
                    InvoiceNumber = officeEquipement.InvoiceNumber,
                    AssignedDate = officeEquipement.AssignedDate,
                    ReturnDate = officeEquipement.ReturnDate,
                    AssetRegister = officeEquipement.AssetRegister,
                    Notes = officeEquipement.Notes,
                    IsAccountingItem = officeEquipement.IsAccountingItem
                };

                return officeEquipementReturn;
            }
            catch (OfficeEquipmentException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve single office equipment base on id
        /// </summary>
        [HttpGet]
        public OfficeEquipmentEditModel OfficeEquipmentGet(Guid? id)
        {
            try
            {
                var officeEquipment = OfficeEquipmentProvider.GetOfficeEquipment(id.Value);

                if (officeEquipment == null)
                    officeEquipment = new OfficeEquipment();

                //  var user = SecurityProvider.GetUserList().Where(a => a.Id == id).Single();

                var model = new OfficeEquipmentEditModel
                {
                    UserAccountId = officeEquipment.UserAccountId,
                    Account = officeEquipment.UserAccount.AccountName,
                    FirstName = officeEquipment.UserAccount.FirstName,
                    Surname = officeEquipment.UserAccount.Surname,
                    Id = officeEquipment.Id,
                    Type = officeEquipment.Type,
                    Model = officeEquipment.Model,
                    SupplierName = officeEquipment.SupplierName,
                    SerialNumber = officeEquipment.SerialNumber,
                    Cost = officeEquipment.Cost,
                    PurchaseDate = officeEquipment.PurchaseDate,
                    InvoiceNumber = officeEquipment.InvoiceNumber,
                    AssignedDate = officeEquipment.AssignedDate,
                    ReturnDate = officeEquipment.ReturnDate,
                    AssetRegister = officeEquipment.AssetRegister,
                    Notes = officeEquipment.Notes,
                    IsAccountingItem = officeEquipment.IsAccountingItem
                };
                return model;
            }
            catch (OfficeEquipmentException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete single office equipment entry based on id
        /// </summary>
        [HttpPost]
        public void OfficeEquipmentDelete(OfficeEquipmentEditModel model)
        {
            try
            {
                OfficeEquipmentProvider.DeleteOfficeEquipemnt(model.Id ?? Guid.Empty);
            }
            catch (OfficeEquipmentException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve full list of office equipment/asset register entries ordered by Assigned Date
        /// </summary>
        [HttpPost]
        public GridResultModel<OfficeEquipmentGridModel> OfficeEquipmentGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = OfficeEquipmentProvider.OfficeEquipementFilterList(model.Id ?? Guid.Empty)
                .Select(a => new OfficeEquipmentGridModel
                {
                    Id = a.Id,
                    UserAccountId = a.UserAccountId,
                    Type = a.Type,
                    Model = a.Model,
                    supplierName = a.SupplierName,
                    SerialNumber = a.SerialNumber,
                    Cost = a.Cost,
                    PurchaseDate = a.PurchaseDate,
                    InvoiceNumber = a.InvoiceNumber,
                    AssignedDate = a.AssignedDate,
                    ReturnDate = a.ReturnDate.Value,
                    AssetRegister = a.AssetRegister
                });

            var totalNumberOfRecords = filteredQuery.Count();

            filteredQuery = filteredQuery.OrderBy(a => a.AssignedDate);

            var returnList = filteredQuery.ToList();

            return new GridResultModel<OfficeEquipmentGridModel>(returnList, totalNumberOfRecords);
        }

        #endregion Office Equipemnt

        #region TeamJobDesignation

        /// <summary>
        /// Save team leader for a user, can only have one team leader in a given period
        /// </summary>
        [HttpPost]
        public TeamJobDesignationEditModel TeamJobDesignationSave(TeamJobDesignationEditModel model)
        {
            try
            {
                CheckModelState();

                var record = TeamJobDesignationProvider.SaveTeamJobDesignation(
                    model.Id,
                    model.UserAccountId,
                    model.JobDesignation,
                    new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0),
                    (model.EndDate == null ? model.EndDate : new DateTime(model.EndDate.Value.Year, model.EndDate.Value.Month, model.EndDate.Value.Day, 0, 0, 0)),
                    model.Location,
                    model.LineLeaderId,
                    model.ClientId,
                    model.EmployerId);

                var result = TeamJobDesignationProvider.GetTeamJobDesignation(record.Id);

                var teamJobDesignationReturn = new TeamJobDesignationEditModel
                {
                    Id = result.Id,
                    UserAccountId = result.UserAccountId,
                    JobDesignation = result.JobDesignation,
                    ClientId = result.Client.Id,
                    LineLeaderId = result.LineLeaderId == null ? null : result.LineLeaderId,
                    StartDate = result.StartDate,
                    EndDate = result.EndDate,
                    Location = result.Location,
                };

                return teamJobDesignationReturn;
            }
            catch (TeamJobDesignationException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Get team leader of a user
        /// </summary>
        [HttpGet]
        public TeamJobDesignationEditModel TeamJobDesignationGet(Guid? id)
        {
            try
            {
                var teamJobDesignation = TeamJobDesignationProvider.GetTeamJobDesignation(id.Value);

                if (teamJobDesignation == null)
                    teamJobDesignation = new TeamJobDesignation();

                var model = new TeamJobDesignationEditModel
                {
                    UserAccountId = teamJobDesignation.UserAccountId,
                    Id = teamJobDesignation.Id,
                    ClientId = teamJobDesignation.ClientId,
                    LineLeaderId = teamJobDesignation.LineLeaderId,
                    EmployerId = teamJobDesignation.EmployerId,
                    StartDate = teamJobDesignation.StartDate,
                    EndDate = teamJobDesignation.EndDate,
                    Location = teamJobDesignation.Location,
                    JobDesignation = teamJobDesignation.JobDesignation
                };
                return model;
            }
            catch (TeamJobDesignationException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete team leader from user
        /// </summary>
        [HttpPost]
        public void TeamJobDesignationDelete(TeamJobDesignationEditModel model)
        {
            try
            {
                TeamJobDesignationProvider.DeleteTeamJobDesignation(model.Id ?? Guid.Empty);
            }
            catch (TeamJobDesignationException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve full team designation list
        /// </summary>
        [HttpPost]
        public GridResultModel<TeamJobDesignationGridModel> TeamJobDesignationGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = TeamJobDesignationProvider.TeamJobDesignationtFilterList(model.Id ?? Guid.Empty)
                .Select(a => new TeamJobDesignationGridModel
                {
                    Id = a.Id,
                    UserAccountId = a.UserAccountId,
                    ClientName = a.Client.EntityName,
                    ClientId = a.ClientId,
                    LineLeader = a.LineLeader.FirstName + " " + a.LineLeader.Surname,
                    JobDesignation = a.JobDesignation,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Location = a.Location
                });

            switch (model.SortKey)
            {
                case "client":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ClientName)
                        : filteredQuery.OrderByDescending(r => r.ClientName);
                    break;

                case "lineleader":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.LineLeader)
                        : filteredQuery.OrderByDescending(r => r.LineLeader);
                    break;

                case "jobdesignation":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.JobDesignation)
                        : filteredQuery.OrderByDescending(r => r.JobDesignation);
                    break;

                case "location":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Location)
                        : filteredQuery.OrderByDescending(r => r.Location);
                    break;

                case "startdate":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.StartDate)
                        : filteredQuery.OrderByDescending(r => r.StartDate);
                    break;

                case "enddate":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EndDate)
                        : filteredQuery.OrderByDescending(r => r.EndDate);
                    break;
            }

            var totalNumberOfRecords = filteredQuery.Count();

            //filteredQuery = filteredQuery.OrderBy(a => a.Location);

            var returnList = filteredQuery.ToList();

            return new GridResultModel<TeamJobDesignationGridModel>(returnList, totalNumberOfRecords);
        }

        #endregion TeamJobDesignation

        #region personal information

        /// <summary>
        /// Retrieve personal information based on id
        /// </summary>
        [HttpGet]
        public PersonalInformationModel PersonalInformationGet(Guid id)
        {
            try
            {
                var personalInformation = PersonalInformationProvider.GetPersonalInformation(id);

                if (personalInformation == null)
                    personalInformation = new PersonalInformation();

                var user = SecurityProvider.GetUserList().Where(a => a.Id == id).Single();

                var model = new PersonalInformationModel
                {
                    Id = personalInformation.Id,
                    UserAccountId = user.Id,
                    FirstName = user.FirstName,
                    Surname = personalInformation.Surname,
                    FullNames = personalInformation.FullNames,
                    Title = personalInformation.Title,
                    IdNumber = personalInformation.IdNumber,
                    Dob = personalInformation.Dob,
                    SpouseName = personalInformation.SpouseName,
                    Children = personalInformation.Children,
                    Company = personalInformation.Company,
                    WorkExperienceStartDate = personalInformation.WorkExperienceStartDate,
                    EmploymentStartDate = personalInformation.EmploymentStartDate,
                    EmploymentEndDate = personalInformation.EmploymentEndDate,
                    Race = personalInformation.Race,
                    Gender = personalInformation.Gender,
                    DoorTagNumber = personalInformation.DoorTagNumber,
                    PhoneExtension = personalInformation.PhoneExtension,
                    CellPhone = personalInformation.CellPhone,
                    LandLinePhone = personalInformation.LandLinePhone,
                    CompanyEmail = personalInformation.CompanyEmail,
                    OtherEmail = personalInformation.OtherEmail,
                    AccessLevel = personalInformation.AccessLevel,
                    MedicalScheme = personalInformation.MedicalScheme,
                    MedicalAidNumber = personalInformation.MedicalAidNumber,
                    MedicalSchemeOption = personalInformation.MedicalSchemeOption
                };

                if (model.Dob.ToString() == "1/1/0001 12:00:00 AM")
                {
                    DateTime date = new DateTime(1940, 01, 01);
                    model.Dob = date;
                }

                if (model.EmploymentStartDate.ToString() == "1/1/0001 12:00:00 AM")
                {
                    DateTime date = new DateTime(2001, 01, 01);
                    model.EmploymentStartDate = date;
                }

                if (model.EmploymentEndDate.ToString() == "1/1/0001 12:00:00 AM")
                {
                    model.EmploymentEndDate = DateTime.Now;
                }

                if (model.WorkExperienceStartDate.ToString() == "1/1/0001 12:00:00 AM")
                {
                    DateTime date = new DateTime(1960, 01, 01);
                    model.WorkExperienceStartDate = date;
                }

                return model;
            }
            catch (PersonalInformationException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Save personal information on user
        /// </summary>
        [HttpPost]
        public PersonalInformationModel PersonalInformationSave(PersonalInformation model)
        {
            try
            {
                CheckModelState();

                if (model.UserAccountId == null || model.UserAccountId == Guid.Empty)
                {
                    var personalInformation = PersonalInformationProvider.GetPersonalInformation(model.UserAccountId);
                    if (personalInformation != null)
                        model.UserAccountId = personalInformation.Id;
                }

                var record = PersonalInformationProvider.SavePersonalInformation(model.Id,
                        model.UserAccountId,
                        model.FullNames,
                        model.Surname,
                        model.Title,
                        model.IdNumber,
                        model.SpouseName,
                        model.Children,
                        model.Dob.ToLocalTime(),
                        model.Company,
                        model.WorkExperienceStartDate.ToLocalTime(),
                        model.EmploymentStartDate.ToLocalTime(),
                        (model.EmploymentEndDate == null ? model.EmploymentEndDate : model.EmploymentEndDate.Value.ToLocalTime()),
                        model.Race,
                        model.Gender,
                        model.DoorTagNumber,
                        model.PhoneExtension,
                        model.CellPhone,
                        model.LandLinePhone,
                        model.CompanyEmail,
                        model.OtherEmail,
                        model.AccessLevel,
                        model.MedicalScheme,
                        model.MedicalSchemeOption,
                        model.MedicalAidNumber);

                var personalInformatoinReturn = new PersonalInformationModel
                {
                    Id = record.Id,
                    UserAccountId = record.UserAccountId,
                    Surname = record.Surname,
                    FullNames = record.FullNames,
                    Title = record.Title,
                    IdNumber = record.IdNumber,
                    Dob = record.Dob,
                    SpouseName = record.SpouseName,
                    Children = record.Children,
                    Company = record.Company,
                    WorkExperienceStartDate = record.WorkExperienceStartDate,
                    EmploymentStartDate = record.EmploymentStartDate,
                    EmploymentEndDate = record.EmploymentEndDate,
                    Race = record.Race,
                    Gender = record.Gender,
                    DoorTagNumber = record.DoorTagNumber,
                    PhoneExtension = record.PhoneExtension,
                    CellPhone = record.CellPhone,
                    LandLinePhone = record.LandLinePhone,
                    CompanyEmail = record.CompanyEmail,
                    OtherEmail = record.OtherEmail,
                    AccessLevel = record.AccessLevel,
                    MedicalScheme = record.MedicalScheme,
                    MedicalSchemeOption = record.MedicalSchemeOption,
                    MedicalAidNumber = record.MedicalAidNumber
                };

                return personalInformatoinReturn;
            }
            catch (PersonalInformationException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion personal information

        #region Dropdown List

        /// <summary>
        /// General purpose active user list used in dropdowns
        /// </summary>
        [HttpGet]
        public List<UserDropdownModel> UserDropdown()
        {
            return SecurityProvider.GetUserAccountList()
                .Where(a => a.Active && !a.FirstName.Equals("Importer"))
                .Select(a =>
                    new UserDropdownModel
                    {
                        Id = a.Id,
                        Firstname = a.FirstName,
                        Surname = a.Surname,
                        AccountName = a.AccountName
                    }).OrderBy(a => a.Firstname).ThenBy(a => a.Surname)
                .ToList();
        }

        /// <summary>
        /// General purpose full user list used in dropdowns
        /// </summary>
        [HttpGet]
        public List<UserDropdownModel> AllUserDropdown()
        {
            return SecurityProvider.GetUserAccountList()
                .Where(a => !a.FirstName.Equals("Importer"))
                .Select(a =>
                    new UserDropdownModel
                    {
                        Id = a.Id,
                        Firstname = a.FirstName,
                        Surname = a.Surname,
                        AccountName = a.Active ? "Yes" : "No"
                    }).OrderBy(a => a.Firstname).ThenBy(a => a.Surname)
                .ToList();
        }

        /// <summary>
        /// Retrieve list of users, as used in Timesheet view's user dropdown
        /// </summary>
        [HttpGet]
        public List<UserDropdownModel> UserTimesheetFilterDropdown()
        {
            if (CurrentUser.AllowedPrivileges.Contains(BL.Entities.Types.PrivilegeType.TimesheetCaptureForOtherAccounts))
                return SecurityProvider.GetUserAccountList()
                .Where(a => a.Active)
                .Select(a =>
                    new UserDropdownModel
                    {
                        Id = a.Id,
                        Firstname = a.FirstName,
                        Surname = a.Surname,
                        AccountName = a.AccountName
                    })
                .OrderBy(a => a.Firstname)
                .ThenBy(a => a.Surname)
                .ToList();

            return SecurityProvider.GetUserAccountList()
                .Where(a => a.Id == CurrentUser.Id)
                .Select(a =>
                    new UserDropdownModel
                    {
                        Id = a.Id,
                        Firstname = a.FirstName,
                        Surname = a.Surname,
                        AccountName = a.AccountName
                    })
                .OrderBy(a => a.Firstname)
                .ThenBy(a => a.Surname)
                .ToList();
        }

        [HttpGet]
        public List<ClientDropdownModel> TeamJobDesignationUniqueClient(Guid? id)
        {
            var filteredQuery = TeamJobDesignationProvider.TeamJobDesignationtFilterList(id.Value)
                .GroupBy(c => c.ClientId)
                .Select(a => new ClientDropdownModel
                {
                    Id = a.FirstOrDefault().ClientId,
                    EntityName = a.FirstOrDefault().Client.EntityName,
                    IsActive = a.FirstOrDefault().Client.IsActive
                });

            return filteredQuery.ToList();
        }

        #endregion Dropdown List
    }

    public class FileResult : IHttpActionResult
    {
        private readonly string _filePath;
        private readonly Stream _fileStream;
        private readonly string _contentType;

        public FileResult(string filePath, string contentType = null)
        {
            if (filePath == null) throw new ArgumentNullException("filePath");

            _filePath = filePath;
            _fileStream = File.OpenRead(_filePath);
            _contentType = contentType;
        }

        public FileResult(Stream fileStream, string contentType = null)
        {
            if (fileStream == null) throw new ArgumentNullException("fileStream empty");

            _fileStream = fileStream;
            _contentType = contentType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(_fileStream)
            };

            var contentType = _contentType ?? MimeMapping.GetMimeMapping(Path.GetExtension(_filePath));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return Task.FromResult(response);
        }
    }
}