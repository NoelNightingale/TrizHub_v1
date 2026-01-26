#region Usings

using System;

#endregion

namespace TRiZHub.Models.SecurityData
{
    public class UserGridModel
    {
        public Guid? Id { get; set; }
        public string Account { get; set; }
        public DateTime? LockedOut { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Employer { get; set; }
        public bool IsAdmin { get; set; }
        public bool Active { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime EmployemntStartDate { get; set;}
        public bool specificProjectTimeCapture { get; set; }
    }
}