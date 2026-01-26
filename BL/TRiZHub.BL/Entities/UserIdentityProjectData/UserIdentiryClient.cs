#region Usings

using System;
using System.ComponentModel.DataAnnotations.Schema;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.ClientEntityData;

#endregion Usings

namespace TRiZHub.BL.Entities.UserIdentityClient
{
    [Table("UserIdentityClient")]
    public class UserIdentityClient : DbEntity
    {
        [Index("IDX_UserIdentityClient", Order = 0)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        public virtual Guid? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual ClientEntity Client { get; set; }

    }
}