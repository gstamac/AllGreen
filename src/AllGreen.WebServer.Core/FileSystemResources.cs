using System;
using System.IO;
using System.Linq;

namespace AllGreen.WebServer.Core
{
    public class FileSystemResources : IWebResources
    {
        private string _RootFolder;
        IFileReader _FileReader;

        public FileSystemResources(string rootFolder, IFileReader fileReader)
        {
            _RootFolder = rootFolder;
            _FileReader = fileReader;
        }

        public string GetContent(string path)
        {
            if (path.StartsWith("Files/"))
            {
                string fullPath = Path.Combine(_RootFolder, path.Substring(6).Replace('/', '\\'));
                return _FileReader.ReadAllText(fullPath);
            }
            return null;
        }
    }
}