using System.Collections.Generic;

namespace AllGreen.WebServer.Core
{
    public interface IRunnerResources
    {
        IEnumerable<string> GetScriptFiles();
    }
}
