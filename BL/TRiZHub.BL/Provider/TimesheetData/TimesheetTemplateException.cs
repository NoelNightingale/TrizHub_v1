#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.TimesheetData
{
    public class TimesheetTemplateException : Exception
    {
        public TimesheetTemplateException(string error) : base(error)
        {
        }
    }
}
