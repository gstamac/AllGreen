using System.Reflection;
using AllGreen.WebServer.Core;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using TinyIoC;

namespace AllGreen.Runner.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8080";

            XmlConfiguration configuration = new XmlConfiguration("");

            TinyIoCContainer resourceResolver = new TinyIoCContainer();

            resourceResolver.Register<IConfiguration>(configuration);
            resourceResolver.Register<IWebResources>(new EmbededResources(@"AllGreen.WebServer.Resources", Assembly.Load("AllGreen.WebServer.Resources")));
            resourceResolver.Register<IRunnerResources>(new RunnerResources(new DynamicScriptList(configuration, new SystemFileLocator())));
            resourceResolver.Register<IHubContext>((ioc, np) => GlobalHost.ConnectionManager.GetHubContext<RunnerHub>());
            resourceResolver.Register<IRunnerHub, RunnerHub>();

            resourceResolver.Register<IReporter, ConsoleReporter>();

            using (WebApp.Start(url, appBuilder => new OwinStartup(resourceResolver).Configuration(appBuilder)))
            {
                System.Console.WriteLine("Server running at " + url);
                IRunnerHub runnerHub = resourceResolver.Resolve<IRunnerHub>();
                string command = "";
                while (command != "x")
                {
                    command = System.Console.ReadLine();
                    runnerHub.ReloadAll();
                }
            }
        }
    }
}
