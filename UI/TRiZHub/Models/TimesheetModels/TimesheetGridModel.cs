#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.TimesheetModels
{
    public class TimesheetGridModel
    {
        public Guid Id { get; set; }

        public Guid UserAccountId { get; set; }
        public string UserAccountName { get; set; }

        public Guid ProjectGridId { get; set; }

        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public Guid? SubProjectId { get; set; }
        public string SubProjectName { get; set; }

        public Project Project { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }

        public Guid ActivityId { get; set; }
        public string ActivityName { get; set; }

        public Guid ClientEntityId { get; set; }
        public string ClientEntityName { get; set; }

        public string Comments { get; set; }

        public decimal Hours { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateEntry { get; set; }

        public bool Billable { get; set; }
    }
}