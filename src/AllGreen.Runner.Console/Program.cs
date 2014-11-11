using System;
using System.IO;
using System.Reflection;
using AllGreen.Core;
using AllGreen.WebServer.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using TinyIoC;

namespace AllGreen.Runner.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            const string url = "http://localhost:8080";

            XmlConfiguration configuration = XmlConfiguration.LoadFrom(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AllGreen\\AllGreen\\AllGreen.config");

            TinyIoCContainer resourceResolver = new TinyIoCContainer();

            resourceResolver.Register<IConfiguration>(configuration);
            CompositeWebResources webResources = new CompositeWebResources();
            DynamicScriptList scriptList = new DynamicScriptList(configuration.RootFolder, configuration.ServedFolderFilters, configuration.ExcludeServedFolderFilters, new FileSystem());
            webResources.Add(new WebServerResources(scriptList));
            webResources.Add(new FileSystemResources(configuration.RootFolder, scriptList, new FileSystem()));
            resourceResolver.Register<IWebResources>(webResources);
            resourceResolver.Register<IRunnerHub, RunnerHub>();
            resourceResolver.Register<IRunnerBroadcaster>((ioc, npo) => new RunnerBroadcaster(GlobalHost.ConnectionManager.GetHubContext<RunnerHub>().Clients));
            resourceResolver.Register<IReporter, ConsoleReporter>();

            using (WebApp.Start(url, appBuilder => new OwinStartup(resourceResolver).Configuration(appBuilder)))
            {
                System.Console.WriteLine("Server running at " + url);
                string command = "";
                while (command != "x")
                {
                    command = System.Console.ReadLine();
                    resourceResolver.Resolve<IRunnerBroadcaster>().StartAll();
                }
            }
        }
    }
}
