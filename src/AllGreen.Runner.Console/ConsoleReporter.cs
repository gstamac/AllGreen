using System;
using AllGreen.Core;

namespace AllGreen.Runner.Console
{
    public class ConsoleReporter : IReporter
    {
        public void Connected(string connectionId, string userAgent)
        {
            System.Console.WriteLine(String.Format("[{0}] Client CONNECTED with UserAgent: {1}", connectionId, userAgent));
        }

        public void Reconnected(string connectionId, string userAgent)
        {
            System.Console.WriteLine(String.Format("[{0}] Client RECONNECTED with UserAgent: {1}", connectionId, userAgent));
        }

        public void Disconnected(string connectionId)
        {
            System.Console.WriteLine(String.Format("[{0}] Client DISCONNECTED", connectionId));
        }

        public void Register(string connectionId, string userAgent)
        {
            System.Console.WriteLine(String.Format("[{0}] Client registered with UserAgent: {1}", connectionId, userAgent));
        }

        public void Reset(string connectionId)
        {
            System.Console.WriteLine(String.Format("[{0}] Runner was reset", connectionId));
        }

        public void Started(string connectionId, int totalTests)
        {
            System.Console.WriteLine(String.Format("[{0}] Runner started running {1} tests", connectionId, totalTests));
        }

        public void SpecUpdated(string connectionId, Spec spec)
        {
            System.Console.WriteLine(String.Format("[{0}] Spec {1} updated at {2} => {3}", connectionId, spec.Id.ToString().Substring(20), spec.Time, spec.Status));
            OutputSpec(spec);
        }

        public void Finished(string connectionId)
        {
            System.Console.WriteLine(String.Format("[{0}] Runner finished", connectionId));
        }

        private void OutputSpec(Spec spec)
        {
            System.Console.WriteLine(String.Format("ID: {0}, Name: {1}, Status: {2}", spec.Id, spec.Name, spec.Status));
            foreach (SpecStep step in spec.Steps)
            {
                OutputStep(step);
            }
            if (spec.Suite != null)
                OutputSuite("\tGROUP", spec.Suite);
        }

        public void OutputSuite(string prefix, Suite suite)
        {
            System.Console.WriteLine(String.Format("{0} ID: {1}, Name: {2}, Status: {3}", prefix, suite.Id, suite.Name, suite.Status));
            if (suite.ParentSuite != null)
                OutputSuite("\t\tPARENT GROUP", suite.ParentSuite);
        }

        public void OutputStep(SpecStep specStep)
        {
            System.Console.WriteLine(String.Format("\tMessage: {0}, Status: {1}, Trace: {2}", specStep.Message, specStep.Status, specStep.Trace));
        }

    }
}
