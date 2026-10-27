#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TRiZHub.Models.WorkGroupModels
{
    public class WorkGroupSaveModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Guid> ClientIds { get; set; }
        public bool IsActive { get; set; }
        public List<WorkGroupManagerSaveModel> Managers { get; set; }
    }

    public class WorkGroupManagerSaveModel
    {
        public Guid UserAccountId { get; set; }
        public bool CanView { get; set; }
    }
}
