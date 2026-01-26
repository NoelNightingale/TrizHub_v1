#region Usings

using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using TRiZHub.SPA;

#endregion

//using Microsoft.Owin.Host.SystemWeb;

[assembly: OwinStartup(typeof(Startup))]

namespace TRiZHub.SPA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            ConfigureAuth(app);
            WebApiConfig.Register(config);

            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
            app.MapSignalR();
        }
    }
}