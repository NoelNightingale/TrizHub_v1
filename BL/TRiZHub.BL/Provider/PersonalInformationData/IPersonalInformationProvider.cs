#region Usings

using System;
using TRiZHub.BL.Entities.PersonalInformationData;

#endregion

namespace TRiZHub.BL.Provider.PersonalInformationData
{
    public interface IPersonalInformationProvider : ITRiZHubProvider
    {
        PersonalInformation SavePersonalInformation(Guid? id, Guid userAccountId, string fullNames,
            string surname, string title, string idNumber, string spouseName, string children, DateTime dob, string company,
            DateTime workExperienceStartDate,
            DateTime empStartDate, DateTime? empEndDate, string race, string gender, string doorTag, string phoneExt,
            string cellphone, string landLinePhone, string companyEmail,
            string otherEmail, String accessLevel, string medicalScheme, string medicalSchemeOption, string medicalAidNumber);

        PersonalInformation GetPersonalInformation(Guid id);
    }
}