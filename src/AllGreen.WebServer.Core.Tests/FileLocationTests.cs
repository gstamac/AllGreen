using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Http.Routing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplateAttributes;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class FileLocationTests
    {
        [TestMethod]
        public void CreateTest()
        {
            FileLocation fileLocation = new FileLocation("filename", "fullPath", 10);
            fileLocation.Filename.Should().Be("filename");
            fileLocation.FullPath.Should().Be("fullPath");
            fileLocation.LineNumber.Should().Be(10);

            fileLocation = new FileLocation("filename", "fullPath", 10, 20);
            fileLocation.Filename.Should().Be("filename");
            fileLocation.FullPath.Should().Be("fullPath");
            fileLocation.LineNumber.Should().Be(10);
            fileLocation.ColumnNumber.Should().Be(20);
        }

        [TestMethod]
        public void ToStringTest()
        {
            FileLocation fileLocation = new FileLocation("filename", "fullPath", 10);
            fileLocation.ToString().Should().Be("filename:10");
            
            fileLocation = new FileLocation("filename", "fullPath", 10, 20);
            fileLocation.ToString().Should().Be("filename:10:20");
        }
    }
}
