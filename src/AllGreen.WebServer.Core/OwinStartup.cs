using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;
using TinyIoC;

namespace AllGreen.WebServer.Core
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            TinyIoCContainer tinyIoCContainer = new TinyIoCContainer();
            tinyIoCContainer.Register<IWebResources>(new EmbededResources(@"AllGreen.WebServer.Resources", Assembly.Load("AllGreen.WebServer.Resources")));
            tinyIoCContainer.Register<IRunnerResources, RunnerResources>();
            //tinyIoCContainer.Register<ClientController>();

            var config = new HttpConfiguration() { DependencyResolver = new ControllerDependencyResolver(tinyIoCContainer) };

            app.MapHubs();
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