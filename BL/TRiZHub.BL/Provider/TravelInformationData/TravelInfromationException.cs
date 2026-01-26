#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.TravelInformationData
{
    public class TravelInfromationException : Exception
    {
        public TravelInfromationException(string error) : base(error)
        {
        }
    }
}