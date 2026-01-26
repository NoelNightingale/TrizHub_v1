#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Entities.EmailData
{
    [Table("EmailQueue")]
    public class EmailQueue : DbEntity
    {
        [Index("IDX_EmailQueue", Order = 0)]
        public EmailStatusType Status { get; set; }

        [Index("IDX_EmailQueue", Order = 1)]
        public DateTime Created { get; set; }

        public DateTime? Processed { get; set; }

        [Required]
        [MaxLength(500)]
        public string ToAddress { get; set; }

        [MaxLength(500)]
        public virtual string CCAddress { get; set; }

        [Required]
        [MaxLength(500)]
        public string Subject { get; set; }

        [Required]
        public string MessageBody { get; set; }

        [MaxLength(1000)]
        public string SendError { get; set; }

        public int SendAttempts { get; set; }

        public virtual ICollection<EmailAttachment> EmailAttachments { get; set; }
    }
}