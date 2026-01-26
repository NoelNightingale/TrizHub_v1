#region Usings

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TCR.Lib.BL;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.TimesheetData;

#endregion

namespace TRiZHub.BL.Entities.ActivityData
{
    [Table("Activity")]
    public class Activity : DbEntity
    {
        [Required]
        [MaxLength(500)]
        public virtual string ActivityName { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual List<TimesheetEntry> TimesheetEntries { get; set; }
    }
}