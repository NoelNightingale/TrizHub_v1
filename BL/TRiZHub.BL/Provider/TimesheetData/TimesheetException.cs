#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.TimesheetData
{
    public class TimesheetException : Exception
    {
        public TimesheetException(string error) : base(error)
        {
        }
    }
}