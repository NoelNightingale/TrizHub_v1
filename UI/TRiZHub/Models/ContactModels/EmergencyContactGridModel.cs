#region Usings

using System;

#endregion

namespace TRiZHub.Models.ContactModels
{
    public class EmergencyContactGridModel
    {
        public Guid? Id { get; set; }

        public Guid UserAccountId { get; set; }

        public string Account { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Name { get; set; }

        public string Relationship { get; set; }

        public string CellphoneNumber { get; set; }

        public string LandLineNumber { get; set; }

       
    }
}