using System;
using System.Linq;
using System.Text.RegularExpressions;
using AllGreen.Core;

namespace AllGreen.Core
{
    public class FileLocationParser : IFileLocationParser
    {
        private string _ServerUrl;
        private IWebResources _WebResources;

        public FileLocationParser(string serverUrl, IWebResources webResources)
        {
            _ServerUrl = serverUrl;
            _WebResources = webResources;
        }

        public FileLocation Parse(string fileLocationText)
        {
            if (String.IsNullOrEmpty(fileLocationText))
                return null;

            FileLocation fileLocation = new FileLocation(fileLocationText, null, 0, 0);

            Match match = Regex.Match(fileLocationText, @"^(.+?)(:(\d+))?:(\d+)$");
            if (match.Success)
            {
                string filename = match.Groups[1].Value;
                int lineNumber = Int32.Parse(match.Groups[3].Success ? match.Groups[3].Value : match.Groups[4].Value);
                int columnNumber = match.Groups[3].Success ? Int32.Parse(match.Groups[4].Value) : 0;
                fileLocation = new FileLocation(filename, null, lineNumber, columnNumber);
            }

            if (!String.IsNullOrEmpty(_ServerUrl) && fileLocation.Filename.StartsWith(_ServerUrl))
                fileLocation.Filename = fileLocation.Filename.Substring(_ServerUrl.Length);
            fileLocation.FullPath = _WebResources.GetSystemFilePath(fileLocation.Filename);

            return fileLocation;
        }
    }
}
