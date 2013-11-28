using System.Collections.Generic;
using System;

namespace AllGreen.WebServer.Core
{
    public interface IConfiguration
    {
        string ServerUrl { get; }
        string RootFolder { get; }
        List<FolderFilter> ServedFolderFilters { get; }
        List<FolderFilter> ExcludeServedFolderFilters { get; }
        List<FolderFilter> WatchedFolderFilters { get; }
    }

}
