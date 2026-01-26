using System;

namespace TRiZHub.Models.TeamModels
{
    public class TeamGridModel
    {
        public Guid? Id { get; set; }

        public string TeamName { get; set; }

        public bool IsActive { get; set; }
    }
}