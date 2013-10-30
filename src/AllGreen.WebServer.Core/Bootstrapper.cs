using System.Reflection;
using Microsoft.AspNet.SignalR;
using System;

namespace AllGreen.WebServer.Core
{
    public static class Bootstrapper
    {
        public static void RegisterServices(TinyIoC.TinyIoCContainer tinyIoCContainer)
        {
            tinyIoCContainer.Register<IWebResources>(new EmbededResources(@"AllGreen.WebServer.Resources", Assembly.Load("AllGreen.WebServer.Resources")));
            IConfiguration configuration = tinyIoCContainer.Resolve<IConfiguration>();
            tinyIoCContainer.Register<IRunnerResources>(new RunnerResources(new DynamicScriptList(configuration.RootFolder, configuration.ServedFolderFilters, new SystemFileLocator())));
            tinyIoCContainer.Register<IHubContext>((ioc, np) => GlobalHost.ConnectionManager.GetHubContext<RunnerHub>());
            tinyIoCContainer.Register<IRunnerHub, RunnerHub>();
        }
    }
}
