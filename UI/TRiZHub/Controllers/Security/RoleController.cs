#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.SecurityData;

#endregion

namespace TRiZHub.Controllers.Security
{
    [Authorize]
    [NoCache]
    public class RoleController : TCRControllerBase
    {
        #region Ctor

        private ISecurityProvider SecurityProvider { get; }

        public RoleController()
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
        }

        public RoleController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
        }

        #endregion

        #region Role

        /// <summary>
        /// Create new Role
        /// </summary>
        [HttpPost]
        public RoleViewModel RoleSave(RoleViewModel model)
        {
            try
            {
                CheckModelState();
                var role1 = SecurityProvider.SaveRole(model.Id, model.RoleName, model.Description, model.StatusType,
                    model.Permissions.Where(a => a.Selected).Select(a => a.Privilege).ToList(), model.IsActive);

                var role = new RoleViewModel
                {
                    Id = role1.Id,
                    RoleName = role1.RoleName,
                    Description = role1.Description,
                    Permissions = role1.Privileges.Select(a => new PermissionViewModel
                    {
                        Privilege = a.Security
                    }).ToList(),
                    StatusType = role1.Status,
                    Status = role1.Status.ToString(),
                    IsActive = role1.isActive,
                };

                return role;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Retrieve list of Roles unsorted
        /// </summary>
        [HttpPost]
        public List<RoleViewModel> RoleList()
        {
            var items = SecurityProvider.GetRoles().ToList();

            var model = items.Select(a => new RoleViewModel
            {
                Id = a.Id,
                RoleName = a.RoleName,
                Description = a.Description,
                IsActive = a.isActive,
            }).ToList();

            return model;
        }

        /// <summary>
        /// Retrieve list of Roles filtered and sorted based on input values
        /// </summary>
        [HttpPost]
        public GridResultModel<RoleGridModel> RoleGrid(GridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = SecurityProvider.GetRoles()
                .Select(a => new RoleGridModel
                {
                    Id = a.Id,
                    RoleName = a.RoleName,
                    Description = a.Description,
                    StatusTypes = a.Status,
                    IsActive = a.isActive,
                });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.RoleName.Contains(model.Searchfor) || r.Description.Contains(model.Searchfor));
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.RoleName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortOrder)
            {
                case "ASC":
                    switch (model.SortKey)
                    {
                        case "rolename":
                            filteredQuery = filteredQuery.OrderBy(r => r.RoleName);
                            break;
                        case "description":
                            filteredQuery = filteredQuery.OrderBy(r => r.Description);
                            break;
                        case "status":
                            filteredQuery = filteredQuery.OrderBy(r => r.IsActive);
                            break;
                    }
                    break;
                case "DESC":
                    switch (model.SortKey)
                    {
                        case "rolename":
                            filteredQuery = filteredQuery.OrderByDescending(r => r.RoleName);
                            break;
                        case "description":
                            filteredQuery = filteredQuery.OrderByDescending(r => r.Description);
                            break;
                        case "status":
                            filteredQuery = filteredQuery.OrderByDescending(r => r.IsActive);
                            break;
                    }
                    break;
            }


            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<RoleGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Get Role based on id
        /// </summary>
        [HttpGet]
        public RoleViewModel RoleGet(Guid id)
        {
            try
            {
                var model = SecurityProvider.GetRoles().Where(a => a.Id == id).Select(a => new RoleViewModel
                {
                    Id = a.Id,
                    RoleName = a.RoleName,
                    Description = a.Description,
                    StatusType = a.Status,
                    IsActive = a.isActive,
                }).Single();

                var allPermissions = SecurityProvider.GetPrivilegeList().Select(p => new PermissionViewModel
                {                    
                    Privilege = p.Security,
                    Selected = false
                }).ToList();


                var rolePermissions = SecurityProvider.GetRolePrivilegeList(id).Select(p => new PermissionViewModel
                {
                    Privilege = p.Security,
                    Selected = false
                }).ToList();

                foreach (var ap in allPermissions)
                {
                    foreach (var rp in rolePermissions)
                    {
                        if (ap.Privilege == rp.Privilege)
                        {
                            ap.Selected = true;
                        }
                    }
                }

                model.Permissions = allPermissions;


                return model;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Get list of Priviledges
        /// </summary>
        [HttpGet]
        public List<PermissionViewModel> RolePrivileges()
        {
            try
            {
                return SecurityProvider.GetPrivilegeList().ToList().Select(p => new PermissionViewModel
                {
                    Privilege = p.Security,
                    Selected = false
                }).ToList();
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion
    }
}