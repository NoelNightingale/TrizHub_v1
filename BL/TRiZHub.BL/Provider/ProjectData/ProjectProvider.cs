#region Usings

using System;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.UserIdentityProject;
using TRiZHub.BL.Provider.Security;
using System.Collections.Generic;


#endregion

namespace TRiZHub.BL.Provider.ProjectData
{
    public class ProjectProvider : TRiZHubProvider, IProjectProvider
    {
        #region Constructor
        IList<PrivilegeType> getTokens;

        public ProjectProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            getTokens = new List<PrivilegeType>();
            getTokens.Add(PrivilegeType.ProjectMaintenance);
            getTokens.Add(PrivilegeType.CustomerReportAccess);
            getTokens.Add(PrivilegeType.ReportGenerationTimesheet);
            getTokens.Add(PrivilegeType.TimesheetCapture);
        }

        #endregion

        #region Project


        public IQueryable<Project> ProjectListForClientReporter()
        {
            AuthenticateList(getTokens);

            List<Guid> clientIds = DataContext.ClientReporterSet.Where(u => u.UserAccountId == CurrentUser.Id).Select(a => a.ClientId).ToList();

            return DataContext.ProjectSet.Where(a => clientIds.Contains(a.ClientId));
        }

        public IQueryable<Project> ProjectList()
        {
            AuthenticateList(getTokens);

            return DataContext.ProjectSet.Where(a => a.IsDeleted == false).OrderByDescending(a => a.ProjectName);
            // return DataContext.ProjectSet.Where(a=> a.IsActive == true);
        }

        public Project GetProject(Guid id)
        {
            AuthenticateList(getTokens);
            return DataContext.ProjectSet.Include(a => a.Client).FirstOrDefault(a => a.Id == id);
        }

        public Project SaveProject(Guid? id, Guid clientId, Guid? projectLeadId, Guid? projectTypeId,
            string projectName, string projectNumber, string projectDescription, bool billable, bool isActive, bool? excludeTimeCapture)
        {
            Authenticate(PrivilegeType.ProjectMaintenance);

            if (excludeTimeCapture == null) excludeTimeCapture = false; 

            var existing =
                DataContext.ProjectSet.FirstOrDefault(
                    a => a.ProjectName == projectName && a.ClientId == clientId && a.Id != id && a.IsActive == true);
            if (existing != null)
                throw new ProjectException("A project with the name: " + projectName +
                                           " already exists for this client.");

            var existing1 =
                DataContext.ProjectSet.FirstOrDefault(
                    a => a.ProjectNumber == projectNumber && a.Id != id && a.IsActive);
            if (existing1 != null)
                throw new ProjectException("A project with the code: " + projectNumber +
                                           " already exists.");

            var record = DataContext.ProjectSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new Project
                {
                    DateCreated = DateTime.UtcNow,
                    IsActive = true
                };
                DataContext.ProjectSet.Add(record);
            }

            // Update sub-project types to parent type
            if (projectTypeId.Value != null && record.ProjectTypeId != null && record.ProjectTypeId.Value != projectTypeId.Value && record.SubProjects.Count() > 0)
            {
                foreach (var item in record.SubProjects)
                {
                    item.SubProjectTypeId = projectTypeId;
                }
            }

            record.ClientId = clientId;
            record.ProjectLeadId = projectLeadId;
            record.ProjectTypeId = projectTypeId;
            record.ProjectName = projectName;
            record.Billable = billable;
            record.IsActive = isActive;
            record.ProjectNumber = projectNumber;
            record.ProjectDescription = projectDescription;
            record.ExcludeTimeCapture = excludeTimeCapture==null?false:excludeTimeCapture;

            DataContextSaveChanges();

            return record;
        }

        public int DeleteProject(Guid id)
        {
            Authenticate(PrivilegeType.ProjectMaintenance);

            // Check if the project has time logged against it
            var projectTimeSheetCount = DataContext.TimesheetEntrySet.Count(t => t.ProjectId == id);

            // Check if the Project's subprojects have time logged against it
            var subProjectTimeSheetCount = DataContext.TimesheetEntrySet.Count(t => t.SubProject.ProjectId == id);

            if (projectTimeSheetCount > 0 || subProjectTimeSheetCount > 0)
            {
                return 0;
            }

            // Delete Project and Sub Projects
            var project = DataContext.ProjectSet.FirstOrDefault(p => p.Id == id);
            project.IsDeleted = true;

            var subProjects = DataContext.SubProjectSet.Where(p => p.ProjectId == id);
            foreach (var subProject in subProjects)
            {
                subProject.IsDeleted = true;
            }

            DataContextSaveChanges();

            return 1;
        }

        #endregion

        #region Sub Project

        public IQueryable<SubProject> SubProjectList()
        {
            AuthenticateList(getTokens);

            return DataContext.SubProjectSet.Where(a => a.IsDeleted == false);
        }

        public SubProject GetSubProject(Guid id)
        {
            AuthenticateList(getTokens);
            return DataContext.SubProjectSet.Include(a => a.Project).FirstOrDefault(a => a.Id == id);
        }

        public SubProject SaveSubProject(Guid? id, Guid projectId, Guid? subProjectTypeId, string projectName, string subProjectNumber, bool isActive)
        {
            AuthenticateList(getTokens);
            Authenticate(PrivilegeType.ClientMaintenance);

            var existing =
                 DataContext.SubProjectSet.FirstOrDefault(
                     a => a.ProjectName == projectName && a.ProjectId == projectId && a.SubProjectNumber == subProjectNumber && a.Id != id);
            if (existing != null)
                throw new ProjectException("A sub project with the Sub Project Number: " + subProjectNumber +
                                           " already exists for this project.");

            var record = DataContext.SubProjectSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new SubProject
                {
                    DateCreated = DateTime.UtcNow,
                    IsActive = true
                };
                DataContext.SubProjectSet.Add(record);
            }

            record.ProjectId = projectId;
            record.SubProjectTypeId = subProjectTypeId;
            record.ProjectName = projectName;
            record.SubProjectNumber = subProjectNumber;
            record.IsActive = isActive;

            DataContextSaveChanges();

            return record;
        }

        public int DeleteSubProject(Guid id)
        {
            Authenticate(PrivilegeType.ProjectMaintenance);

            // Check if the sub project has time logged against it
            var timeSheetCount = DataContext.TimesheetEntrySet.Count(t => t.SubProjectId == id);

            if (timeSheetCount > 0)
            {
                return 0;
            }

            // Delete Sub Projects
            var subProject = DataContext.SubProjectSet.FirstOrDefault(p => p.Id == id);
            subProject.IsDeleted = true;

            DataContextSaveChanges();

            return 1;
        }

        #endregion

        #region Project Type

        public IQueryable<ProjectType> ProjectTypeDropdown()
        {
            Authenticate(PrivilegeType.ProjectMaintenance);
            return DataContext.ProjectTypeSet;
        }

        #endregion

        public IQueryable<UserIdentityProject> UserIdentityProjectList(Guid userID)
        {
            Authenticate(PrivilegeType.UserProjectMaintenance);

            return DataContext.UserIdentityProjectSet.Where(u => u.UserAccountId == userID);
        }

        public IQueryable<UserIdentityProject> GetUserAllocatedProjects(Guid userID)
        {
            return DataContext.UserIdentityProjectSet.Where(u => u.UserAccountId == userID)
                .Include(p=> p.Project)
                .Include(p=> p.Project.Client)
                .Include(p=> p.SubProject);
        }

        public void SaveUserIdentityProject(Guid userId, List<UserIdentityProject> projects)
        {
            Authenticate(PrivilegeType.UserProjectMaintenance);

            // Remove old
            var toDelete = DataContext.UserIdentityProjectSet.Where(x => x.UserAccountId == userId);
            DataContext.UserIdentityProjectSet.RemoveRange(toDelete);
            DataContext.UserIdentityProjectSet.AddRange(projects);
            DataContextSaveChanges();

        }

        public void AddUserIdentityProjects(Guid userId, List<UserIdentityProject> projects)
        {
            Authenticate(PrivilegeType.UserProjectMaintenance);
            DataContext.UserIdentityProjectSet.AddRange(projects);

            DataContextSaveChanges();

        }

        public void RemoveUserIdentityProjects(Guid userId, List<UserIdentityProject> projects)
        {
            Authenticate(PrivilegeType.UserProjectMaintenance);

            foreach (var project in projects)
            {
                DataContext.UserIdentityProjectSet.RemoveRange(DataContext.UserIdentityProjectSet.Where(u => u.Id == project.Id).ToList());
            }
            DataContextSaveChanges();

        }

    }
}