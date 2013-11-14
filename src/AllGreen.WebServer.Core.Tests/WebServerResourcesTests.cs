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
    public partial class WebServerResourcesTests
    {
        [DataTestMethod]
        [DataRow(@"")]
        [DataRow(@"nonexistent.html")]
        public void FileDoesntExist(string path)
        {
            new WebServerResources(Mock.Of<IScriptList>()).GetContent(path).Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(@"Client/allgreen.js")]
        [DataRow(@"Client/client.html")]
        [DataRow(@"Client/client.css")]
        [DataRow(@"Client/runner.html")]
        [DataRow(@"Client/reporter.js")]
        [DataRow(@"Client/ReporterAdapters/jasmineAdapter.js")]
        [DataRow(@"Scripts/jquery.js")]
        [DataRow(@"Scripts/jquery.signalR.js")]
        [DataRow(@"/Scripts/jquery.signalR.js")]
        public void FileExists(string path)
        {
            new WebServerResources(Mock.Of<IScriptList>()).GetContent(path).Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void RunnerScriptsInject()
        {
            Mock<IScriptList> servedScriptListMock = new Mock<IScriptList>();
            servedScriptListMock.Setup(sl => sl.Files).Returns(new string[] { "Scripts/jasmine.js", "Client/ReporterAdapters/jasmineAdapter.js", "Client/testScript.js" });

            string responseContent = new WebServerResources(servedScriptListMock.Object).GetContent("Client/runner.html");

            responseContent.Should().Contain("<script src=\"/Scripts/jasmine.js\"></script>");
            responseContent.Should().Contain("<script src=\"/Client/ReporterAdapters/jasmineAdapter.js\"></script>");
            responseContent.Should().Contain("<script src=\"/Client/testScript.js\"></script>");
        }
    }
}