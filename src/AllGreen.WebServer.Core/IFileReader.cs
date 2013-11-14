using System.Collections.Generic;
using System.IO;

namespace AllGreen.WebServer.Core
{
    public interface IFileReader
    {
        string ReadAllText(string path);
    }
}