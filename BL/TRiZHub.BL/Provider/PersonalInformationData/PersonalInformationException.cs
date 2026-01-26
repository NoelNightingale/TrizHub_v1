#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.PersonalInformationData
{
    public class PersonalInformationException : Exception
    {
        public PersonalInformationException(string error) : base(error)
        {
        }
    }
}