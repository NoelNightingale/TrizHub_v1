#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.UserIdentityProject;

#endregion

namespace TRiZHub.BL.Provider.ProjectData
{
    public interface IProjectProvider : ITRiZHubProvider
    {
        #region Project

        IQueryable<Project> ProjectList();
        IQueryable<Project> ProjectListForClientReporter();
        Project GetProject(Guid id);

        Project SaveProject(Guid? id, Guid clientId, Guid? projectLeadId, Guid? projectTypeId,
            string projectName, string projectNumber, string projectDescription, bool billable, bool isActive, bool? excludeTimeCapture);

        int DeleteProject(Guid id);

        #endregion

        #region Sub Project

        IQueryable<SubProject> SubProjectList();

        SubProject GetSubProject(Guid id);

        SubProject SaveSubProject(Guid? id, Guid projectId, Guid? subProjectTypeId, string projectName, string subProjectNumber, bool isActive);

        int DeleteSubProject(Guid id);

        #endregion

        #region Project Type
        IQueryable<ProjectType> ProjectTypeDropdown();
        #endregion

        IQueryable<UserIdentityProject> UserIdentityProjectList(Guid userID);
        IQueryable<UserIdentityProject> GetUserAllocatedProjects(Guid userID);
        void SaveUserIdentityProject(Guid userId, List<UserIdentityProject> projects);

        void AddUserIdentityProjects(Guid userId, List<UserIdentityProject> projects);

        void RemoveUserIdentityProjects(Guid userId, List<UserIdentityProject> projects);


    }
}