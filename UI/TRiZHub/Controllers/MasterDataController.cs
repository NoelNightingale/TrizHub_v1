#region Usings

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Email;
using TRiZHub.BL.Provider.ImageData;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models.AdminModels;
using TRiZHub.Models.AdminModels.settings;

#endregion

namespace TRiZHub.Controllers
{
    [Authorize]
    [NoCache]
    public class MasterDataController : TCRControllerBase
    {
        #region Ctor

        public MasterDataController()
        {
            AppSettings = new AppSettings(Context);
            ImageDataProvider = new ImageDataProvider(Context, CurrentUser);
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            SettingsProvider = new SettingsProvider(Context, CurrentUser);
            EmailProvider = new EmailProvider(Context, AppSettings);
        }

        public MasterDataController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = settings;
            ImageDataProvider = new ImageDataProvider(Context, CurrentUser);
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            SettingsProvider = new SettingsProvider(Context, CurrentUser);
            EmailProvider = new EmailProvider(Context, AppSettings);
        }

        private IAppSettings AppSettings { get; }
        private IImageDataProvider ImageDataProvider { get; }
        private ISecurityProvider SecurityProvider { get; }
        private ISettingsProvider SettingsProvider { get; }
        private IEmailProvider EmailProvider { get; }

        #endregion

        #region Settings

        //[HttpGet]
        //[AllowAnonymous]
        //public SettingsModel Settings()
        //{
        //    return new SettingsModel
        //    {
        //        EmailFromName = AppSettings.EmailFromName,
        //        AboutApp = AppSettings.AboutApp,
        //        EmailFromAddress = AppSettings.EmailFromAddress
        //    };
        //}

        #endregion

        #region Settings

        //[HttpGet]
        //public SettingsModel SettingsGet()
        //{
        //    return new SettingsModel
        //    {
        //        EmailFromName = AppSettings.EmailFromName,
        //        AboutApp = AppSettings.AboutApp,
        //        EmailFromAddress = AppSettings.EmailFromAddress
        //    };
        //}

        //[HttpPost]
        //public SettingsModel SettingsSave(SettingsModel model)
        //{
        //    CheckModelState();
        //    try
        //    {
        //        var result = SettingsProvider.SettingsSave(model.EmailFromName, model.EmailFromAddress, model.AboutApp);
        //        return new SettingsModel
        //        {
        //            EmailFromName = result.EmailFromName,
        //            AboutApp = result.AboutApp,
        //            EmailFromAddress = result.EmailFromAddress
        //        };
        //    }
        //    catch (SecurityException e)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
        //    }
        //    catch (SettingsException ce)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ce.Message));
        //    }
        //}

        #endregion

        #region Profile

        /// <summary>
        /// Save profile information 
        /// </summary>
        [HttpPost]
        public ProfileViewModel ProfileSave(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).First();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
            }

            try
            {
                byte[] imageData = null;
                string imageName = null;
                if (model.ProfileImage != null && !string.IsNullOrWhiteSpace(model.ProfileImage.FileName))
                {
                    imageData = model.ProfileImage.FileData;
                    imageName = model.ProfileImage.FileName;
                }

                var item = SecurityProvider.EditProfile(model.FirstName, model.Surname, model.EmailAddress, imageName,
                    imageData);

                model.ProfileImageId = item.ProfileImageDataId;
            }
            catch (SecurityException e)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    e.Message));
            }
            return model;
        }

        /// <summary>
        /// Retrieve Profile information based on current logged in windows user
        /// </summary>
        [HttpGet]
        public ProfileViewModel ProfileGet()
        {
            var profile = SecurityProvider.GetMyProfile();
            var model = new ProfileViewModel
            {
                EmailAddress = profile.AccountName,
                FirstName = profile.FirstName,
                Surname = profile.Surname,
                ProfileImageId = profile.ProfileImageDataId,
                ProfileImageLocation = ImageFileLocation + profile.ProfileImageDataId
            };

            return model;
        }

        #endregion
    }
}