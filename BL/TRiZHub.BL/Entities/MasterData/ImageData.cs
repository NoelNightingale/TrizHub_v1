#region Usings

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using TCR.Lib.BL;
using TCR.Lib.Utility;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.MasterData
{
    [Table("ImageData")]
    public class ImageData : DbEntity
    {
        [MaxLength(500)]
        [Required]
        public virtual string FileName { get; set; }

        [Required]
        public virtual byte[] FileData { get; set; }

        #region Navigation

        public virtual ICollection<UserIdentity> Profiles { get; set; }

        #endregion

        #region Static methods

        //resize to fixed size for profile images

        public static byte[] CreateGenericImage(byte[] profileImageData)
        {
            return ImageUtils.GetThumbnail(profileImageData, new Size(150, 150));
        }

        #endregion
    }
}