using System;
using System.Collections.Generic;
using System.IO;

namespace AllGreen.WebServer.Core
{
    public interface IFolderWatcher : IDisposable
    {
        event FileSystemEventHandler Changed;
    }
}
