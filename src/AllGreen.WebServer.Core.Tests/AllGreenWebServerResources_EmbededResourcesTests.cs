using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Routing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplateAttributes;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public partial class AllGreenWebServerResources_EmbededResourcesTests
    {
        [TestMethod]
        public void FileDoesntExist()
        {
            EmbededResources embededResources = new EmbededResources(Assembly.Load("AllGreen.WebServer.Resources"));
            embededResources.GetContent("").Should().BeNull();
            embededResources.GetContent("file1.js").Should().BeNull();
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
            EmbededResources embededResources = new EmbededResources(Assembly.Load("AllGreen.WebServer.Resources"));
            embededResources.GetContent(path).Should().NotBeNullOrEmpty();
        }
    }
}
