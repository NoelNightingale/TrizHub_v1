#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.SettingsData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.Settings
{
    public class SettingsProvider : TRiZHubProvider, ISettingsProvider
    {
        public SettingsProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #region Settings

        //public string EmailFromName(string value)
        //{
        //    Authenticate(PrivilegeType.SettingsMaintenance);
        //    DataContext.SystemParameterSet.Single().EmailFromName = value;
        //    DataContextSaveChanges();
        //    return DataContext.SystemParameterSet.Single().EmailFromName;
        //}

        //public string EmailFromAddress(string value)
        //{
        //    Authenticate(PrivilegeType.SettingsMaintenance);
        //    DataContext.SystemParameterSet.Single().EmailFromAddress = value;
        //    DataContextSaveChanges();
        //    return DataContext.SystemParameterSet.Single().EmailFromAddress;
        //}

        //public string AboutApp(string value)
        //{
        //    Authenticate(PrivilegeType.SettingsMaintenance);
        //    DataContext.SystemParameterSet.Single().AboutApp = value;
        //    DataContextSaveChanges();
        //    return DataContext.SystemParameterSet.Single().AboutApp;
        //}

        //public SystemParameter SettingsSave(string emailFromName, string emailFromAddress, string aboutApp)
        //{
        //    Authenticate(PrivilegeType.SettingsMaintenance);
        //    var model = DataContext.SystemParameterSet.Single();
        //    model.EmailFromName = emailFromName;
        //    model.EmailFromAddress = emailFromAddress;
        //    model.AboutApp = aboutApp;

        //    DataContextSaveChanges();
        //    return model;
        //}

        #endregion
    }
}