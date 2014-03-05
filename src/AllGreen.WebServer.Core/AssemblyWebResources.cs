using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AllGreen.WebServer.Core
{
    public class AssemblyWebResources : IWebResources
    {
        private readonly _Assembly _ResourcesAssembly;
        private readonly string _WebSiteRoot;

        public AssemblyWebResources(_Assembly resourcesAssembly)
        {
            _ResourcesAssembly = resourcesAssembly;
            _WebSiteRoot = _ResourcesAssembly.GetName().Name;
        }

        public string GetContent(string path)
        {
            try
            {
                var stream = _ResourcesAssembly.GetManifestResourceStream(GetResourcePath(path));
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch { }
            return null;
        }

        private string GetResourcePath(string path)
        {
            return String.Format("{0}.{1}", _WebSiteRoot, path.Replace('/', '.')).Replace("..", ".");
        }


        public string GetSystemFilePath(string path)
        {
            return null;
        }
    }
}