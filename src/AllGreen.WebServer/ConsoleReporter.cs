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
        public void Register(Guid connectionId, string userAgent)
        {
            Console.WriteLine(String.Format("[{0}] Client connected with UserAgent: {1}", connectionId, userAgent));
        }

        public void Reset(Guid connectionId)
        {
            Console.WriteLine(String.Format("[{0}] Runner was reset", connectionId));
        }

        public void Started(Guid connectionId, int totalTests)
        {
            Console.WriteLine(String.Format("[{0}] Runner started running {1} tests", connectionId, totalTests));
        }

        public void SpecUpdated(Guid connectionId, Spec spec)
        {
            Console.WriteLine(String.Format("[{0}] Spec {1} updated => {2}", connectionId, spec.Id.ToString().Substring(20), spec.Status));
            OutputSpec(spec);
        }

        public void Finished(Guid connectionId)
        {
            Console.WriteLine(String.Format("[{0}] Runner finished", connectionId));
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
}
