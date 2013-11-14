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
    public class FileSystemResourcesTests
    {
        [TestMethod]
        public void Test()
        {
            IFileReader fileReader = Mock.Of<IFileReader>();
            Mock.Get<IFileReader>(fileReader).Setup(fr => fr.ReadAllText(@"C:\content\test.js")).Returns("content");

            FileSystemResources fileSystemResources = new FileSystemResources(@"C:\content\", fileReader);

            fileSystemResources.GetContent("test.js").Should().BeNull();

            fileSystemResources.GetContent("Files/test.js").Should().NotBeNull();
        }
    }
}
