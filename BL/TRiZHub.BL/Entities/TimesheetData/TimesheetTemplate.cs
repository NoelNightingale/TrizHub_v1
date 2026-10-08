#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.TimesheetData
{
    [Table("TimesheetTemplate")]
    public class TimesheetTemplate : DbEntity
    {
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        [Required]
        [MaxLength(200)]
        public virtual string Name { get; set; }

        /// <summary>Clipboard shape: "day" or "week".</summary>
        [Required]
        [MaxLength(20)]
        public virtual string TemplateType { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateModified { get; set; }

        public virtual ICollection<TimesheetTemplateItem> Items { get; set; }
    }
}
