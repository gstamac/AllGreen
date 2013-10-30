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
            if (_FileList == null)
                return new string[0];
            else
                return _FileList.Files;
        }
    }
}