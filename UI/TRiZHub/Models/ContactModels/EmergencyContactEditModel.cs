#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.ContactModels
{
    public class EmergencyContactEditModel
    {
        public Guid? Id { get; set; }

        public Guid UserAccountId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Relationship { get; set; }

        [Required]
        public string CellphoneNumber { get; set; }

        public string LandLineNumber { get; set; }

    }
}