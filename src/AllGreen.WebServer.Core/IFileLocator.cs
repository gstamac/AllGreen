using System.Collections.Generic;
using System.IO;

namespace AllGreen.WebServer.Core
{
    public interface IFileLocator
    {
        bool GetFiles(string path, string searchPattern, bool includeSubfolders, out string[] files);
        bool GetFolders(string path, out string[] folders);
    }
}