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
    [Table("ProjectType")]
    public class ProjectType : DbEntity
    {
        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual string Description { get; set; }

        [Required]
        public virtual bool AllowSubProjectAlternativeType { get; set; }

        [Required]
        public virtual bool AllowSubProjectBillable { get; set; }

        [Required]
        public virtual int SortOrder { get; set; }
    }
}