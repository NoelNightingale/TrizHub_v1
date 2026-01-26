#region Usings

using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.SettingsData;

#endregion

namespace TRiZHub.BL.Provider.Settings
{
    public class AppSettings : IAppSettings
    {
        private readonly SystemParameter _parameter;

        public AppSettings(DataContext context)
        {
            Context = context;
            _parameter = context.SystemParameterSet.Single();
        }

        public AppSettings()
        {
        }

        private DataContext Context { get; }

        public string EmailFromName
        {
            get { return _parameter.EmailFromName; }
        }

        public string EmailFromAddress
        {
            get { return _parameter.EmailFromAddress; }
        }

        public string AboutApp
        {
            get { return _parameter.AboutApp; }
        }
    }
}