#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.BillingCycleData
{
    public class BillingCyleException : Exception
    {
        public BillingCyleException(string error) : base(error)
        {
        }
    }
}