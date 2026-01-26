#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.ProjectData;
using TRiZHub.BL.Provider.ClientEntityData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Entities.UserIdentityProject;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.ProjectModels;
using System.Web.Configuration;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using TRiZHub.BL.Entities.UserIdentityClient;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class ProjectController : TCRControllerBase
    {
        #region Ctor

        public ProjectController()
        {
            AppSettings = new AppSettings(Context);
            ProjectProvider = new ProjectProvider(Context, CurrentUser);
            ClientProvider = new ClientProvider(Context, CurrentUser);
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
        }

        public ProjectController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            ProjectProvider = new ProjectProvider(Context, CurrentUser);
            ClientProvider = new ClientProvider(Context, CurrentUser);
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
        }

        private IAppSettings AppSettings { get; }
        private IProjectProvider ProjectProvider { get; }
        private IClientProvider ClientProvider { get; }

        private ISecurityProvider SecurityProvider { get; }

        #endregion

        #region Project

        /// <summary>
        /// Retrieve list of Projects based on filter and sort input values
        /// </summary>
        [HttpPost]
        public GridResultModel<ProjectGridModel> ProjectsGrid(GridModel model)
        {
            var begin = SetupGridParams(model);

            var filteredQuery = ProjectProvider.ProjectList().Where(p => !p.IsDeleted).Select(a => new ProjectGridModel
            {
                Id = a.Id,
                DateCreated = a.DateCreated,
                ProjectName = a.ProjectName,
                ProjectDescription = a.ProjectDescription,
                ProjectNumber = a.ProjectNumber,
                ClientName = a.Client.EntityName,
                Billable = a.Billable,
                ProjectLeadName = a.ProjectLead != null ? a.ProjectLead.FirstName + " " + a.ProjectLead.Surname : "",
                IsActive = a.IsActive
            });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.ProjectName.Contains(model.Searchfor) ||
                       r.ClientName.Contains(model.Searchfor) ||
                       r.ProjectNumber.Contains(model.Searchfor)

                        );



            }

            if (!model.ShowInactive)
            {
                filteredQuery = filteredQuery.Where(r => r.IsActive == true);
            }


            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.ProjectName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            switch (model.SortKey)
            {
                case "projectname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ProjectName)
                        : filteredQuery.OrderByDescending(r => r.ProjectName);
                    break;
                case "projectnumber":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ProjectNumber)
                        : filteredQuery.OrderByDescending(r => r.ProjectNumber);
                    break;
                case "projectdescription":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ProjectDescription)
                        : filteredQuery.OrderByDescending(r => r.ProjectDescription);
                    break;

                case "clientname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ClientName)
                        : filteredQuery.OrderByDescending(r => r.ClientName);
                    break;
                case "billable":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.Billable)
                        : filteredQuery.OrderByDescending(r => r.Billable);
                    break;
                case "projectleadname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ProjectLeadName)
                        : filteredQuery.OrderByDescending(r => r.ProjectLeadName);
                    break;
                case "isactive":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.IsActive)
                        : filteredQuery.OrderByDescending(r => r.IsActive);
                    break;

            }


            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            return new GridResultModel<ProjectGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve single Project 
        /// </summary>
        [HttpGet]
        public ProjectModel ProjectGet(Guid? id)
        {
            try
            {
                var record = ProjectProvider.GetProject(id.Value);

                var model = new ProjectModel
                {
                    Id = record.Id,
                    DateCreated = record.DateCreated,
                    IsActive = record.IsActive,
                    ClientId = record.ClientId,
                    ClientName = record.Client.EntityName,
                    ProjectLeadId = record.ProjectLeadId,
                    ProjectLeadName = record.ProjectLead != null ? record.ProjectLead.AccountName : "",
                    ProjectNumber = record.ProjectNumber,
                    ProjectDescription = record.ProjectDescription,
                    Billable = record.Billable,
                    ProjectName = record.ProjectName,
                    ProjectTypeId = record.ProjectTypeId,
                    AllowSubProjectAlternativeType = record.ProjectType.AllowSubProjectAlternativeType,
                    HasSubprojects = record.SubProjects.Count() > 0 ? true : false,
                    ExcludeTimeCapture = (bool)(record.ExcludeTimeCapture == null?false: record.ExcludeTimeCapture)

                };

                return model;
            }
            catch (ProjectException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Create or update Project
        /// </summary>
        [HttpPost]
        public ProjectModel ProjectSave(ProjectModel model)
        {
            try
            {
                CheckModelState();

                var record = ProjectProvider.SaveProject(model.Id, model.ClientId,
                    model.ProjectLeadId, model.ProjectTypeId, model.ProjectName, model.ProjectNumber, model.ProjectDescription, model.Billable, model.IsActive, model.ExcludeTimeCapture);

                var result = ProjectProvider.GetProject(record.Id);

                model = new ProjectModel
                {
                    Id = result.Id,
                    DateCreated = result.DateCreated,
                    IsActive = result.IsActive,
                    ClientId = result.ClientId,
                    ClientName = result.Client.EntityName,
                    ProjectLeadId = result.ProjectLeadId,
                    ProjectLeadName = result.ProjectLead != null ? result.ProjectLead.AccountName : "",
                    Billable = result.Billable,
                    ExcludeTimeCapture = (bool)(result.ExcludeTimeCapture == null ? false: result.ExcludeTimeCapture),
                    ProjectName = result.ProjectName,
                    ProjectTypeId = result.ProjectTypeId
                };

                return model;
            }
            catch (ProjectException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Project by ID
        /// </summary>
        [HttpGet]
        public int DeleteProject(Guid id)
        {
            try
            {
                return ProjectProvider.DeleteProject(id);
            }
            catch (ProjectException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion

        #region Sub Projects

        /// <summary>
        /// Retrieve list of SubProjects based on parent project and sorted by input values
        /// </summary>
        [HttpPost]
        public GridResultModel<SubProjectGridModel> SubProjectsGrid(IdGridModel model)
        {
            var begin = SetupGridParams(model);

            var parentProject = ProjectProvider.GetProject(model.ParentId.Value);

            var filteredQuery =
                ProjectProvider.SubProjectList()
                    .Where(a => a.ProjectId == model.ParentId && !a.IsDeleted)
                    .Select(a => new SubProjectGridModel
                    {
                        Id = a.Id,
                        DateCreated = a.DateCreated,
                        ProjectName = a.ProjectName,
                        IsActive = a.IsActive,
                        ParentProjectName = a.Project.ProjectName,
                        SubProjectNumber = parentProject.ProjectNumber + "-" + a.SubProjectNumber
                    });

            if (model.Searchfor != "null")
            {
                filteredQuery =
                    filteredQuery.Where(
                        r => r.ProjectName.Contains(model.Searchfor));
            }

            //Get Record count
            var totalNumberOfRecords = filteredQuery.Count();

            if (string.IsNullOrWhiteSpace(model.SortKey))
                filteredQuery = filteredQuery.OrderBy(a => a.ProjectName); //default sort order

            if (!string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = model.SortKey.ToLower();

            //            filteredQuery = filteredQuery.Skip(begin).Take(model.RecordsPerPage.Value);

            switch (model.SortKey)
            {
                case "subprojectnumber":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.SubProjectNumber)
                        : filteredQuery.OrderByDescending(r => r.SubProjectNumber);
                    break;
                case "projectname":
                    filteredQuery = model.SortOrder == "ASC"
                        ? filteredQuery.OrderBy(r => r.ProjectName)
                        : filteredQuery.OrderByDescending(r => r.ProjectName);
                    break;
            }

            return new GridResultModel<SubProjectGridModel>(filteredQuery.ToList(), totalNumberOfRecords);
        }

        /// <summary>
        /// Retrieve single subproject based on id
        /// </summary>
        [HttpGet]
        public SubProjectModel SubProjectGet(Guid? id)
        {
            try
            {
                var record = ProjectProvider.GetSubProject(id.Value);

                var model = new SubProjectModel
                {
                    Id = record.Id,
                    DateCreated = record.DateCreated,
                    IsActive = record.IsActive,
                    ProjectId = record.ProjectId,
                    ProjectName = record.ProjectName,
                    ParentProjectName = record.Project.ProjectName,
                    ParentProjectTypeId = record.Project.ProjectTypeId,
                    ParentAllowSubProjectAlternativeType = record.Project.ProjectType.AllowSubProjectAlternativeType,
                    subProjectNumber = record.SubProjectNumber,
                    SubProjectTypeId = record.SubProjectTypeId
                };

                return model;
            }
            catch (ProjectException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Create or update Subproject
        /// </summary>
        [HttpPost]
        public SubProjectModel SubProjectSave(SubProjectModel model)
        {
            try
            {
                CheckModelState();

                var record = ProjectProvider.SaveSubProject(model.Id, model.ProjectId, model.SubProjectTypeId,
                    model.ProjectName, model.subProjectNumber, model.IsActive);

                var result = ProjectProvider.GetSubProject(record.Id);

                model = new SubProjectModel
                {
                    Id = result.Id,
                    DateCreated = result.DateCreated,
                    IsActive = result.IsActive,
                    ProjectId = result.ProjectId,
                    ProjectName = result.ProjectName,
                    ParentProjectName = result.Project.ProjectName,
                    subProjectNumber = result.SubProjectNumber,
                    SubProjectTypeId = record.SubProjectTypeId
                };
                return model;
            }
            catch (ProjectException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        /// <summary>
        /// Delete Sub Project by ID
        /// </summary>
        [HttpGet]
        public int DeleteSubProject(Guid id)
        {
            try
            {
                return ProjectProvider.DeleteSubProject(id);
            }
            catch (ProjectException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        #endregion

        [HttpGet]
        public List<UserIdentityProjectModel> UserIdentityProjectList(string id, bool includeInactive)
        {
            try
            {
                return GetClientTree(Guid.Parse(id), includeInactive);
            }
            catch (ProjectException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }

        [HttpPost]
        public void SaveUserIdentityProjectList([FromBody] UserIdentityProjectSaveModel model)
        {
            try
            {
                var clients = model.Projects.Where(p => p.ProjectId == Guid.Empty).Select(p => new UserIdentityClient()
                {
                    ClientId = p.ClientId,
                    UserAccountId = model.UserId
                }).ToList();

                //Remove projects where entire clients are selected
                var idsList = clients.Select(obj => obj.ClientId).ToList();
                var filteredModel = model.Projects.Where(x => !idsList.Contains(x.ClientId)).ToList();

                var projects = filteredModel.Where(p => p.ProjectId != Guid.Empty).Select(p => new UserIdentityProject()
                {
                    ProjectId = p.ProjectId,
                    SubProjectId = p.SubProjectId,
                    UserAccountId = model.UserId
                }).ToList();

                //Remove Sub-projects where entire projects are selected
                var entireProjectIDs= projects.Where(p => p.SubProjectId == Guid.Empty || p.SubProjectId == null).Select(obj => obj.ProjectId).ToList();
                List<UserIdentityProject> subProjects = projects.Where(x => !entireProjectIDs.Contains(x.ProjectId) && x.SubProjectId != Guid.Empty).ToList();
                projects = projects.Where(x => x.SubProjectId == Guid.Empty || x.SubProjectId == null).ToList();
                projects.AddRange(subProjects);
                foreach (var item in projects)
                {
                    if (item.SubProjectId == Guid.Empty)
                    {
                        item.SubProjectId = null;
                    }
                }

                ClientProvider.SaveUserAlllocation(model.UserId, clients);
                ProjectProvider.SaveUserIdentityProject(model.UserId, projects);
            }
            catch (ProjectException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
        }


        private List<UserIdentityProjectModel> GetClientTree(Guid userId, bool includeInactive)
        {

            var clients = ClientProvider.ClientEntityList().ToList();
            var projects = ProjectProvider.ProjectList();
            var subProjects = ProjectProvider.SubProjectList().ToList();
            var userProjects = ProjectProvider.GetUserAllocatedProjects(userId).ToList();
            var userClients = ClientProvider.GetUserAllocatedClients(userId).ToList();

            var results = new List<UserIdentityProjectModel>();

            if (!includeInactive) {
                clients = clients.Where(c => c.IsActive).ToList();
                projects = projects.Where(c => c.IsActive);
                subProjects = subProjects.Where(c => c.IsActive).ToList();
            }

            // Build full tree            
            foreach (var client in clients)
            {
                var clientResult = new UserIdentityProjectModel
                {
                    ClientId = client.Id,
                    Name = client.EntityName,
                    Selected = userClients.FindIndex(uc => uc.ClientId == client.Id) > -1 ? true : false,

                    ListOfProjects = projects.Where(p => p.ClientId == client.Id).ToList().Select(p => new UserIdentityProjectModel
                    {
                        ClientId = client.Id,
                        ProjectId = p.Id,                        
                        Name = p.ProjectName,
                        Code = p.ProjectNumber,
                        isActive = p.IsActive,
                        ListOfProjects = subProjects.Where(sp => sp.ProjectId == p.Id).Select(sp => new UserIdentityProjectModel
                        {
                            ClientId = client.Id,
                            ProjectId = p.Id,
                            SubProjectId = sp.Id,
                            Name = sp.ProjectName,
                            Code = sp.SubProjectNumber,
                            isActive = sp.IsActive
                        }).OrderBy(c => c.Name).ToList()
                    }).OrderBy(c => c.Name).ToList()
                };

                results.Add(clientResult);
            }


            foreach (var client in results)
            {
                foreach (var project in client.ListOfProjects)
                {
                    project.Selected = userProjects.FindIndex(up => up.ProjectId == project.ProjectId && up.SubProjectId == null) > -1 ? true : false;

                    foreach (var subProject in project.ListOfProjects)
                    {
                        subProject.Selected = userProjects.FindIndex(up => up.SubProjectId == subProject.SubProjectId) > -1 ? true : false;
                    }
                }
            }

            results = results.OrderBy(r => r.Name).ToList();

            return results;
        }





        #region Dropdown List
        /// <summary>
        /// Retrieve combined list of Project and Subprojects sorted by Description
        /// </summary>
        /// 
        [HttpGet]
        public List<UserProjectGridModel> ProjectAndSubProjectDropdown()
        {
            var returnList = new List<UserProjectGridModel>();
            returnList.AddRange(ProjectProvider.ProjectList().Where(a => a.IsActive == true && !a.IsDeleted)
                .Select(a => new UserProjectGridModel
                {
                    ProjectId = a.Id,
                    ProjectName = (a.ProjectNumber == null || a.ProjectNumber.Equals("")) ? a.ProjectName : ("[" + a.ProjectNumber + "] " + a.ProjectName),
                    IsBillable = a.Billable,
                    ClientName = a.Client.EntityName,
                    ClientId = a.ClientId
                }));
            returnList.AddRange(ProjectProvider.SubProjectList().Where(p => p.IsActive == true && !p.IsDeleted).Where(p => p.Project.IsActive == true && !p.IsDeleted)
                .Select(a => new UserProjectGridModel
                {
                    ProjectId = a.ProjectId,
                    SubProjectId = a.Id,
                    ProjectName = a.SubProjectNumber == null ? (a.Project.ProjectNumber == null || a.Project.ProjectNumber.Equals("")) ? a.Project.ProjectName : ("[" + a.Project.ProjectNumber + "] " + a.Project.ProjectName) : (a.Project.ProjectNumber == null || a.Project.ProjectNumber.Equals("")) ? a.Project.ProjectName : ("[" + a.Project.ProjectNumber + "-" + a.SubProjectNumber + "] " + a.Project.ProjectName),
                    SubProjectName = a.ProjectName,
                    IsBillable = a.Project.Billable,
                    ClientName = a.Project.Client.EntityName,
                    ClientId = a.Project.ClientId
                }));
            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }

//        GetUserAllocatedProjects/" + id + "?includeInactive=" + includeInactive
//        [HttpGet("/api/v1/GetUserAllocatedProjects/{id}?includeInactive")]
        public List<UserProjectGridModel> GetUserAllocatedProjects1(string id, bool includeInactive = false)
        {

            var returnList = new List<UserProjectGridModel>();

            var userProjectAllocation = ProjectProvider.GetUserAllocatedProjects(new Guid(id));

            if ((bool)!includeInactive)
            {
                userProjectAllocation = userProjectAllocation.Where(p => p.Project.IsActive == true).Where(p => (p.SubProject == null) || ((p.SubProject.IsActive == true) && (p.SubProject.IsDeleted == false) ));
            }

           

            returnList.AddRange(userProjectAllocation.Select(a => new UserProjectGridModel
            {
                Id = a.Id,
                ProjectId = a.ProjectId,
                ProjectName = a.Project.ProjectName,
                ProjectNumber = a.Project.ProjectNumber,
                SubProjectId = a.SubProjectId,
                SubProjectName = a.SubProject.ProjectName,
                SubProjectNumber = a.SubProject.SubProjectNumber,
                ClientId = a.Project.ClientId,
                ClientName = a.Project.Client.EntityName,
                IsActive = a.Project.IsActive && ((a.SubProject == null) || (a.SubProject.IsActive))

            })); 

            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }



        public List<UserProjectGridModel> GetUserAllocatedProjects(string id, bool includeInactive = false)
        {

            var clients = ClientProvider.ClientEntityList().ToList();
            var projects = ProjectProvider.ProjectList();
            var subProjects = ProjectProvider.SubProjectList().ToList();
            var userProjects = ProjectProvider.GetUserAllocatedProjects(new Guid(id)).ToList();
            var userClients = ClientProvider.GetUserAllocatedClients(new Guid(id)).ToList();

            var results = new List<UserProjectGridModel>();

            if (!includeInactive)
            {
                clients = clients.Where(c => c.IsActive).ToList();
                projects = projects.Where(c => c.IsActive);
                subProjects = subProjects.Where(c => c.IsActive).ToList();
            }

            // Build full tree            
            foreach (var client in clients)
            {
                var clientSelected = userClients.FindIndex(uc => uc.ClientId == client.Id) > -1 ? true : false;

                foreach (var project in projects.Where(p => p.ClientId == client.Id).ToList())
                {
                   var projectSelected = userProjects.FindIndex(up => up.ProjectId == project.Id && up.SubProjectId == null) > -1 ? true : false;
                   if ((projectSelected || clientSelected) && (project.ExcludeTimeCapture == false || project.ExcludeTimeCapture == null))
                   {
                        results.Add(new UserProjectGridModel
                        {
                            Id = new Guid(),
                            ProjectId = project.Id,
                            ProjectName = project.ProjectName,
                            ProjectNumber = project.ProjectNumber,
                            ClientId = project.ClientId,
                            ClientName = project.Client.EntityName,
                            IsActive = project.IsActive,
                            IsBillable = project.Billable
                        });
                   }

                    foreach (var subProject in subProjects.Where(sp => sp.ProjectId == project.Id).ToList())
                    {
                        var subProjectSelected = userProjects.FindIndex(up => up.ProjectId == project.Id && up.SubProjectId == subProject.Id) > -1 ? true : false;
                        if (projectSelected || clientSelected || subProjectSelected)
                        {
                            results.Add(new UserProjectGridModel
                            {
                                Id = new Guid(),
                                ProjectId = project.Id,
                                ProjectName = project.ProjectName,
                                ProjectNumber = project.ProjectNumber,
                                SubProjectId = subProject.Id,
                                SubProjectName = subProject.ProjectName,
                                SubProjectNumber = subProject.SubProjectNumber,
                                ClientId = project.ClientId,
                                ClientName = project.Client.EntityName,
                                IsActive = subProject.IsActive,
                                IsBillable = project.Billable

                            });
                        }

                    }
                }

            }

            results = results.OrderBy(r => r.ClientName).ThenBy(r => r.Description).ToList();

            return results;
        }





        /// <summary>
        /// Retrieve list of active Projects sorted by Description
        /// </summary>
        [HttpGet]
        public List<ProjectDropdownModel> ProjectDropdown()
        {
            var returnList = new List<ProjectDropdownModel>();
            returnList.AddRange(ProjectProvider.ProjectList().Where(a => a.IsActive == true && !a.IsDeleted)
                .Select(a => new ProjectDropdownModel { ProjectId = a.Id, ProjectName = (a.ProjectNumber == null || a.ProjectNumber.Equals("")) ? a.ProjectName : ("[" + a.ProjectNumber + "] " + a.ProjectName), IsActive = a.IsActive }));
            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }

        /// <summary>
        /// Retrieve list of active Projects belolling to a client reporter
        /// </summary>
        [HttpGet]
        public List<ProjectDropdownModel> ProjectDropdownForClientReporter()
        {
            var returnList = new List<ProjectDropdownModel>();
            returnList.AddRange(ProjectProvider.ProjectListForClientReporter()
                .Where(a => a.IsActive == true && !a.IsDeleted)
                .Select(a => new ProjectDropdownModel { ProjectId = a.Id, ProjectName = (a.ProjectNumber == null || a.ProjectNumber.Equals("")) ? a.ProjectName : ("[" + a.ProjectNumber + "] " + a.ProjectName), IsActive = a.IsActive }));
            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }

        /// <summary>
        /// Retrieve list of active Projects belolling to a client reporter
        /// </summary>
        [HttpGet]
        public List<ProjectDropdownModel> AllProjectDropdownForClientReporter()
        {
            var returnList = new List<ProjectDropdownModel>();
            returnList.AddRange(ProjectProvider.ProjectListForClientReporter().Where(a => !a.IsDeleted)
                .Select(a => new ProjectDropdownModel { ProjectId = a.Id, ProjectName = (a.ProjectNumber == null || a.ProjectNumber.Equals("")) ? a.ProjectName : ("[" + a.ProjectNumber + "] " + a.ProjectName) }));
            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }

        /// <summary>
        /// Retrieve list of all Projects sorted by Description
        /// </summary>
        [HttpGet]
        public List<ProjectDropdownModel> AllProjectDropdown()
        {
            var returnList = new List<ProjectDropdownModel>();
            returnList.AddRange(ProjectProvider.ProjectList().Where(a => !a.IsDeleted)
                .Select(a => new ProjectDropdownModel { ProjectId = a.Id, ProjectName = (a.ProjectNumber == null || a.ProjectNumber.Equals("")) ? a.ProjectName : ("[" + a.ProjectNumber + "] " + a.ProjectName), IsActive = a.IsActive }));
            return returnList.ToList().OrderBy(a => a.Description).ToList();
        }

        /// <summary>
        /// Retrieve list of all Project Types ordered by sort order
        /// </summary>
        [HttpGet]
        public List<ProjectTypeDropdownModel> ProjectTypeDropdown()
        {
            var returnList = new List<ProjectTypeDropdownModel>();
            returnList.AddRange(ProjectProvider.ProjectTypeDropdown()
                .Select(a => new ProjectTypeDropdownModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    AllowSubProjectAlternativeType = a.AllowSubProjectAlternativeType,
                    AllowSubProjectBillable = a.AllowSubProjectBillable,
                    SortOrder = a.SortOrder
                }));
            return returnList.OrderBy(a => a.SortOrder).ToList();
        }

        #endregion
    }
}