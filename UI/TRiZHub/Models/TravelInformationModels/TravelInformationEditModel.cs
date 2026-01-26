#region Usings

using System;

#endregion

namespace TRiZHub.Models.TravelInformationModels
{
    public class TravelInformationEditModel
    {
        public Guid? Id { get; set; }

        public Guid UserAccountId { get; set; }

        public string DocumentType { get; set; }

        public virtual string Number { get; set; }

        public virtual DateTime ExpiryDate { get; set; }
    }
}