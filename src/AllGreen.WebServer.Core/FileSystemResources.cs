using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AllGreen.WebServer.Core
{
    public class FileSystemResources : IWebResources
    {
        IConfiguration _Configuration;

        public FileSystemResources(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public string GetContent(string path)
        {
            if (path.StartsWith("Files/"))
            {
                string fullPath = Path.Combine(_Configuration.RootFolder, path.Substring(6).Replace('/', '\\'));
                if (File.Exists(fullPath))
                    return File.ReadAllText(fullPath);
            }
            return null;
        }
    }
}