#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.TimesheetData;

#endregion

namespace TRiZHub.BL.Entities.ProjectData
{
    [Table("Project")]
    public class Project : DbEntity
    {
        [Index("IDX_ProjectClient", Order = 0)]
        public virtual Guid ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual ClientEntity Client { get; set; }

        public virtual Guid? ProjectLeadId { get; set; }

        [ForeignKey("ProjectLeadId")]
        public virtual UserAccount ProjectLead { get; set; }
        
        public virtual Guid? ProjectTypeId { get; set; }
        [ForeignKey("ProjectTypeId")]
        public virtual ProjectType ProjectType { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string ProjectName { get; set; }

        public virtual string ProjectNumber { get; set;}

        public virtual string ProjectDescription { get; set; }

        public virtual bool Billable { get; set; }
        
        public virtual bool? ExcludeTimeCapture { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual bool IsDeleted { get; set; }

        public virtual List<TimesheetEntry> TimesheetEntries { get; set; }

        public virtual List<SubProject> SubProjects { get; set; }
    }
}