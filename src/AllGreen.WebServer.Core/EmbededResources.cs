using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AllGreen.WebServer.Core
{
    public class EmbededResources : IWebResources
    {
        private string _WebSiteRoot = (typeof(EmbededResources)).Namespace;
        private Assembly _ExecutingAssembly = Assembly.GetExecutingAssembly();

        public EmbededResources(string webSiteRoot, Assembly executingAssembly)
        {
            _WebSiteRoot = webSiteRoot;
            _ExecutingAssembly = executingAssembly;
        }

        public string GetContent(string path)
        {
            try
            {
                var stream = _ExecutingAssembly.GetManifestResourceStream(GetResourcePath(path));
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
            string[] manifestResourceNames = _ExecutingAssembly.GetManifestResourceNames();
            if (!manifestResourceNames.Contains(resourcePath))
            {
                string extension = Path.GetExtension(resourcePath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(resourcePath);
                Regex regex = new Regex(fileNameWithoutExtension + @"(-\d+\.\d+\.\d+)?(-beta[^\.]*|-rc[^\.]*)?(.min)?" + extension);
                string resourceName = manifestResourceNames.Where(rn => regex.IsMatch(rn)).FirstOrDefault();
                if (!String.IsNullOrEmpty(resourceName))
                    resourcePath = resourceName;
            }
            return resourcePath;
        }

    }
}
