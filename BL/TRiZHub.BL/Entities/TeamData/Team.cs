#region Usings

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.TimesheetData;

#endregion

namespace TRiZHub.BL.Entities.TeamData
{
    [Table("Team")]
    public class Team : DbEntity
    {
        [Required]
        [MaxLength(500)]
        public virtual string TeamName { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual List<TimesheetEntry> TimesheetEntries { get; set; }
    }
}