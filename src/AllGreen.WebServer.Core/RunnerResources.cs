using System.Collections.Generic;

namespace AllGreen.WebServer.Core
{
    public class RunnerResources : IRunnerResources
    {
        IScriptList _FileList;

        public RunnerResources(IScriptList fileList)
        {
            _FileList = fileList;
        }

        public IEnumerable<string> GetScriptFiles()
        {
            return _FileList.Files;
        }
    }
}