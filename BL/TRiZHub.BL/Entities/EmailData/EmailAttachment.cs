#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;

#endregion

namespace TRiZHub.BL.Entities.EmailData
{
    [Table("EmailAttachment")]
    public class EmailAttachment : DbEntity
    {
        [Index("IDX_EmailAttachment", Order = 0)]
        public virtual Guid EmailQueueId { get; set; }

        [ForeignKey("EmailQueueId")]
        public virtual EmailQueue EmailQueue { get; set; }

        [Required]
        public virtual string FileName { get; set; }

        public virtual byte[] FileData { get; set; }
    }
}