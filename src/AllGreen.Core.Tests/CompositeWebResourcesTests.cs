using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.Core.Tests
{
    [TestClass]
    public class CompositeWebResourcesTests
    {
        [TestMethod]
        public void GetContentTest()
        {
            CompositeWebResources compositeWebResources = new CompositeWebResources();
            Mock<IWebResources> webResources1Mock = new Mock<IWebResources>();
            webResources1Mock.Setup(wr => wr.GetContent("file1.js")).Returns("file1 content");
            Mock<IWebResources> webResources2Mock = new Mock<IWebResources>();
            webResources1Mock.Setup(wr => wr.GetContent("file2.js")).Returns("file2 content");
            compositeWebResources.Add(webResources1Mock.Object);
            compositeWebResources.Add(webResources2Mock.Object);

            compositeWebResources.GetContent("file1.js").Should().Be("file1 content");
            compositeWebResources.GetContent("file2.js").Should().Be("file2 content");
            compositeWebResources.GetContent("file3.js").Should().BeNull();
        }

        [TestMethod]
        public void GetSystemFilePathTest()
        {
            CompositeWebResources compositeWebResources = new CompositeWebResources();
            Mock<IWebResources> webResources1Mock = new Mock<IWebResources>();
            webResources1Mock.Setup(wr => wr.GetSystemFilePath("file1.js")).Returns("c:\resources\file1.js");
            Mock<IWebResources> webResources2Mock = new Mock<IWebResources>();
            webResources1Mock.Setup(wr => wr.GetSystemFilePath("file2.js")).Returns("c:\resources\file2.js");
            compositeWebResources.Add(webResources1Mock.Object);
            compositeWebResources.Add(webResources2Mock.Object);

            compositeWebResources.GetSystemFilePath("file1.js").Should().Be("c:\resources\file1.js");
            compositeWebResources.GetSystemFilePath("file2.js").Should().Be("c:\resources\file2.js");
            compositeWebResources.GetSystemFilePath("file3.js").Should().BeNull();
        }
    }
}
