using System;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Owin;
using TinyIoC;

namespace AllGreen.WebServer.Core
{
    public class OwinStartup
    {
        TinyIoCContainer _TinyIoCContainer;

        public OwinStartup(TinyIoCContainer tinyIoCContainer)
        {
            _TinyIoCContainer = tinyIoCContainer;
        }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration() { DependencyResolver = new ControllerDependencyResolver(_TinyIoCContainer) };

            SignalRDependencyResolver resolver = new SignalRDependencyResolver(_TinyIoCContainer);
            GlobalHost.DependencyResolver = resolver;
            HubConfiguration hubConfiguration = new HubConfiguration() { Resolver = resolver };
            app.MapSignalR(hubConfiguration);
            app.UseWebApi(config);
            SetupRoutes(config);
        }

        public static void SetupRoutes(HttpConfiguration config)
        {
            //config.Routes.MapHttpRoute("Runner", "Runner/{*path}", new { controller = "Runner", action = "Get", path = "Client/client.html" });
            //config.Routes.MapHttpRoute("Files", "{*path}", new { controller = "Files", action = "Get", path = "" });
            config.Routes.MapHttpRoute("Web", "{*path}", new { controller = "Web", action = "Get", path = RouteParameter.Optional });
        }
    }
}