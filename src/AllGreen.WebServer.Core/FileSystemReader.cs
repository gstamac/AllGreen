using System.Collections.Generic;
using System.IO;

namespace AllGreen.WebServer.Core
{
    public class FileSystemReader : IFileReader
    {
        public string ReadAllText(string path)
        {
            if (File.Exists(path))
                return File.ReadAllText(path);
            return null;
        }
    }
}
