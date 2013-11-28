using System;
using System.IO;
using System.Linq;

namespace AllGreen.WebServer.Core
{
    public class FileSystemResources : IWebResources
    {
        private string _RootFolder;
        IScriptList _ScriptList;
        IFileSystem _FileSystem;

        public FileSystemResources(string rootFolder, IScriptList scriptList, IFileSystem fileSystem)
        {
            _RootFolder = rootFolder;
            _ScriptList = scriptList;
            _FileSystem = fileSystem;
        }

        public string GetContent(string path)
        {
            string fullPath = GetSystemFilePath(path);
            if (!String.IsNullOrEmpty(fullPath))
            {
                string content = _FileSystem.ReadAllText(fullPath);
                if (content != null) return content.Replace("var AllGreenApp = null;", "");
            }
            return null;
        }

        public string GetSystemFilePath(string path)
        {
            if (path.StartsWith("/")) path = path.Substring(1);

            if (_ScriptList.Scripts.Contains(path))
            {
                string fullPath = Path.Combine(_RootFolder, path.Replace('/', '\\'));
                if (_FileSystem.FileExists(fullPath))
                    return fullPath;
            }
            return null;
        }
    }
}