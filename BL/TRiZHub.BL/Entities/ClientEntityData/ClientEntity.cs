#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.ClientEntityData
{
    [Table("ClientEntity")]
    public class ClientEntity : DbEntity
    {

        public virtual DateTime DateCreated { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual bool IsDeleted { get; set; }

        [Required]
        public virtual string EntityName { get; set; }

        public virtual List<Project> Projects { get; set; }

        public virtual List<TimesheetEntry> TimesheetEntries { get; set; }
    }
}