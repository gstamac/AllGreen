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
    public class ConsoleReporter : IReporter
    {
        public void SpecUpdated(Spec spec)
        {
            Console.WriteLine("Spec updated");
            OutputSpec(spec);
        }

        private void OutputSpec(Spec spec)
        {
            Console.WriteLine(String.Format("ID: {0}, Name: {1}, Status: {2}", spec.Id, spec.Name, spec.Status));
            foreach (SpecStep step in spec.Steps)
            {
                OutputStep(step);
            }
            if (spec.Suite != null)
                OutputSuite("\tGROUP", spec.Suite);
        }

        public void OutputSuite(string prefix, Suite suite)
        {
            Console.WriteLine(String.Format("{0} ID: {1}, Name: {2}, Status: {3}", prefix, suite.Id, suite.Name, suite.Status));
            if (suite.ParentSuite != null)
                OutputSuite("\t\tPARENT GROUP", suite.ParentSuite);
        }

        public void OutputStep(SpecStep specStep)
        {
            Console.WriteLine(String.Format("\tMessage: {0}, Status: {1}, Trace: {2}", specStep.Message, specStep.Status, specStep.Trace));
        }
    }

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
