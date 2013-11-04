using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AllGreen.WebServer.Core
{
    public class EmbededResources : IWebResources
    {
        private readonly Assembly _ResourcesAssembly;
        private readonly string _WebSiteRoot;
        private string[] _ManifestResourceNames;

        public EmbededResources(Assembly resourcesAssembly)
        {
            _ResourcesAssembly = resourcesAssembly;
            _WebSiteRoot = resourcesAssembly.GetName().Name;
            _ManifestResourceNames = _ResourcesAssembly.GetManifestResourceNames();
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
            //ncrunch: no coverage start 
            catch { }
            //ncrunch: no coverage end
            return null;
        }

        private string GetResourcePath(string path)
        {
            string resourcePath = String.Format("{0}.{1}", _WebSiteRoot, path.Replace('/', '.'));
            if (!_ManifestResourceNames.Contains(resourcePath))
            {
                string extension = Path.GetExtension(resourcePath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(resourcePath);
                Regex regex = new Regex(String.Format(@"{0}(-\d+\.\d+\.\d+)?(-beta[^\.]*|-rc[^\.]*)?(.min)?{1}", fileNameWithoutExtension, extension));
                string resourceName = _ManifestResourceNames.Where(rn => regex.IsMatch(rn)).FirstOrDefault();
                if (!String.IsNullOrEmpty(resourceName))
                    resourcePath = resourceName;
            }
            return resourcePath;
        }

    }
}
