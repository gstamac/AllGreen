using AllGreen.WebServer.Core;
using Microsoft.Owin.Hosting;
using TinyIoC;

namespace AllGreen.Runner.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8080";

            TinyIoCContainer ResourceResolver = new TinyIoCContainer();
            ResourceResolver.Register<IReporter, ConsoleReporter>();
            Bootstrapper.RegisterServices(ResourceResolver);

            using (WebApp.Start(url, appBuilder => new OwinStartup(ResourceResolver).Configuration(appBuilder)))
            {
                System.Console.WriteLine("Server running at " + url);
                IRunnerHub runnerHub = ResourceResolver.Resolve<IRunnerHub>();
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
