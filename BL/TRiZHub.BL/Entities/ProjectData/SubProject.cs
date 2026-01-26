#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.TimesheetData;

#endregion

namespace TRiZHub.BL.Entities.ProjectData
{
    [Table("SubProject")]
    public class SubProject : DbEntity
    {
        [Index("IDX_SubProject", Order = 0)]
        public virtual Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public virtual Guid? SubProjectTypeId { get; set; }
        [ForeignKey("SubProjectTypeId")]
        public virtual ProjectType SubProjectType { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string ProjectName { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual bool IsDeleted { get; set; }

        public virtual List<TimesheetEntry> TimesheetEntries { get; set; }

        public virtual string SubProjectNumber { get; set; } 
    }
}