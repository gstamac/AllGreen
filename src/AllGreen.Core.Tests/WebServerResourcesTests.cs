using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplateAttributes;

namespace AllGreen.Core.Tests
{

    [TestClass]
    public partial class WebServerResourcesTests
    {
        [DataTestMethod]
        [DataRow(@"")]
        [DataRow(@"nonexistent.html")]
        [DataRow(@"Client/allgreen.js")]
        [DataRow(@"/Client/allgreen.js")]
        public void FileDoesntExist(string path)
        {
            new WebServerResources(Mock.Of<IScriptList>()).GetContent(path).Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(@"~internal~/Client/allgreen.js")]
        [DataRow(@"~internal~/Client/client.html")]
        [DataRow(@"~internal~/Client/client.css")]
        [DataRow(@"~internal~/Client/runner.html")]
        [DataRow(@"~internal~/Client/reporter.js")]
        [DataRow(@"~internal~/Client/ReporterAdapters/jasmineAdapter.js")]
        [DataRow(@"~internal~/Scripts/jquery.js")]
        [DataRow(@"~internal~/Scripts/jquery.signalR.js")]
        [DataRow(@"/~internal~/Scripts/jquery.signalR.js")]
        public void FileExists(string path)
        {
            new WebServerResources(Mock.Of<IScriptList>()).GetContent(path).Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void GetSystemFilePathTest()
        {
            WebServerResources webServerResources = new WebServerResources(Mock.Of<IScriptList>());
            webServerResources.GetSystemFilePath("").Should().BeNull();
            webServerResources.GetSystemFilePath("file2.js").Should().BeNull();
            webServerResources.GetSystemFilePath("file1.js").Should().BeNull();
            webServerResources.GetSystemFilePath("folder1/file2.js").Should().BeNull();
        }

        [TestMethod]
        public void RunnerScriptsInject()
        {
            Mock<IScriptList> servedScriptListMock = new Mock<IScriptList>();
            servedScriptListMock.Setup(sl => sl.Scripts).Returns(new string[] { "Scripts/jasmine.js", "Client/ReporterAdapters/jasmineAdapter.js", "Client/testScript.js" });

            string responseContent = new WebServerResources(servedScriptListMock.Object).GetContent("~internal~/Client/runner.html");

            responseContent.Should().Contain("<script src=\"/Scripts/jasmine.js\"></script>");
            responseContent.Should().Contain("<script src=\"/Client/ReporterAdapters/jasmineAdapter.js\"></script>");
            responseContent.Should().Contain("<script src=\"/Client/testScript.js\"></script>");
        }
    }
}