#region Usings

using System;

#endregion

namespace TRiZHub.Models
{
    public class UserAccountIdModel : GridModel
    {
        public Guid UserAccountId { get; set; }

        public bool ShowInactive { get; set; }

        public bool ShowRates { get; set; }
    }
}