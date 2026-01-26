#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Hosting;
using TCR.Lib.BL;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Entities.PersonalInformationData
{
    [Table("PersonalInformation")]
    public class PersonalInformation : DbEntity
    {
        [Index("IDX_PersonalInformationUserAccount", Order = 0)]
        public virtual Guid UserAccountId { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string FullNames { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Surname { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string IdNumber { get; set; }

        [Required]
        public virtual DateTime Dob { get; set; }

        [MaxLength(500)]
        public virtual string SpouseName { get; set; }

        [MaxLength(500)]
        public virtual string Children { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Company { get; set; }

        [Required]
        public virtual DateTime WorkExperienceStartDate { get; set; }

        [Required]
        public virtual DateTime EmploymentStartDate { get; set; }

        public virtual DateTime? EmploymentEndDate { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Race { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Gender { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string DoorTagNumber { get; set; }

      
        [MaxLength(500)]
        public virtual string PhoneExtension { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string CellPhone { get; set; }

        
        [MaxLength(500)]
        public virtual string LandLinePhone { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string CompanyEmail { get; set; }

        [MaxLength(500)]
        public virtual string OtherEmail { get; set; }

        [MaxLength(500)]
        public virtual string AccessLevel { get; set; }

        [MaxLength(500)]
        public virtual string MedicalScheme { get; set; }

        [MaxLength(500)]
        public virtual string MedicalSchemeOption { get; set; }

        [MaxLength(500)]
        public virtual string MedicalAidNumber { get; set; }

    }
}