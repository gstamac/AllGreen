using System.Collections.Generic;
using System.IO;

namespace AllGreen.Core
{
    public interface IFileSystem
    {
        string ReadAllText(string path);
        bool FileExists(string path);

        bool GetFiles(string path, string searchPattern, bool includeSubfolders, out string[] files);
        bool GetFolders(string path, out string[] folders);
    }
}