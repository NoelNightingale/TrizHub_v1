#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public class WorkGroupTree
    {
        public bool IsAdmin { get; set; }
        public List<WorkGroupNode> Groups { get; set; }
    }

    public class WorkGroupNode
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WorkGroupClientRef> Clients { get; set; }
        public bool IsActive { get; set; }
        public bool CanEdit { get; set; }
        public bool CanAddTeam { get; set; }
        public bool CanDelete { get; set; }
        public List<WorkGroupPerson> Managers { get; set; }
        public List<WorkTeamNode> Teams { get; set; }
    }

    public class WorkGroupClientRef
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
    }

    public class WorkTeamNode
    {
        public Guid Id { get; set; }
        public Guid WorkGroupId { get; set; }
        public Guid TeamTypeId { get; set; }
        public string TeamTypeName { get; set; }
        public string Label { get; set; }
        public bool IsActive { get; set; }
        public bool CanEdit { get; set; }
        public bool CanEditLeads { get; set; }
        public bool CanEditMembers { get; set; }
        public bool CanDelete { get; set; }
        public List<WorkGroupPerson> Leads { get; set; }
        public List<WorkGroupPerson> Members { get; set; }
    }

    public class WorkGroupPerson
    {
        public Guid Id { get; set; }
        public Guid UserAccountId { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public bool IsHomeTeam { get; set; }
        public bool CanView { get; set; }
    }

    public class WorkGroupManagerSave
    {
        public Guid UserAccountId { get; set; }
        public bool CanView { get; set; }
    }
}
