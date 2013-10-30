using System;
using System.Collections.Generic;
using System.Linq;

namespace AllGreen.WebServer.Core
{
    public class XmlConfiguration : IConfiguration
    {
        private string _Filename;

        public XmlConfiguration(string filename)
        {
            _Filename = filename;
        }

        public string RootFolder
        {
            get { return @"C:\Work\Projects\AllGreen\src\AllGreen.WebServer.Resources"; }
        }

        public string ServerUrl
        {
            get { return @"http://localhost:8080"; }
        }

        public IEnumerable<FolderFilter> ServedFolderFilters
        {
            get
            {
                //return new FolderFilter[0];
                return new FolderFilter[] { 
                    new FolderFilter() { Folder = "Scripts", FilePattern = "jasmine.js", IncludeSubfolders = false },
                    new FolderFilter() { Folder = "Client/ReporterAdapters", FilePattern = "jasmineAdapter.js", IncludeSubfolders = false },
                    new FolderFilter() { Folder = "Client", FilePattern = "testScript.js", IncludeSubfolders = false },
                };

                // "Scripts/jasmine.js", "Client/ReporterAdapters/jasmineAdapter.js", "Client/testScript.js" 
            }
        }

        public IEnumerable<FolderFilter> ExcludeServedFolderFilters
        {
            get
            {
                return new FolderFilter[0];
            }
        }

        public IEnumerable<FolderFilter> WatchedFolderFilters
        {
            get
            {
                return new FolderFilter[0];
            }
        }
    }
}
