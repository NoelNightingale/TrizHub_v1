#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.BillingRatesData
{
    public class BillingRatesException : Exception
    {
        public BillingRatesException(string error) : base(error)
        {
        }
    }
}