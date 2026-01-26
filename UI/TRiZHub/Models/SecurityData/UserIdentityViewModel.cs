#region Usings

using System;

#endregion

namespace TRiZHub.Models.SecurityData
{
    public class UserIdentityViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool Active { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}