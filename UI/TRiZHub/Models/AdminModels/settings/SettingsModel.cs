#region Usings

using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.AdminModels.settings
{
    public class SettingsModel
    {
        [Required]
        public string EmailFromAddress { get; set; }

        [Required]
        public string EmailFromName { get; set; }

        [Required]
        public string AboutApp { get; set; }
    }
}