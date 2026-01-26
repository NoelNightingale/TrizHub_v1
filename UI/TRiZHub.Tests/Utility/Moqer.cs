using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace TRiZHub.Tests.Utility
{
    [ExcludeFromCodeCoverage]
    public static class Moqer
    {
        public static T CreateController<T>(
                                            object[] constructorParams = null,
                                            string userName = null,
                                            System.Collections.Specialized.NameValueCollection queryString = null
                                            ) where T : Controller
        {

            var httpCtxStub = new Mock<HttpContextBase>();

            var routes = MocRoutes();

            var context = MoqContext(userName, queryString);

            T controller = null;
            if (constructorParams != null)
                controller = System.Activator.CreateInstance(typeof(T), constructorParams) as T;
            else
                controller = System.Activator.CreateInstance<T>() as T;
            controller.Url = new UrlHelper(new RequestContext(context.Object, new RouteData()), routes);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.ControllerContext.HttpContext = httpCtxStub.Object;
            return controller;
        }

        public static T CreateAPIController<T>(object[] constructorParams = null) where T : System.Web.Http.ApiController
        {
            var routes = MocRoutes();
            T controller = null;
            if (constructorParams != null)
                controller = System.Activator.CreateInstance(typeof(T), constructorParams) as T;
            else
                controller = System.Activator.CreateInstance<T>() as T;

            var route = new System.Web.Routing.RouteData();

            System.Net.Http.HttpRequestMessage requestMessage = new System.Net.Http.HttpRequestMessage();
            controller.Url = new System.Web.Http.Routing.UrlHelper(requestMessage);
            controller.ControllerContext = new System.Web.Http.Controllers.HttpControllerContext();
            //if(!String.IsNullOrWhiteSpace(userName))
            //  controller.User = new GenericPrincipal(new GenericIdentity(userName), null);
            return controller;
        }


        private static RouteCollection MocRoutes()
        {
            var routes = new RouteCollection();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
                );
            return routes;
        }

        public static Mock<HttpContextBase> MoqContext(string userName = null,
                                            System.Collections.Specialized.NameValueCollection queryString = null)
        {
            var routes = new RouteCollection();
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
                );

            var context = new Mock<HttpContextBase>(MockBehavior.Loose);
            var request = new Mock<HttpRequestBase>(MockBehavior.Loose);
            var response = new Mock<HttpResponseBase>(MockBehavior.Loose);
            var session = new Mock<HttpSessionStateBase>(MockBehavior.Loose);
            var server = new Mock<HttpServerUtilityBase>(MockBehavior.Loose);

            string rootUrl = "http://" + "www" + ".TestServer.com";

            response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(x => x);
            request.Setup(x => x.ApplicationPath).Returns("/");
            request.Setup(x => x.AppRelativeCurrentExecutionFilePath).Returns("/");
            request.Setup(x => x.Url).Returns(new Uri(rootUrl, UriKind.Absolute));

            if (queryString != null)
                request.SetupGet(x => x.ServerVariables).Returns(queryString);
            else
                request.SetupGet(x => x.ServerVariables).Returns(new NameValueCollection());


            if (userName != null)
                request.Setup(x => x.IsAuthenticated).Returns(true);

            if (queryString != null)
                request.Setup(x => x.QueryString).Returns(queryString);

            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            context.SetupGet(x => x.Server).Returns(server.Object);
            context.SetupGet(x => x.Session).Returns(session.Object);

            if (userName != null)
                context.SetupGet(p => p.User.Identity.Name).Returns(userName);

            context.SetupGet(x => x.User.Identity.IsAuthenticated).Returns(true);

            return context;
        }
    }
}
