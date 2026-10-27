#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.WorkTeamData;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public class WorkGroupProvider : TRiZHubProvider, IWorkGroupProvider
    {
        public WorkGroupProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        public WorkGroupTree Tree()
        {
            var actorId = CurrentUser == null ? Guid.Empty : CurrentUser.Id;
            var isAdmin = IsAdmin();

            var groups = DataContext.WorkGroupSet
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Description,
                    g.IsActive
                })
                .ToList();

            var clients = DataContext.WorkGroupClientSet
                .Select(c => new
                {
                    c.WorkGroupId,
                    c.ClientId,
                    ClientName = c.Client.EntityName
                })
                .ToList();

            var managers = DataContext.WorkGroupManagerSet
                .Select(m => new
                {
                    m.Id,
                    m.WorkGroupId,
                    m.UserAccountId,
                    m.UserAccount.FirstName,
                    m.UserAccount.Surname
                })
                .ToList();

            var managerCaps = DataContext.WorkGroupManagerCapabilitySet
                .Select(c => new { c.WorkGroupManagerId, c.Capability, c.IsAllowed })
                .ToList();

            var teams = DataContext.WorkTeamSet
                .Select(t => new
                {
                    t.Id,
                    t.WorkGroupId,
                    t.TeamTypeId,
                    TypeName = t.TeamType.TeamName,
                    t.IsActive
                })
                .ToList();

            var people = DataContext.WorkTeamMemberSet
                .Select(m => new
                {
                    m.Id,
                    m.WorkTeamId,
                    m.UserAccountId,
                    m.Role,
                    m.UserAccount.FirstName,
                    m.UserAccount.Surname,
                    m.IsHomeTeam
                })
                .ToList();

            var memberCaps = DataContext.WorkTeamMemberCapabilitySet
                .Select(c => new { c.WorkTeamMemberId, c.Capability, c.IsAllowed })
                .ToList();

            var managedGroupIds = new HashSet<Guid>(managers.Where(m => m.UserAccountId == actorId).Select(m => m.WorkGroupId));
            var leadTeamIds = new HashSet<Guid>(people.Where(p => p.UserAccountId == actorId && p.Role == WorkTeamRoleType.Lead).Select(p => p.WorkTeamId));
            var memberTeamIds = new HashSet<Guid>(people.Where(p => p.UserAccountId == actorId && p.Role == WorkTeamRoleType.Member).Select(p => p.WorkTeamId));

            var nodes = new List<WorkGroupNode>();
            foreach (var group in groups.OrderBy(g => g.Name))
            {
                var isManager = managedGroupIds.Contains(group.Id);
                var groupTeams = teams.Where(t => t.WorkGroupId == group.Id).ToList();
                var hasATeam = groupTeams.Any(t => leadTeamIds.Contains(t.Id) || memberTeamIds.Contains(t.Id));
                if (!WorkGroupAccess.CanSeeGroup(isAdmin, isManager, hasATeam))
                    continue;

                var manage = WorkGroupAccess.CanManageStructure(isAdmin, isManager);
                var groupClients = clients
                    .Where(c => c.WorkGroupId == group.Id)
                    .OrderBy(c => c.ClientName)
                    .Select(c => new WorkGroupClientRef { ClientId = c.ClientId, ClientName = c.ClientName })
                    .ToList();

                var managerNodes = managers
                    .Where(m => m.WorkGroupId == group.Id)
                    .Select(m => Person(
                        m.Id,
                        m.UserAccountId,
                        m.FirstName,
                        m.Surname,
                        false,
                        HasCapability(managerCaps.Where(c => c.WorkGroupManagerId == m.Id).Select(c => new Cap(c.Capability, c.IsAllowed)), WorkAssignmentCapability.View)))
                    .OrderBy(p => p.Name)
                    .ToList();

                var teamNodes = new List<WorkTeamNode>();
                foreach (var team in groupTeams.OrderBy(t => t.TypeName))
                {
                    var isLead = leadTeamIds.Contains(team.Id);
                    var isOnTeam = isLead || memberTeamIds.Contains(team.Id);
                    if (!WorkGroupAccess.CanSeeTeam(isAdmin, isManager, isOnTeam))
                        continue;

                    var teamPeople = people.Where(p => p.WorkTeamId == team.Id).ToList();
                    var leads = teamPeople.Where(p => p.Role == WorkTeamRoleType.Lead)
                        .Select(p => Person(
                            p.Id,
                            p.UserAccountId,
                            p.FirstName,
                            p.Surname,
                            p.IsHomeTeam,
                            HasCapability(memberCaps.Where(c => c.WorkTeamMemberId == p.Id).Select(c => new Cap(c.Capability, c.IsAllowed)), WorkAssignmentCapability.View)))
                        .OrderBy(p => p.Name)
                        .ToList();
                    var members = teamPeople.Where(p => p.Role == WorkTeamRoleType.Member)
                        .Select(p => Person(
                            p.Id,
                            p.UserAccountId,
                            p.FirstName,
                            p.Surname,
                            p.IsHomeTeam,
                            HasCapability(memberCaps.Where(c => c.WorkTeamMemberId == p.Id).Select(c => new Cap(c.Capability, c.IsAllowed)), WorkAssignmentCapability.View)))
                        .OrderBy(p => p.Name)
                        .ToList();

                    teamNodes.Add(new WorkTeamNode
                    {
                        Id = team.Id,
                        WorkGroupId = group.Id,
                        TeamTypeId = team.TeamTypeId,
                        TeamTypeName = team.TypeName,
                        Label = Label(group.Name, team.TypeName),
                        IsActive = team.IsActive,
                        CanEdit = manage,
                        CanEditLeads = manage,
                        CanEditMembers = WorkGroupAccess.CanEditMembers(isAdmin, isManager, isLead, group.IsActive, team.IsActive),
                        CanDelete = WorkGroupAccess.CanDeleteTeam(manage, teamPeople.Count),
                        Leads = leads,
                        Members = members
                    });
                }

                nodes.Add(new WorkGroupNode
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Clients = groupClients,
                    IsActive = group.IsActive,
                    CanEdit = WorkGroupAccess.CanEditGroup(isAdmin),
                    CanAddTeam = manage,
                    CanDelete = WorkGroupAccess.CanDeleteGroup(isAdmin, groupTeams.Count, managerNodes.Count),
                    Managers = managerNodes,
                    Teams = teamNodes
                });
            }

            return new WorkGroupTree { IsAdmin = isAdmin, Groups = nodes };
        }

        public Guid SaveGroup(Guid? id, string name, string description, IList<Guid> clientIds, bool isActive, IList<WorkGroupManagerSave> managers)
        {
            if (!IsAdmin())
                throw new WorkGroupException("You are not allowed to change this work group.");

            name = (name ?? "").Trim();
            if (name.Length == 0)
                throw new WorkGroupException("A work group needs a name.");
            if (name.Length > 500)
                throw new WorkGroupException("The work group name is too long.");
            description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            if (description != null && description.Length > 2000)
                throw new WorkGroupException("The description is too long.");

            var wantedClients = (clientIds ?? new List<Guid>()).Where(g => g != Guid.Empty).Distinct().ToList();
            if (wantedClients.Count == 0)
                throw new WorkGroupException("Choose at least one client for this work group.");
            if (DataContext.ClientEntitySet.Count(c => wantedClients.Contains(c.Id) && !c.IsDeleted) != wantedClients.Count)
                throw new WorkGroupException("Choose valid clients for this work group.");

            var wantedManagers = (managers ?? new List<WorkGroupManagerSave>())
                .Where(m => m != null && m.UserAccountId != Guid.Empty)
                .GroupBy(m => m.UserAccountId)
                .Select(g => g.Last())
                .ToList();

            WorkGroup record;
            if (id == null || id == Guid.Empty)
            {
                record = new WorkGroup();
                DataContext.WorkGroupSet.Add(record);
            }
            else
            {
                record = DataContext.WorkGroupSet.FirstOrDefault(g => g.Id == id.Value);
                if (record == null)
                    throw new WorkGroupException("The work group could not be found.");
            }

            record.Name = name;
            record.Description = description;
            record.IsActive = isActive;

            var existingClients = id == null || id == Guid.Empty
                ? new List<WorkGroupClient>()
                : DataContext.WorkGroupClientSet.Where(c => c.WorkGroupId == record.Id).ToList();
            var removeClients = existingClients.Where(c => !wantedClients.Contains(c.ClientId)).ToList();
            if (removeClients.Count > 0)
                DataContext.WorkGroupClientSet.RemoveRange(removeClients);
            var alreadyClients = new HashSet<Guid>(existingClients.Select(c => c.ClientId));
            foreach (var clientId in wantedClients.Where(c => !alreadyClients.Contains(c)))
            {
                DataContext.WorkGroupClientSet.Add(new WorkGroupClient
                {
                    WorkGroupId = record.Id,
                    ClientId = clientId
                });
            }

            var existingManagers = id == null || id == Guid.Empty
                ? new List<WorkGroupManager>()
                : DataContext.WorkGroupManagerSet.Where(m => m.WorkGroupId == record.Id).ToList();
            var wantedUserIds = new HashSet<Guid>(wantedManagers.Select(m => m.UserAccountId));
            var removeManagers = existingManagers.Where(m => !wantedUserIds.Contains(m.UserAccountId)).ToList();
            if (removeManagers.Count > 0)
            {
                var removeIds = removeManagers.Select(m => m.Id).ToList();
                var caps = DataContext.WorkGroupManagerCapabilitySet.Where(c => removeIds.Contains(c.WorkGroupManagerId)).ToList();
                if (caps.Count > 0)
                    DataContext.WorkGroupManagerCapabilitySet.RemoveRange(caps);
                DataContext.WorkGroupManagerSet.RemoveRange(removeManagers);
            }

            foreach (var existing in existingManagers.Where(m => wantedUserIds.Contains(m.UserAccountId)))
            {
                var save = wantedManagers.First(m => m.UserAccountId == existing.UserAccountId);
                SetManagerCapability(existing.Id, WorkAssignmentCapability.View, save.CanView);
            }

            var alreadyManagers = new HashSet<Guid>(existingManagers.Select(m => m.UserAccountId));
            foreach (var save in wantedManagers.Where(m => !alreadyManagers.Contains(m.UserAccountId)))
            {
                EnsureActiveUser(save.UserAccountId);
                var row = new WorkGroupManager
                {
                    WorkGroupId = record.Id,
                    UserAccountId = save.UserAccountId
                };
                DataContext.WorkGroupManagerSet.Add(row);
                DataContext.WorkGroupManagerCapabilitySet.Add(new WorkGroupManagerCapability
                {
                    WorkGroupManagerId = row.Id,
                    Capability = WorkAssignmentCapability.View,
                    IsAllowed = save.CanView
                });
            }

            DataContextSaveChanges();
            return record.Id;
        }

        public void DeleteGroup(Guid id)
        {
            if (!IsAdmin())
                throw new WorkGroupException("You are not allowed to delete this work group.");

            var record = DataContext.WorkGroupSet.FirstOrDefault(g => g.Id == id);
            if (record == null)
                throw new WorkGroupException("The work group could not be found.");
            if (DataContext.WorkTeamSet.Any(t => t.WorkGroupId == id))
                throw new WorkGroupException("Remove the teams under this work group before deleting it.");
            if (DataContext.WorkGroupManagerSet.Any(m => m.WorkGroupId == id))
                throw new WorkGroupException("Remove the managers before deleting this work group.");

            var clients = DataContext.WorkGroupClientSet.Where(c => c.WorkGroupId == id).ToList();
            if (clients.Count > 0)
                DataContext.WorkGroupClientSet.RemoveRange(clients);

            DataContext.WorkGroupSet.Remove(record);
            DataContextSaveChanges();
        }

        public Guid SaveTeam(Guid? id, Guid workGroupId, Guid teamTypeId, bool isActive)
        {
            var group = DataContext.WorkGroupSet.FirstOrDefault(g => g.Id == workGroupId);
            if (group == null)
                throw new WorkGroupException("The work group could not be found.");
            if (!WorkGroupAccess.CanManageStructure(IsAdmin(), IsManagerOf(workGroupId)))
                throw new WorkGroupException("You are not allowed to change teams in this work group.");

            WorkTeam record;
            if (id == null || id == Guid.Empty)
            {
                if (teamTypeId == Guid.Empty || !DataContext.TeamSet.Any(t => t.Id == teamTypeId && t.IsActive))
                    throw new WorkGroupException("Choose an active team type.");
                if (DataContext.WorkTeamSet.Any(t => t.WorkGroupId == workGroupId && t.TeamTypeId == teamTypeId))
                    throw new WorkGroupException("This work group already has a team of that type.");

                record = new WorkTeam { WorkGroupId = workGroupId, TeamTypeId = teamTypeId, IsActive = isActive };
                DataContext.WorkTeamSet.Add(record);
            }
            else
            {
                record = DataContext.WorkTeamSet.FirstOrDefault(t => t.Id == id.Value && t.WorkGroupId == workGroupId);
                if (record == null)
                    throw new WorkGroupException("The work team could not be found.");
                record.IsActive = isActive;
            }

            DataContextSaveChanges();
            return record.Id;
        }

        public void DeleteTeam(Guid id)
        {
            var record = DataContext.WorkTeamSet.FirstOrDefault(t => t.Id == id);
            if (record == null)
                throw new WorkGroupException("The work team could not be found.");
            if (!WorkGroupAccess.CanManageStructure(IsAdmin(), IsManagerOf(record.WorkGroupId)))
                throw new WorkGroupException("You are not allowed to delete this work team.");
            if (DataContext.WorkTeamMemberSet.Any(m => m.WorkTeamId == id))
                throw new WorkGroupException("Remove the leads and members before deleting this team.");

            DataContext.WorkTeamSet.Remove(record);
            DataContextSaveChanges();
        }

        public Guid SavePerson(Guid workTeamId, Guid userAccountId, WorkTeamRoleType role, bool isHomeTeam, bool canView)
        {
            if (role != WorkTeamRoleType.Lead && role != WorkTeamRoleType.Member)
                throw new WorkGroupException("Choose lead or member.");

            var team = DataContext.WorkTeamSet
                .Where(t => t.Id == workTeamId)
                .Select(t => new { t.Id, t.WorkGroupId, t.IsActive, GroupActive = t.WorkGroup.IsActive })
                .FirstOrDefault();
            if (team == null)
                throw new WorkGroupException("The work team could not be found.");

            var isAdmin = IsAdmin();
            var isManager = IsManagerOf(team.WorkGroupId);
            var isLead = IsLeadOf(team.Id);
            if (role == WorkTeamRoleType.Lead)
            {
                if (!WorkGroupAccess.CanManageStructure(isAdmin, isManager))
                    throw new WorkGroupException("You are not allowed to change the leads on this team.");
            }
            else if (!WorkGroupAccess.CanEditMembers(isAdmin, isManager, isLead, team.GroupActive, team.IsActive))
            {
                throw new WorkGroupException(team.IsActive && team.GroupActive
                    ? "You are not allowed to change the members on this team."
                    : "Members can only be changed on an active team.");
            }

            var existing = DataContext.WorkTeamMemberSet.FirstOrDefault(m => m.WorkTeamId == workTeamId && m.UserAccountId == userAccountId);
            if (existing != null)
            {
                if (existing.Role != role)
                    throw new WorkGroupException("This person is already on the team. Remove the other role first.");

                existing.IsHomeTeam = isHomeTeam;
                SetMemberCapability(existing.Id, WorkAssignmentCapability.View, canView);
                DataContextSaveChanges();
                return existing.Id;
            }

            EnsureActiveUser(userAccountId);
            var row = new WorkTeamMember
            {
                WorkTeamId = workTeamId,
                UserAccountId = userAccountId,
                Role = role,
                IsHomeTeam = isHomeTeam
            };
            DataContext.WorkTeamMemberSet.Add(row);
            DataContext.WorkTeamMemberCapabilitySet.Add(new WorkTeamMemberCapability
            {
                WorkTeamMemberId = row.Id,
                Capability = WorkAssignmentCapability.View,
                IsAllowed = canView
            });
            DataContextSaveChanges();
            return row.Id;
        }

        public void DeletePerson(Guid id)
        {
            var row = DataContext.WorkTeamMemberSet.FirstOrDefault(m => m.Id == id);
            if (row == null)
                throw new WorkGroupException("That person is no longer on the team.");

            var team = DataContext.WorkTeamSet
                .Where(t => t.Id == row.WorkTeamId)
                .Select(t => new { t.WorkGroupId, TeamActive = t.IsActive, GroupActive = t.WorkGroup.IsActive })
                .FirstOrDefault();
            if (team == null)
                throw new WorkGroupException("The work team could not be found.");

            var isAdmin = IsAdmin();
            var isManager = IsManagerOf(team.WorkGroupId);
            if (row.Role == WorkTeamRoleType.Lead)
            {
                if (!WorkGroupAccess.CanManageStructure(isAdmin, isManager))
                    throw new WorkGroupException("You are not allowed to change the leads on this team.");
            }
            else if (!WorkGroupAccess.CanEditMembers(isAdmin, isManager, IsLeadOf(row.WorkTeamId), team.GroupActive, team.TeamActive))
            {
                throw new WorkGroupException(team.TeamActive && team.GroupActive
                    ? "You are not allowed to change the members on this team."
                    : "Members can only be changed on an active team.");
            }

            var caps = DataContext.WorkTeamMemberCapabilitySet.Where(c => c.WorkTeamMemberId == id).ToList();
            if (caps.Count > 0)
                DataContext.WorkTeamMemberCapabilitySet.RemoveRange(caps);

            DataContext.WorkTeamMemberSet.Remove(row);
            DataContextSaveChanges();
        }

        private bool IsAdmin()
        {
            return CurrentUser != null && UserIsAllowed(PrivilegeType.WorkGroupAdmin);
        }

        private bool IsManagerOf(Guid workGroupId)
        {
            return CurrentUser != null
                   && DataContext.WorkGroupManagerSet.Any(m => m.WorkGroupId == workGroupId && m.UserAccountId == CurrentUser.Id);
        }

        private bool IsLeadOf(Guid workTeamId)
        {
            return CurrentUser != null
                   && DataContext.WorkTeamMemberSet.Any(m => m.WorkTeamId == workTeamId
                                                             && m.UserAccountId == CurrentUser.Id
                                                             && m.Role == WorkTeamRoleType.Lead);
        }

        private void EnsureActiveUser(Guid userAccountId)
        {
            if (!DataContext.UserAccountSet.Any(u => u.Id == userAccountId && u.Active))
                throw new WorkGroupException("Choose an active user.");
        }

        private void SetManagerCapability(Guid managerId, WorkAssignmentCapability capability, bool isAllowed)
        {
            var row = DataContext.WorkGroupManagerCapabilitySet
                .FirstOrDefault(c => c.WorkGroupManagerId == managerId && c.Capability == capability);
            if (row == null)
            {
                DataContext.WorkGroupManagerCapabilitySet.Add(new WorkGroupManagerCapability
                {
                    WorkGroupManagerId = managerId,
                    Capability = capability,
                    IsAllowed = isAllowed
                });
                return;
            }
            row.IsAllowed = isAllowed;
        }

        private void SetMemberCapability(Guid memberId, WorkAssignmentCapability capability, bool isAllowed)
        {
            var row = DataContext.WorkTeamMemberCapabilitySet
                .FirstOrDefault(c => c.WorkTeamMemberId == memberId && c.Capability == capability);
            if (row == null)
            {
                DataContext.WorkTeamMemberCapabilitySet.Add(new WorkTeamMemberCapability
                {
                    WorkTeamMemberId = memberId,
                    Capability = capability,
                    IsAllowed = isAllowed
                });
                return;
            }
            row.IsAllowed = isAllowed;
        }

        private static bool HasCapability(IEnumerable<Cap> caps, WorkAssignmentCapability capability)
        {
            var row = caps.FirstOrDefault(c => c.Capability == capability);
            return row == null || row.IsAllowed;
        }

        private static WorkGroupPerson Person(Guid id, Guid userAccountId, string firstName, string surname,
            bool isHomeTeam, bool canView)
        {
            var first = (firstName ?? "").Trim();
            var last = (surname ?? "").Trim();
            var initials = (first.Length > 0 ? first.Substring(0, 1) : "") + (last.Length > 0 ? last.Substring(0, 1) : "");
            return new WorkGroupPerson
            {
                Id = id,
                UserAccountId = userAccountId,
                Name = (first + " " + last).Trim(),
                Initials = initials.ToUpper(),
                IsHomeTeam = isHomeTeam,
                CanView = canView
            };
        }

        private static string Label(string groupName, string teamTypeName)
        {
            return (groupName ?? "").Trim() + " — " + (teamTypeName ?? "").Trim();
        }

        private sealed class Cap
        {
            public Cap(WorkAssignmentCapability capability, bool isAllowed)
            {
                Capability = capability;
                IsAllowed = isAllowed;
            }

            public WorkAssignmentCapability Capability { get; private set; }
            public bool IsAllowed { get; private set; }
        }
    }
}
