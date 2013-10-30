using System.Collections.Generic;

namespace AllGreen.WebServer.Core
{
    public interface IScriptList
    {
        IEnumerable<string> Files { get; }
    }
}