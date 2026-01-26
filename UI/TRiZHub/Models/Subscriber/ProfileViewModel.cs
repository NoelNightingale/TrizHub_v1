#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace TRiZHub.Models.Subscriber
{
    public class ProfileViewModel
    {
        public Guid? Id { get; set; }

        public Guid? CountryId { get; set; }

        public Guid? ProfileImageId { get; set; }

        public FileDataModel ImageData { get; set; }

        public string ImageURL { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Identification { get; set; }

        [Required]
        public string PhysicalAddressLine1 { get; set; }

        [Required]
        public string PhysicalAddressLine2 { get; set; }

        [Required]
        public string PhysicalAddressCity { get; set; }

        [Required]
        public virtual string MobileNumber { get; set; }

        [MaxLength(20)]
        public string LandlineNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public virtual bool ProfileCompleted { get; set; }
    }
}