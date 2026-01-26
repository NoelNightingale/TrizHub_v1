#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.ClientEntityData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.BL.Entities.EmployerData;

#endregion Usings

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class EmployerController : TCRControllerBase
    {
        #region Ctor

        public EmployerController()
        {
            AppSettings = new AppSettings(Context);
            EmployerProvider = new EmployerProvider(Context, CurrentUser);
        }

        public EmployerController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            EmployerProvider = new EmployerProvider(Context, CurrentUser);
        }

        private IAppSettings AppSettings { get; }
        private IEmployerProvider EmployerProvider { get; }

        #endregion Ctor

        /// <summary>
        /// Retrieve list of Clients filtered and sorted based on input values
        /// </summary>
        [HttpPost]
        public GridResultModel<Employer> EmployerGrid(GridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = EmployerProvider.EmployerList().Where(c => !c.IsDeleted);

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.Name.Contains(model.Searchfor));
            }

            if (!model.ShowInactive)
            {
                filteredQuery = filteredQuery.Where(r => r.IsActive == true);
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.Name); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "name":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Name)
                        : filteredQuery.OrderByDescending(r => r.Name);
                    break;

                case "datecreated":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.DateCreated)
                        : filteredQuery.OrderByDescending(r => r.DateCreated);
                    break;

                case "isactive":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.IsActive)
                        : filteredQuery.OrderByDescending(r => r.IsActive);
                    break;
            }

            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<Employer>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve full list of Employers sorted by Name
        /// </summary>
        [HttpGet]
        public List<Employer> EmployerDropdown()
        {
            return EmployerProvider.EmployerList().Where(c => !c.IsDeleted && c.IsActive).OrderBy(a => a.Name)
                .ToList();
        }

        /// <summary>
        /// Retrieve full list of Employers sorted by Name
        /// </summary>
        [HttpGet]
        public List<Employer> AllEmployerDropdown()
        {
            return EmployerProvider.EmployerList().Where(c => !c.IsDeleted).OrderBy(a => a.Name)
                .ToList();
        }

        /// <summary>
        /// Retrieve single Employer based on id
        /// </summary>
        [HttpGet]
        public Employer EmployerGet(Guid? id)
        {
            try
            {
                var record = EmployerProvider.GetEmployer(id.Value);

                return record;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpPost]
        public Employer EmployerSave(Employer model)
        {
            try
            {
                var record = EmployerProvider.SaveEmployer(model);

                return record;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpGet]
        public int Activate(Guid id)
        {
            try
            {
                return EmployerProvider.Activate(id);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpGet]
        public int Deactivate(Guid id)
        {
            try
            {
                return EmployerProvider.Deactivate(id);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete team leader from user
        /// </summary>
        [HttpDelete]
        public int Delete(Guid id)
        {
            try
            {
                return EmployerProvider.Delete(id);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        ///// <summary>
        ///// Retrieve single Client based on id
        ///// </summary>
        //[HttpGet]
        //public ClientGridModel ClientGet(Guid? id)
        //{
        //    try
        //    {
        //        var record = ClientProvider.ClientEntityList().SingleOrDefault(a => a.Id == id.Value);

        //        var model = new ClientGridModel
        //        {
        //            Id = record.Id,
        //            DateCreated = record.DateCreated,
        //            EntityName = record.EntityName,
        //            IsActive = record.IsActive,
        //        };

        //        return model;
        //    }
        //    catch (ClientException e)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
        //    }
        //}

        /*

        /// <summary>
        /// Delete Client by ID
        /// </summary>
        [HttpGet]
        public int DeleteClient(Guid id)
        {
            try
            {
                return ClientProvider.DeleteClient(id);
            }
            catch (ClientException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion

        #region Customers

        /// <summary>
        /// Create or update Client
        /// </summary>
        [HttpPost]
        public ClientModel ClientSave(ClientModel model)
        {
            try
            {
                CheckModelState();

                var result = ClientProvider.SaveClientEntity(model.Id, model.EntityName, model.IsActive);

                model = new ClientModel
                {
                    Id = result.Id,
                    EntityName = result.EntityName,
                    DateCreated = result.DateCreated,
                    IsActive = result.IsActive
                };

                return model;
            }
            catch (ClientException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion

        #region Dropdown List

        /// <summary>
        /// Retrieve full list of Clients sorted by Name
        /// </summary>
        [HttpGet]
        public List<ClientDropdownModel> ClientDropdown()
        {
            return ClientProvider.ClientEntityList().Where(c => !c.IsDeleted)//.Where(a => a.IsActive)
                .Select(a =>
                    new ClientDropdownModel {Id = a.Id, EntityName = a.EntityName, IsActive = a.IsActive}).OrderBy(a => a.EntityName)
                .ToList();
        }

        /// <summary>
        /// Retrieve full list of Clients sorted by Name
        /// </summary>
        [HttpGet]
        public List<ClientDropdownModel> ClientReporterDropdown()
        {
            return ClientProvider.ClientEntityListForClientReporter()
                .Where(c => !c.IsDeleted)
                .Select(a =>
                    new ClientDropdownModel { Id = a.Id, EntityName = a.EntityName, IsActive = a.IsActive }).OrderBy(a => a.EntityName)
                .ToList();
        }
        #endregion

        [HttpGet]
        public List<UserDropdownModel> GetClientReporters(Guid id)
        {
            var users = ClientProvider.GetClientReporters(id)
                .Select(a =>
                    new UserDropdownModel
                    {
                        Id = a.Id,
                        Firstname = a.FirstName,
                        Surname = a.Surname,
                        AccountName = a.AccountName
                    }).OrderBy(a => a.Firstname).ThenBy(a => a.Surname)
                .ToList();

            return users;
        }

        [HttpGet]
        public List<UserDropdownModel> AddClientReporter(Guid clientId, Guid userId)
        {
            ClientProvider.AddClientReporter(clientId, userId);

            return GetClientReporters(clientId);
        }

        [HttpGet]
        public List<UserDropdownModel> RemoveClientReporter(Guid clientId, Guid userId)
        {
            ClientProvider.RemoveClientReporter(clientId, userId);
            return GetClientReporters(clientId);
        }*/
    }
}