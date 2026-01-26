#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Entities.Types;

#endregion Usings

namespace TRiZHub.BL.Entities.EmployerData
{
    [Table("Employer")]
    public class Employer : DbEntity
    {
        public virtual Guid Id { get; set; }

        [Required]
        public virtual string Name { get; set; }

        public virtual DateTime DateCreated { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsDeleted { get; set; }
    }
}