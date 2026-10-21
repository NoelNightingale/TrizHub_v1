#region Usings

using System;
using System.Net;
using System.Security;
using System.Web;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Security;
using TRiZHub.Models.Account;

#endregion

namespace TRiZHub.Controllers.Auth
{
    /// <summary>
    /// Local Debug-only stand-in for the signed-in Windows account.
    /// Release builds compile the cookie read out, and a Debug build ignores it
    /// unless the request is local.
    /// </summary>
    internal static class LocalImpersonation
    {
        public const string CookieName = "TrizHub.Impersonate";

        public static bool IsAvailable(Uri requestUri = null)
        {
#if DEBUG
            var context = HttpContext.Current;
            if (context != null && context.Request != null)
            {
                if (context.Request.IsLocal)
                    return true;
                // IIS Express often reports localhost as ::ffff:127.0.0.1, which IsLocal rejects.
                if (IsLoopback(context.Request.UserHostAddress))
                    return true;
                if (context.Request.Url != null && IsLoopback(context.Request.Url.Host))
                    return true;
            }

            return requestUri != null && IsLoopback(requestUri.Host);
#else
            return false;
#endif
        }

        public static CurrentUserModel TryImpersonate(DataContext context)
        {
#if DEBUG
            if (!IsAvailable())
                return null;

            var accountName = ReadAccountName();
            if (string.IsNullOrWhiteSpace(accountName))
                return null;

            try
            {
                var user = new SecurityProvider(context).GetCurrentUser(accountName);
                var model = new CurrentUserModel(user);
                model.Impersonating = true;
                return model;
            }
            catch (SecurityException)
            {
                return null;
            }
#else
            return null;
#endif
        }

#if DEBUG
        public static void SetCookie(string accountName)
        {
            var cookie = new HttpCookie(CookieName, HttpUtility.UrlEncode(accountName))
            {
                HttpOnly = true,
                Path = CookiePath()
            };
            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        public static void ClearCookie()
        {
            var cookie = new HttpCookie(CookieName, "")
            {
                HttpOnly = true,
                Path = CookiePath(),
                Expires = DateTime.UtcNow.AddDays(-1)
            };
            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        private static string ReadAccountName()
        {
            var cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
                return null;

            var decoded = HttpUtility.UrlDecode(cookie.Value);
            return string.IsNullOrWhiteSpace(decoded) ? null : decoded.Trim();
        }

        private static string CookiePath()
        {
            var path = HttpContext.Current.Request.ApplicationPath;
            return string.IsNullOrEmpty(path) ? "/" : path;
        }

        private static bool IsLoopback(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var host = value.Trim();
            if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
                return true;

            if (host.StartsWith("[") && host.EndsWith("]"))
                host = host.Substring(1, host.Length - 2);

            IPAddress address;
            if (!IPAddress.TryParse(host, out address))
                return false;

            if (IPAddress.IsLoopback(address))
                return true;

            if (address.IsIPv4MappedToIPv6)
                return IPAddress.IsLoopback(address.MapToIPv4());

            return false;
        }
#endif
    }
}
