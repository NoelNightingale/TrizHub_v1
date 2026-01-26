#region Usings

using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace TRiZHub
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Admin", "admin/{id}",
                new {controller = "Home", action = "Admin", id = UrlParameter.Optional});

            routes.MapRoute("Subscriber", "subscriber/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional});

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}