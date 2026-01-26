#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.TravelInformationData
{
    [Table("TravelInformation")]
    public class TravelInformation : DbEntity
    {
        [Index("IDX_TravelInformtionUserAccount", Order = 0)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        [MaxLength(500)]
        public virtual string DocumentType { get; set; }

        public virtual string Number { get; set; }

        public virtual DateTime ExpiryDate { get; set; }
    }
}