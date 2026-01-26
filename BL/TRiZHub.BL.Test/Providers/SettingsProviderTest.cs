#region Usings

using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TRiZHub.BL.Provider.Settings;

#endregion

namespace TRiZHub.BL.Test.Providers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SettingsProviderTest : ProviderTestBase
    {
        [TestMethod]
        [TestCategory("Provider.Settings")]
        public void SettingsChanges()
        {
            IAppSettings settingsProvider = new AppSettings(Context);
            var admin = SeedData.CreateAdmin(Context);
            ISettingsProvider provider = new SettingsProvider(Context, admin);

            var emailFromName = settingsProvider.EmailFromName;
            var emailFromAddress = settingsProvider.EmailFromAddress;

            emailFromName.ShouldEqual("noreply");
            emailFromAddress.ShouldEqual("noreply2@s7on.co.za");

           // provider.EmailFromName("test.com");
           // provider.EmailFromAddress("testEmail@test.com");

            var emailFromNameNew = settingsProvider.EmailFromName;
            var emailFromAddressNew = settingsProvider.EmailFromAddress;

            emailFromNameNew.ShouldEqual("test.com");
            emailFromAddressNew.ShouldEqual("testEmail@test.com");

           // provider.SettingsSave("1", "2", "3");

            var a = settingsProvider.EmailFromName;
            var b = settingsProvider.EmailFromAddress;
            var c = settingsProvider.AboutApp;

            a.ShouldEqual("1");
            b.ShouldEqual("2");
            c.ShouldEqual("3");
        }
    }
}