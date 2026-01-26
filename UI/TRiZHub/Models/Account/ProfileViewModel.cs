#region Usings

using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.Account
{
    public class ProfileViewModel
    {
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        public string ProfileImage { get; set; }

        public string Department { get; set; }
    }
}