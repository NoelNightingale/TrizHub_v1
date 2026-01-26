#region Usings

using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Email;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models.Account;
using System.Security.Principal;

#endregion

namespace TRiZHub.Controllers.Security
{
    [Authorize]
    [NoCache]
    public class AccountController : TCRControllerBase
    {
        #region Ctor

        private ISecurityProvider SecurityProvider { get; }
        private IEmailProvider EmailProvider { get; }
        private IAppSettings AppSettings { get; }

        public AccountController()
        {
            AppSettings = new AppSettings(Context);
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            EmailProvider = new EmailProvider(Context, AppSettings);
        }

        public AccountController(IAppSettings settings, DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            AppSettings = new AppSettings(Context);
            SecurityProvider = new SecurityProvider(Context, CurrentUser);
            EmailProvider = new EmailProvider(Context, AppSettings);
        }

        #endregion

        #region Account
        /// <summary>
        /// Get full user information bases on current windows logged in user
        /// </summary>
        public CurrentUserModel GetCurrentUser()
        {
            if (CurrentUser == null)
            {
                try
                {
                    var localAccount = HttpContext.Current.User.Identity;
                    var account = SecurityProvider.UserLogin(localAccount.Name);
                    return new CurrentUserModel(account);
                }
                catch (Exception)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "Your connecting account is not valid..."));
                }
            }
            return CurrentUser;
        }

        /// <summary>
        /// Retrieve profile information based on windows logged in user 
        /// </summary>
        [HttpGet]
        public ProfileViewModel GetMyProfile()
        {
            var profile = SecurityProvider.GetMyProfile();
            var model = new ProfileViewModel
            {
                EmailAddress = profile.AccountName,
                FirstName = profile.FirstName,
                Surname = profile.Surname
            };

            return model;
        }

        #endregion
    }
}