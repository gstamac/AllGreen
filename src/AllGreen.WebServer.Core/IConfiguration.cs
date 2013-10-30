using System.Collections.Generic;
using System;

namespace AllGreen.WebServer.Core
{
    public interface IConfiguration
    {
        string RootFolder { get; }
        IEnumerable<FolderFilter> ServedFolderFilters { get; }
        IEnumerable<FolderFilter> ExcludeServedFolderFilters { get; }
        IEnumerable<FolderFilter> WatchedFolderFilters { get; }
    }
}
