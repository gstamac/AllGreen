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
            config.Routes.MapHttpRoute("Scripts", "Scripts/{*path}", new { controller = "Scripts", action = "Get", path = RouteParameter.Optional });
            config.Routes.MapHttpRoute("Client Explicit", "Client/{*path}", new { controller = "Client", action = "Get", path = "client.html" });
            config.Routes.MapHttpRoute("Client", "{*path}", new { controller = "Client", action = "Get", path = "client.html" });
        }
    }
}