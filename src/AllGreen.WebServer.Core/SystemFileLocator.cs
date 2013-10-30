using System.Collections.Generic;
using System.IO;

namespace AllGreen.WebServer.Core
{
    //ncrunch: no coverage start
    public class SystemFileLocator : IFileLocator
    {
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
    //ncrunch: no coverage end
}
