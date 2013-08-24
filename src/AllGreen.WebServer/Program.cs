using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AllGreen.WebServer.Core;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using TinyIoC;

namespace AllGreen.WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8080";

            TinyIoCContainer tinyIoCContainer = new TinyIoCContainer();
            tinyIoCContainer.Register<IWebResources>(new EmbededResources(@"AllGreen.WebServer.Resources", Assembly.Load("AllGreen.WebServer.Resources")));
            tinyIoCContainer.Register<IRunnerResources, RunnerResources>();
            tinyIoCContainer.Register<IHubContext>((ioc, np) => GlobalHost.ConnectionManager.GetHubContext<RunnerHub>());
            tinyIoCContainer.Register<IReporter, ConsoleReporter>();

            using (WebApp.Start(url, appBuilder => new OwinStartup(tinyIoCContainer).Configuration(appBuilder)))
            {
                Console.WriteLine("Server running at " + url);
                RunnerHub runnerHub = new RunnerHub(GlobalHost.ConnectionManager.GetHubContext<RunnerHub>(), tinyIoCContainer.Resolve<IReporter>());
                string command = "";
                while (command != "x")
                {
                    command = Console.ReadLine();
                    runnerHub.Reload();
                }
            }
        }
    }
}
