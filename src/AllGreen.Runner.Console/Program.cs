using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllGreen.WebServer.Core;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using TinyIoC;
using System.Reflection;

namespace AllGreen.Runner.Console
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
                System.Console.WriteLine("Server running at " + url);
                RunnerHub runnerHub = tinyIoCContainer.Resolve<RunnerHub>();
                string command = "";
                while (command != "x")
                {
                    command = System.Console.ReadLine();
                    runnerHub.Reload();
                }
            }
        }
    }
}
