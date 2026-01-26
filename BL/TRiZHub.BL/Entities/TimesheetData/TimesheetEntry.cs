#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ActivityData;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.TeamData;

#endregion

namespace TRiZHub.BL.Entities.TimesheetData
{
    [Table("TimesheetEntry")]
    public class TimesheetEntry : DbEntity
    {
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        [InverseProperty("TimesheetEntries")]
        public virtual UserAccount UserAccount { get; set; }

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

        public virtual Guid CreatedByAccountId { get; set; }

        [ForeignKey("CreatedByAccountId")]
        [InverseProperty("TimesheetsCreated")]
        public virtual UserAccount CreatedByAccount { get; set; }

        [Required]
        public virtual string Comments { get; set; }

        public virtual decimal Hours { get; set; }

        public virtual DateTime DateEntry { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual bool IsActive { get; set; }
    }
}