#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ScorecardData
{
    public class ScorecardException : Exception
    {
        public ScorecardException(string error) : base(error)
        {
        }
    }
}