#region Usings

using System;

#endregion

namespace TRiZHub.Models.WorkGroupModels
{
    public class WorkTeamSaveModel
    {
        public Guid? Id { get; set; }
        public Guid WorkGroupId { get; set; }
        public Guid TeamTypeId { get; set; }
        public bool IsActive { get; set; }
    }

    public class WorkTeamPersonSaveModel
    {
        public Guid WorkTeamId { get; set; }
        public Guid UserAccountId { get; set; }
        public int Role { get; set; }
        public bool IsHomeTeam { get; set; }
        public bool CanView { get; set; }
    }

    public class WorkGroupIdModel
    {
        public Guid Id { get; set; }
    }
}
