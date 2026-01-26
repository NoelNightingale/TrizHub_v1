#region Usings

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Security;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models;
using TRiZHub.Models.Account;
using System.Web.Mvc;

#endregion

namespace TRiZHub.Controllers
{
    [NoCache]
    public class TCRRepControllerBase : Controller
    {
        private CurrentUserModel _CurrentUser;
        private bool _DisposeContext = true;
        private ICurrentUser _injectedUser;
        private string _ThreadUserName;
        private bool _UseTokens = true;


        public TCRRepControllerBase()
        {
            Context = new DataContext();
            _DisposeContext = true;
            _UseTokens = true;
        }

        public TCRRepControllerBase(DataContext context, ICurrentUser currentUser)
        {
            Context = context;
            _DisposeContext = false;
            _injectedUser = currentUser;
            _UseTokens = false;
        }

        protected DataContext Context { get; }

        private string ThreadUserName
        {
            get
            {
                if (_ThreadUserName == null)
                {
                    var claimIdentity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
                    if (claimIdentity != null && claimIdentity.Claims.Any())
                    {
                        var userNameClaim = claimIdentity.Claims.SingleOrDefault(a => a.Type == "sub");
                        if (userNameClaim != null)
                            _ThreadUserName = userNameClaim.Value;
                    }
                }
                return _ThreadUserName;
            }
        }

        public CurrentUserModel CurrentUser
        {
            get
            {

                if (_UseTokens == false)
                {
                    if (_injectedUser != null)
                        _CurrentUser = new CurrentUserModel(_injectedUser);
                }
                else
                {
                    if (ThreadUserName != null)
                    {
                        if (_CurrentUser == null || _CurrentUser.UserName != ThreadUserName)
                        {
                            var securityProvider = new SecurityProvider(Context);
                            var aUser = securityProvider.GetCurrentUser(ThreadUserName);
                            _CurrentUser = new CurrentUserModel(aUser);
                        }
                    }
                }
                if (_CurrentUser == null)
                {
                    var localAccount = HttpContext.User.Identity;
                    ISecurityProvider securityProviderBase = new SecurityProvider(Context);
                    var account = securityProviderBase.UserLogin(localAccount.Name);
                    var aUser = securityProviderBase.GetCurrentUser(account.UserName);
                    _CurrentUser = new CurrentUserModel(aUser);
                }
                return _CurrentUser;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_DisposeContext)
                Context.Dispose();
            base.Dispose(disposing);
        }

        protected int SetupGridParams(GridModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Searchfor))
                model.Searchfor = "null";
            if (string.IsNullOrWhiteSpace(model.SortKey))
                model.SortKey = string.Empty;
            if (string.IsNullOrWhiteSpace(model.SortOrder))
                model.SortOrder = "ASC";

            model.SortKey = model.SortKey.ToLower();

            if (model.CurrentPage == null || model.CurrentPage <= 0)
                model.CurrentPage = 1;
            if (model.RecordsPerPage == null || model.RecordsPerPage <= 0)
                model.RecordsPerPage = 60;

            var begin = (model.CurrentPage.Value - 1)*model.RecordsPerPage.Value;
            return begin;
        }

        protected string ImageFileLocation
        {
            get
            {
                if (Url == null)
                    return @"Image/GetJpgImage/";

                var url = Url.Content("~/Image/GetJpgImage");
                if (!url.EndsWith("/"))
                    url = url + "/";
                return url;
            }
        }
    }
}