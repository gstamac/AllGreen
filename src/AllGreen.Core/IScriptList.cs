using System.Collections.Generic;

namespace AllGreen.Core
{
    public interface IScriptList
    {
        IEnumerable<string> Scripts { get; }
    }
}