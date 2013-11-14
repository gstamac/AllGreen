using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class XmlConfigurationTests
    {
        XmlConfiguration _XmlConfiguration;
        XDocument _XDocument;

        [TestInitialize]
        public void Setup()
        {
            _XmlConfiguration = new XmlConfiguration()
            {
                RootFolder = @"C:\root",
                ServerUrl = @"http://localhost:8080",
                ServedFolderFilters = new List<FolderFilter>(new FolderFilter[] { 
                    new FolderFilter { Folder = @"C:\", FilePattern = "*.js", IncludeSubfolders = true },
                    new FolderFilter { Folder = @"D:\", FilePattern = "*.html", IncludeSubfolders = false } 
                }),
                ExcludeServedFolderFilters = new List<FolderFilter>(new FolderFilter[] { 
                    new FolderFilter { Folder = @"E:\", FilePattern = "*.js", IncludeSubfolders = true },
                    new FolderFilter { Folder = @"F:\", FilePattern = "*.html", IncludeSubfolders = false } 
                }),
                WatchedFolderFilters = new List<FolderFilter>(new FolderFilter[] { 
                    new FolderFilter { Folder = @"G:\", FilePattern = "*.js", IncludeSubfolders = true },
                    new FolderFilter { Folder = @"H:\", FilePattern = "*.html", IncludeSubfolders = false } 
                })
            };

            _XDocument = new XDocument(
                new XElement("XmlConfiguration",
                    new XAttribute(XNamespace.Xmlns + "xsi", XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance")),
                    new XAttribute(XNamespace.Xmlns + "xsd", XNamespace.Get("http://www.w3.org/2001/XMLSchema")),
                    new XElement("RootFolder", @"C:\root"),
                    new XElement("ServerUrl", @"http://localhost:8080"),
                    new XElement("ServedFolderFilters",
                        new XElement("FolderFilter",
                            new XElement("Folder", @"C:\"), new XElement("FilePattern", "*.js"), new XElement("IncludeSubfolders", @"true")
                            ),
                        new XElement("FolderFilter",
                            new XElement("Folder", @"D:\"), new XElement("FilePattern", "*.html"), new XElement("IncludeSubfolders", @"false")
                            )
                        ),
                    new XElement("ExcludeServedFolderFilters",
                        new XElement("FolderFilter",
                            new XElement("Folder", @"E:\"), new XElement("FilePattern", "*.js"), new XElement("IncludeSubfolders", @"true")
                            ),
                        new XElement("FolderFilter",
                            new XElement("Folder", @"F:\"), new XElement("FilePattern", "*.html"), new XElement("IncludeSubfolders", @"false")
                            )
                        ),
                    new XElement("WatchedFolderFilters",
                        new XElement("FolderFilter",
                            new XElement("Folder", @"G:\"), new XElement("FilePattern", "*.js"), new XElement("IncludeSubfolders", @"true")
                            ),
                        new XElement("FolderFilter",
                            new XElement("Folder", @"H:\"), new XElement("FilePattern", "*.html"), new XElement("IncludeSubfolders", @"false")
                            )
                        )
                    )
            );
        }

        [TestMethod]
        public void SaveToTest()
        {
            MemoryStream memoryStream = new MemoryStream();
            _XmlConfiguration.SaveTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            XDocument.Load(memoryStream).Should().BeEquivalentTo(_XDocument);
        }

        [TestMethod]
        public void LoadFromTest()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream))
            {
                _XDocument.WriteTo(xmlWriter);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            XmlConfiguration.LoadFrom(memoryStream).ShouldBeEquivalentTo(_XmlConfiguration);
        }
    }
}