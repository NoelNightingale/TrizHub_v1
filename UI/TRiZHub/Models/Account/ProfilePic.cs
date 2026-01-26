#region Usings

using System;

#endregion

namespace TRiZHub.Models.Account
{
    public class ProfilePic
    {
        public Guid ProfilePictureId { get; set; }

        public string ProfileImageBase64 { get; set; }
    }
}