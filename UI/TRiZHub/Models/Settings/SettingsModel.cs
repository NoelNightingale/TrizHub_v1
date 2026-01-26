#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.Settings
{
    public class SettingsModel
    {
        [Required]
        public string EmailFromAddress { get; set; }

        [Required]
        public string EmailFromName { get; set; }

        [Required]
        public string ApplicationName { get; set; }

        public Guid? DailyQuizId { get; set; }
        public DateTime? DailyQuizGeneratedDateTime { get; set; }
        public bool CurrentUserDailyQuizDone { get; set; }
        public bool NewDailyQuizAvailable { get; set; }
    }
}