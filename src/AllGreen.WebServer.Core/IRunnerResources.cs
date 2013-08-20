using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllGreen.WebServer.Core
{
    public interface IRunnerResources
    {
        IEnumerable<string> GetScriptFiles();
    }
}
