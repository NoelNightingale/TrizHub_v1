#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.MasterData;
using TRiZHub.BL.Entities.PersonalInformationData;

#endregion

namespace TRiZHub.BL.Entities.SecurityData
{
    [Table("UserIdentity")]
    public abstract class UserIdentity : DbEntity
    {
        [Index("UIDX_UserIdentity", IsUnique = true, Order = 0)]
        [MaxLength(200)]
        [Required]
        public virtual string AccountName { get; set; }

        [Index("UIDX_UserIdentity", IsUnique = true, Order = 1)]
        public virtual bool IsSystemAdmin { get; set; }

        [MaxLength(200)]
        public virtual string FirstName { get; set; }

        [MaxLength(200)]
        public virtual string Surname { get; set; }

        public virtual Guid ProfileImageDataId { get; set; }

        [ForeignKey("ProfileImageDataId")]
        public virtual ImageData ProfileImageData { get; set; }

        [Index("UIDX_UserIdentityRegistered")]
        public virtual DateTime Registered { get; set; }

        public virtual bool Active { get; set; }

        [NotMapped]
        public string Fullname
        {
            get { return string.Format("{0} {1}", FirstName, Surname); }
        }

        [NotMapped]
        public string Initials
        {
            get
            {
                return string.Format("{0}{1}", Surname.Substring(0, 1).ToUpper(), FirstName.Substring(0, 1).ToUpper());
            }
        }
        
    }
}