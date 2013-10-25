using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllGreen.WebServer.Core
{
    public class RunnerResources : IRunnerResources
    {
        public IEnumerable<string> GetScriptFiles()
        {
            return new string[] { "Scripts/jasmine.js", "Client/ReporterAdapters/jasmineAdapter.js", "Client/testScript.js" };
        }
    }
}