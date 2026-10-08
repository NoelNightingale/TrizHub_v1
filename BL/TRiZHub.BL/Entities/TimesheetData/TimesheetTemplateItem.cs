#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ActivityData;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.TeamData;

#endregion

namespace TRiZHub.BL.Entities.TimesheetData
{
    [Table("TimesheetTemplateItem")]
    public class TimesheetTemplateItem : DbEntity
    {
        public virtual Guid TimesheetTemplateId { get; set; }

        [ForeignKey("TimesheetTemplateId")]
        public virtual TimesheetTemplate TimesheetTemplate { get; set; }

        /// <summary>Mon=0 … Sun=6 (clipboard dayOffset).</summary>
        public virtual int DayOffset { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public virtual Guid? SubProjectId { get; set; }

        [ForeignKey("SubProjectId")]
        public virtual SubProject SubProject { get; set; }

        public virtual Guid TeamId { get; set; }

        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        public virtual Guid ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        public virtual decimal Hours { get; set; }

        [Required]
        public virtual string Comments { get; set; }

        [MaxLength(500)]
        public virtual string ProjectDescription { get; set; }

        [MaxLength(500)]
        public virtual string ClientEntityName { get; set; }

        public virtual bool? Billable { get; set; }
    }
}
