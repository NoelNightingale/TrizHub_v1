#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ActivityData
{
    public class ActivityException : Exception
    {
        public ActivityException(string error) : base(error)
        {
        }
    }
}