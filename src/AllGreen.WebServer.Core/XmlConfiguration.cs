using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace AllGreen.WebServer.Core
{
    public class XmlConfiguration : IConfiguration
    {
        public string ServerUrl { get; set; }
        public string RootFolder { get; set; }
        public List<FolderFilter> ServedFolderFilters { get; set; }
        public List<FolderFilter> ExcludeServedFolderFilters { get; set; }
        public List<FolderFilter> WatchedFolderFilters { get; set; }

        public XmlConfiguration()
        {
            RootFolder = "";
            ServerUrl = "";
            ServedFolderFilters = new List<FolderFilter>();
            ExcludeServedFolderFilters = new List<FolderFilter>();
            WatchedFolderFilters = new List<FolderFilter>();
        }

        public void SaveTo(Stream stream)
        {
            new XmlSerializer(typeof(XmlConfiguration)).Serialize(stream, this);
        }

        public static XmlConfiguration LoadFrom(Stream stream)
        {
            return new XmlSerializer(typeof(XmlConfiguration)).Deserialize(stream) as XmlConfiguration;
        }

        //ncrunch: no coverage start
        public static XmlConfiguration LoadFrom(string filename)
        {
            XmlConfiguration configuration = new XmlConfiguration();
            if (File.Exists(filename))
            {
                using (FileStream fileStream = new FileStream(filename, FileMode.Open))
                {
                    configuration = XmlConfiguration.LoadFrom(fileStream);
                }
            }
            return configuration;
        }
        //ncrunch: no coverage start
    }
}
