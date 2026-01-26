#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.BillingRatesData;
using TRiZHub.BL.Entities.ContactData;
using TRiZHub.BL.Entities.MasterData;
using TRiZHub.BL.Entities.OfficeEquipmentData;
using TRiZHub.BL.Entities.PersonalInformationData;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.ScorecardData;
using TRiZHub.BL.Entities.TeamJobDesignationData;
using TRiZHub.BL.Entities.TimesheetData;
using TRiZHub.BL.Entities.TravelInformationData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Resources;

#endregion

namespace TRiZHub.BL.Entities.SecurityData
{
    [Table("UserAccount")]
    public class UserAccount : UserIdentity
    {
        public virtual ICollection<PersonalInformation> PersonalInformation { get; set; }

        public virtual ICollection<OfficeEquipment> OfficeEquipemnt { get; set; }

        public virtual ICollection<TeamJobDesignation> TeamJobDesignation { get; set; }

        public virtual ICollection<BillingRates> BillingRates { get; set; }

        public virtual ICollection<TravelInformation> TravelInformations { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        public virtual ICollection<EmergancyContact> EmergancyContacts { get; set; }

        public virtual List<Project> Projects { get; set; }

        public virtual List<TimesheetEntry> TimesheetEntries { get; set; }
        public virtual List<TimesheetEntry> TimesheetsCreated { get; set; }

        public virtual List<Scorecard> EvaluatorsScorecards { get; set; }
        public virtual List<Scorecard> EmployeesScorecards { get; set; }

        [NotMapped]
        public List<PrivilegeType> AllowedPrivileges { get; set; }

        public bool ProfileComplete { get; set; }

        public static void LoadDefault(DataContext context)
        {
            var accountName = @"RAEZOR_PC\franc";
            var defaultUser =
                context.UserAccountSet.Include(a => a.Roles).SingleOrDefault(a => a.AccountName == accountName);

            if (defaultUser == null)
            {
                defaultUser = new UserAccount
                {
                    Roles = new List<Role>()
                };
                context.UserIdentitySet.Add(defaultUser);
                defaultUser.ProfileComplete = true;
                defaultUser.AccountName = accountName;
                defaultUser.FirstName = "Administrator";
                defaultUser.Surname = "Administrator";
                defaultUser.Active = true;
                defaultUser.IsSystemAdmin = true;
                defaultUser.Registered = DateTime.UtcNow;
                defaultUser.ProfileImageData = new ImageData
                {
                    FileData = ImageData.CreateGenericImage(ResourceManager.DefaultProfileIcon()),
                    FileName = "Test"
                };
                context.SaveChanges();
            }
        }
    }
}