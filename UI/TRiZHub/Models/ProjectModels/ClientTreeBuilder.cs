#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.ProjectData;

#endregion

namespace TRiZHub.Models.ProjectModels
{
    /// <summary>
    /// Builds the client / project / subproject selection tree shared by user and team allocation screens.
    /// </summary>
    public class ClientTreeSelection
    {
        public ClientTreeSelection()
        {
            ClientIds = new HashSet<Guid>();
            ProjectIds = new HashSet<Guid>();
            SubProjectIds = new HashSet<Guid>();
        }

        public HashSet<Guid> ClientIds { get; private set; }

        /// <summary>Whole-project selections.</summary>
        public HashSet<Guid> ProjectIds { get; private set; }

        public HashSet<Guid> SubProjectIds { get; private set; }
    }

    public static class ClientTreeBuilder
    {
        public static List<UserIdentityProjectModel> Build(
            IEnumerable<ClientEntity> clients,
            IEnumerable<Project> projects,
            IEnumerable<SubProject> subProjects,
            ClientTreeSelection selected,
            bool includeInactive,
            IDictionary<Guid, string> inheritedFrom = null)
        {
            var clientList = clients.ToList();
            var projectList = projects.ToList();
            var subProjectList = subProjects.ToList();

            if (!includeInactive)
            {
                clientList = clientList.Where(c => c.IsActive).ToList();
                projectList = projectList.Where(p => p.IsActive).ToList();
                subProjectList = subProjectList.Where(s => s.IsActive).ToList();
            }

            var projectsByClient = projectList.ToLookup(p => p.ClientId);
            var subProjectsByProject = subProjectList.ToLookup(s => s.ProjectId);

            Func<Guid, string> inherited = id =>
            {
                string teams;
                return inheritedFrom != null && inheritedFrom.TryGetValue(id, out teams) ? teams : null;
            };

            return clientList
                .Select(client => new UserIdentityProjectModel
                {
                    ClientId = client.Id,
                    Name = client.EntityName,
                    Selected = selected.ClientIds.Contains(client.Id),
                    InheritedFrom = inherited(client.Id),
                    ListOfProjects = projectsByClient[client.Id]
                        .Select(p => new UserIdentityProjectModel
                        {
                            ClientId = client.Id,
                            ProjectId = p.Id,
                            Name = p.ProjectName,
                            Code = p.ProjectNumber,
                            isActive = p.IsActive,
                            Selected = selected.ProjectIds.Contains(p.Id),
                            InheritedFrom = inherited(p.Id),
                            ListOfProjects = subProjectsByProject[p.Id]
                                .Select(sp => new UserIdentityProjectModel
                                {
                                    ClientId = client.Id,
                                    ProjectId = p.Id,
                                    SubProjectId = sp.Id,
                                    Name = sp.ProjectName,
                                    Code = sp.SubProjectNumber,
                                    isActive = sp.IsActive,
                                    Selected = selected.SubProjectIds.Contains(sp.Id),
                                    InheritedFrom = inherited(sp.Id)
                                })
                                .OrderBy(c => c.Name)
                                .ToList()
                        })
                        .OrderBy(c => c.Name)
                        .ToList()
                })
                .OrderBy(c => c.Name)
                .ToList();
        }

        /// <summary>
        /// Flattens a posted tree selection: an empty ProjectId is a whole client, an empty SubProjectId is a whole project.
        /// </summary>
        public static void SplitSelection(IEnumerable<UserIdentityProjectModel> selection,
            out List<Guid> clientIds, out List<KeyValuePair<Guid, Guid?>> projectAndSubProjectIds)
        {
            var items = (selection ?? Enumerable.Empty<UserIdentityProjectModel>()).ToList();
            clientIds = items.Where(p => p.ProjectId == Guid.Empty && p.ClientId != Guid.Empty)
                .Select(p => p.ClientId).Distinct().ToList();
            projectAndSubProjectIds = items.Where(p => p.ProjectId != Guid.Empty)
                .Select(p => new KeyValuePair<Guid, Guid?>(p.ProjectId,
                    p.SubProjectId.HasValue && p.SubProjectId.Value != Guid.Empty ? p.SubProjectId : null))
                .Distinct()
                .ToList();
        }
    }
}
