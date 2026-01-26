#region Usings

using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.Account
{
    public class EditProfileModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string EmailAddress { get; set; }
    }
}