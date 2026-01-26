#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ContactData
{
    public class ContactException : Exception
    {
        public ContactException(string error) : base(error)
        {
        }
    }
}