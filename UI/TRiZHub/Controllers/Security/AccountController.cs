#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Email;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.Settings;
using TRiZHub.BL.Provider.WorkTeamData;
using TRiZHub.Controllers.Auth;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models.Account;
using TRiZHub.Models.SecurityData;
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
            CurrentUserModel model;
            if (CurrentUser == null)
            {
                try
                {
                    var localAccount = HttpContext.Current.User.Identity;
                    var account = SecurityProvider.UserLogin(localAccount.Name);
                    model = new CurrentUserModel(account);
                }
                catch (Exception)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "Your connecting account is not valid..."));
                }
            }
            else
            {
                model = CurrentUser;
            }

            model.CanImpersonate = LocalImpersonation.IsAvailable(Request != null ? Request.RequestUri : null);
            model.TeamCapabilities = new WorkTeamAccessProvider(Context, model).CapabilitySummary(model.Id);
            return model;
        }

#if DEBUG
        /// <summary>
        /// Active users for the localhost Debug user picker. Compiled out of Release.
        /// </summary>
        [HttpGet]
        public List<UserDropdownModel> ImpersonationCandidates()
        {
            if (!LocalImpersonation.IsAvailable(Request != null ? Request.RequestUri : null))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return SecurityProvider.GetUserAccountList()
                .Where(a => a.Active && a.FirstName != "Importer")
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.Surname)
                .Select(a => new UserDropdownModel
                {
                    Id = a.Id,
                    Firstname = a.FirstName,
                    Surname = a.Surname,
                    AccountName = a.AccountName
                })
                .ToList();
        }

        /// <summary>
        /// Stores the selected account in a session cookie. An empty name clears it.
        /// </summary>
        [HttpPost]
        public IHttpActionResult Impersonate(ImpersonateModel model)
        {
            if (!LocalImpersonation.IsAvailable(Request != null ? Request.RequestUri : null))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var accountName = model == null ? null : model.AccountName;
            if (string.IsNullOrWhiteSpace(accountName))
            {
                LocalImpersonation.ClearCookie();
                return Ok();
            }

            accountName = accountName.Trim();
            try
            {
                SecurityProvider.GetCurrentUser(accountName);
            }
            catch (SecurityException)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "That account is not an active user."));
            }

            LocalImpersonation.SetCookie(accountName);
            return Ok();
        }
#endif

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