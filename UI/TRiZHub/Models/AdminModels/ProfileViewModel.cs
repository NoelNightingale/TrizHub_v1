#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.AdminModels
{
    public class ProfileViewModel
    {
        public Guid ProfileImageId { get; set; }
        public FileDataModel ProfileImage { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        public string ProfileImageLocation { get; set; }
    }
}