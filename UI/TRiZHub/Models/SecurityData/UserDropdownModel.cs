#region Usings

using System;

#endregion

namespace TRiZHub.Models.SecurityData
{
    public class UserDropdownModel
    {
        public Guid Id { get; set; }

        public string Description
        {
            get { return string.Format("{0} {1}", Firstname, Surname); }
        }

        public string Firstname { get; set; }

        public string Surname { get; set; }

        public string AccountName { get; set; }
    }
}