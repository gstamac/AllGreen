using System;
using System.Linq;
using System.Windows.Threading;
using AllGreen.Runner.WPF.ViewModels;
using AllGreen.WebServer.Core;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF
{
    public class UrlToPathMapper : IFileLocationMapper
    {
        string _ServerUrl;
        IWebResources _WebResources;

        public UrlToPathMapper(string serverUrl, IWebResources webResources)
        {
            _ServerUrl = serverUrl;
            _WebResources = webResources;
        }

        public FileLocation Map(string filename, int lineNumber)
        {
            if (filename.StartsWith(_ServerUrl)) filename = filename.Substring(_ServerUrl.Length);
            return new FileLocation(filename, _WebResources.GetSystemFilePath(filename), lineNumber);
        }
    }
}