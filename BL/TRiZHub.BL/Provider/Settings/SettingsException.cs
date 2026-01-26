#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.Settings
{
    public class SettingsException : Exception
    {
        public SettingsException(string error)
            : base(error)
        {
        }
    }
}