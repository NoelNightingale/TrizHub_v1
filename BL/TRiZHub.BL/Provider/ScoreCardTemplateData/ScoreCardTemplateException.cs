#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ScorecardTemplateData
{
    public class ScorecardTemplateException : Exception
    {
        public ScorecardTemplateException(string error) : base(error)
        {
        }
    }
}