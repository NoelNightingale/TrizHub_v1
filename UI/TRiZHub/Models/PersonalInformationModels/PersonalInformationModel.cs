#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.PersonalInformationModels
{
    public class PersonalInformationModel
    {
        public Guid Id { get; set; }

        public Guid UserAccountId { get; set; }

        public string FirstName { get; set; }

        public string Account { get; set; }

        [Required]
        public string FullNames { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string IdNumber { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        public string SpouseName { get; set; }

        public string Children { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public DateTime WorkExperienceStartDate { get; set; }

        [Required]
        public DateTime EmploymentStartDate { get; set; }

        public DateTime? EmploymentEndDate { get; set; }

        [Required]
        public string Race { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string DoorTagNumber { get; set; }

        [Required]
        public string PhoneExtension { get; set; }

        [Required]
        public virtual string CellPhone { get; set; }

        [Required]
        [MaxLength(500)]
        public string LandLinePhone { get; set; }

        [Required]
        public string CompanyEmail { get; set; }

        public string OtherEmail { get; set; }

        public string AccessLevel { get; set; }

        public string MedicalScheme { get; set; }

        public string MedicalSchemeOption { get; set; }

        public string MedicalAidNumber { get; set; }


    }
}