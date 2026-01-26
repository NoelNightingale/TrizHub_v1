#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ReportData
{
    public class ReportException : Exception
    {
        public ReportException(string error) : base(error)
        {
        }
    }
}