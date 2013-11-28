using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AllGreen.WebServer.Core
{
    public class WebServerResources : IWebResources
    {
        const string INTERNAL_PATH_PREFIX = "~internal~/";
        private readonly AssemblyWebResources _AssemblyWebResources;
        private readonly String[] _ManifestResourceNames;
        private readonly IScriptList _ServedScriptList;
        
        public WebServerResources(IScriptList servedScriptList)
        {
            _ServedScriptList = servedScriptList;

            Assembly assembly = Assembly.Load("AllGreen.WebServer.Resources");
            int namespaceLength = assembly.GetName().Name.Length;
            _ManifestResourceNames = assembly.GetManifestResourceNames().Select(s => s.Substring(namespaceLength + 1)).ToArray();

            _AssemblyWebResources = new AssemblyWebResources(assembly);
        }

        public string GetContent(string path)
        {
            path = path.Trim('/');
            if (!path.StartsWith(INTERNAL_PATH_PREFIX)) return null;
            path = path.Substring(INTERNAL_PATH_PREFIX.Length);

            string content = _AssemblyWebResources.GetContent(path);
            if (content == null && !_ManifestResourceNames.Contains(path))
            {
                string extension = Path.GetExtension(path);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path.Replace('/', '.').TrimStart('.'));
                Regex regex = new Regex(String.Format(@"^{0}(-\d+\.\d+\.\d+)?(-beta[^\.]*|-rc[^\.]*)?(.min)?{1}$", fileNameWithoutExtension, extension));
                string resourceName = _ManifestResourceNames.Where(rn => regex.IsMatch(rn)).FirstOrDefault();
                if (!String.IsNullOrEmpty(resourceName))
                    content = _AssemblyWebResources.GetContent(resourceName);
            }
            if (path.EndsWith("runner.html") && content != null)
            {
                content = ModifyRunnerHtml(content);
            }
            return content;
        }

        private string ModifyRunnerHtml(string result)
        {
            IEnumerable<string> scriptFiles = _ServedScriptList.Scripts;
            string scripts = String.Join("", scriptFiles.Select(scriptFile => String.Format("<script src=\"/{0}\"></script>", scriptFile)));
            return result.Replace("<!--%SCRIPTS%-->", scripts);
        }

        public string GetSystemFilePath(string path)
        {
            return null;
        }
    }
}
