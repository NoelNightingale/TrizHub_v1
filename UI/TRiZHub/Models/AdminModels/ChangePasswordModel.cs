#region Usings

using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.AdminModels
{
    public class ChangePasswordModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}