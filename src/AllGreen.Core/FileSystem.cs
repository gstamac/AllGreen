using System.Collections.Generic;
using System.IO;

namespace AllGreen.Core
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
    public class FileSystem : IFileSystem
    {
        public string ReadAllText(string path)
        {
            if (File.Exists(path))
                return File.ReadAllText(path);
            return null;
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool GetFiles(string path, string searchPattern, bool includeSubfolders, out string[] files)
        {
            files = null;
            try
            {
                files = Directory.GetFiles(path, searchPattern, includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            catch { }
            return (files != null);
        }

        public bool GetFolders(string path, out string[] folders)
        {
            folders = null;
            try
            {
                folders = Directory.GetDirectories(path);
            }
            catch { }
            return folders != null;
        }
    }
}
