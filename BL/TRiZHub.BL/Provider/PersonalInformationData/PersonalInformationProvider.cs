#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.PersonalInformationData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.PersonalInformationData
{
    public class PersonalInformationProvider : TRiZHubProvider, IPersonalInformationProvider
    {
        #region Constructor

        public PersonalInformationProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        public PersonalInformation SavePersonalInformation(Guid? id, Guid userAccountId, string fullNames,
            string surname, string title, string idNumber, string spouseName, string children, DateTime dob, string company,DateTime workExperienceStartDate,
            DateTime empStartDate, DateTime? empEndDate, string race, string gender, string doorTag, string phoneExt,
            string cellphone, string landLinePhone, string companyEmail,
            string otherEmail, String accessLevel, string medicalScheme, string medicalSchemeOption, string medicalAidNumber )
        {
           Authenticate(PrivilegeType.UserPersonalInformationMaintenance);

            if (id == Guid.Empty)
                id = null;


            if (empStartDate.Date > empEndDate)
            {
                throw new PersonalInformationException("Employment Start date is before Employment End date");
            }

            if (workExperienceStartDate.Date > empStartDate.Date || workExperienceStartDate.Date > empEndDate)
            {
                throw new PersonalInformationException("Work Experience Start Date is before Employment Start and, or Employment End Date!");
            }

            var personalInfoRecords = DataContext.PersonalInformationSet.Where(u => u.UserAccountId == userAccountId).ToList();
            if (personalInfoRecords.Count > 1)
            {
                DataContext.PersonalInformationSet.Remove(personalInfoRecords[0]);
                DataContextSaveChanges();
            }

            var record = DataContext.PersonalInformationSet.FirstOrDefault(u => u.UserAccountId == userAccountId);
            if (record == null)
            {
                record = new PersonalInformation
                {
                    UserAccountId = userAccountId
                };

                DataContext.PersonalInformationSet.Add(record);
            }

            record.FullNames = fullNames;
            record.Surname = surname;
            record.Title = title;
            record.IdNumber = idNumber;
            record.Dob = dob;
            record.SpouseName = spouseName;
            record.Children = children;
            record.Company = company;
            record.WorkExperienceStartDate = workExperienceStartDate;
            record.EmploymentStartDate = empStartDate;
            record.EmploymentEndDate = empEndDate;
            record.Race = race;
            record.Gender = gender;
            record.DoorTagNumber = doorTag;
            record.PhoneExtension = phoneExt;
            record.CellPhone = cellphone;
            record.LandLinePhone = landLinePhone;
            record.CompanyEmail = companyEmail;
            record.OtherEmail = otherEmail;
            record.AccessLevel = accessLevel;
            record.MedicalScheme = medicalScheme;
            record.MedicalSchemeOption = medicalSchemeOption;
            record.MedicalAidNumber = medicalAidNumber;

            DataContextSaveChanges();

            return record;
        }

        public PersonalInformation GetPersonalInformation(Guid id)
        {
            Authenticate(PrivilegeType.UserPersonalInformationMaintenance);
            return DataContext.PersonalInformationSet.FirstOrDefault(a => a.UserAccountId == id);
        }
    }
}