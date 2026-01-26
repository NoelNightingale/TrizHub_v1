#region Usings

using System;

#endregion Usings

namespace TRiZHub.Models.TeamJobDesignationModels
{
    public class TeamJobDesignationEditModel
    {
        public Guid? Id { get; set; }

        public Guid UserAccountId { get; set; }

        public Guid ClientId { get; set; }

        public Guid? LineLeaderId { get; set; }
        public Guid? EmployerId { get; set; }

        public string JobDesignation { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Location { get; set; }
    }
}