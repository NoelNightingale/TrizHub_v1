#region Usings

using System;

#endregion

namespace TRiZHub.Models.WorkTeamModels
{
    public class WorkTeamGridModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string TeamTypeName { get; set; }

        public int CurrentPeople { get; set; }

        public bool IsActive { get; set; }
    }
}
