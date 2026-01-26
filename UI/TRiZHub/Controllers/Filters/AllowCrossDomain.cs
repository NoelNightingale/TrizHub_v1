#region Usings

using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

#endregion

namespace TRiZHub.Controllers.Filters
{
    [ExcludeFromCodeCoverage]
    public class AllowCrossDomain : ActionFilterAttribute
    {
        public const string AllDomains = "*";
        private readonly string[] _allowMethods;
        private readonly string _allowOrigin;

        public AllowCrossDomain()
            : this(null, null)
        {
        }

        public AllowCrossDomain(string allowOrigin, params string[] allowMethods)
        {
            _allowMethods = allowMethods;
            _allowOrigin = allowOrigin;

            if (string.IsNullOrWhiteSpace(_allowOrigin))
            {
                _allowOrigin = AllDomains;
            }

            if (_allowMethods == null || _allowMethods.Length == 0)
            {
                _allowMethods = new[] {"GET"};
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", _allowOrigin);

            filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods",
                string.Join(", ", _allowMethods));

            filterContext.HttpContext.Response.Headers.Add("Access-Control-Max-Age", "86400");
        }
    }
}