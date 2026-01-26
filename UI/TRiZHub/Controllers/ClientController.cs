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
using TRiZHub.Models.ClientModels;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.Models.SecurityData;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class ClientController : TCRControllerBase
    {
        #region Ctor

        public ClientController()
        {
            AppSettings = new AppSettings(Context);
            ClientProvider = new ClientProvider(Context, CurrentUser);
        }

        public ClientController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            ClientProvider = new ClientProvider(Context, CurrentUser);
        }

        private IAppSettings AppSettings { get; }
        private IClientProvider ClientProvider { get; }

        #endregion

        #region Client

        /// <summary>
        /// Retrieve list of Clients filtered and sorted based on input values
        /// </summary>
        [HttpPost]
        public GridResultModel<ClientGridModel> ClientsGrid(GridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = ClientProvider.ClientEntityList().Where(c => !c.IsDeleted).Select(a => new ClientGridModel
            {
                Id = a.Id,
                DateCreated = a.DateCreated,
                EntityName = a.EntityName,
                IsActive = a.IsActive
            });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.EntityName.Contains(model.Searchfor));
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.EntityName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
              
                case "entityname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.EntityName)
                        : filteredQuery.OrderByDescending(r => r.EntityName);
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

            return new GridResultModel<ClientGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve single Client based on id
        /// </summary>
        [HttpGet]
        public ClientGridModel ClientGet(Guid? id)
        {
            try
            {
                var record = ClientProvider.ClientEntityList().SingleOrDefault(a => a.Id == id.Value);

                var model = new ClientGridModel
                {
                    Id = record.Id,
                    DateCreated = record.DateCreated,
                    EntityName = record.EntityName,
                    IsActive = record.IsActive,
                };

                return model;
            }
            catch (ClientException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

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
        }
    }
}