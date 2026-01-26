#region Usings

using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Security;
using System.Security.Principal;

#endregion Usings

namespace TRiZHub.Controllers.Auth
{
    public class TCRAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
#pragma warning disable 1998

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
#pragma warning restore 1998
        {
            context.Validated();
        }

#pragma warning disable 1998

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
#pragma warning restore 1998
        {
            //allow cross origins
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            //if (HttpContext.Current.User == null)
            if (HttpContext.Current.User == null)
            {
                context.SetError("invalid_grant", "Account used is not valid...");
                return;
            }

            var localAccount = HttpContext.Current.User.Identity;

            DataContext.Setup();
            using (var db = new DataContext())
            {
                ISecurityProvider securityProviderBase = new SecurityProvider(db);
                try
                {
//                    var account = securityProviderBase.UserLogin(@"GVWHOLDINGS\adsmit");
                    var account = securityProviderBase.UserLogin(localAccount.Name);

                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("sub", account.UserName));
                    identity.AddClaim(new Claim("role", "user"));
                    context.Validated(identity);
                }
                catch (SecurityException e)
                {
                    context.SetError("invalid_grant", e.Message);
                }
            }
        }
    }
}