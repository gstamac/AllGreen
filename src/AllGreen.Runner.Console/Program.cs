using System;
using System.IO;
using System.Reflection;
using AllGreen.WebServer.Core;
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
            webResources.Add(new WebServerResources(new DynamicScriptList(configuration.RootFolder, configuration.ServedFolderFilters, configuration.ExcludeServedFolderFilters, new SystemFileLocator())));
            webResources.Add(new FileSystemResources(configuration.RootFolder, new FileSystemReader()));
            resourceResolver.Register<IWebResources>(webResources);
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
