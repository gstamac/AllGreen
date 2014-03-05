using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class AssemblyWebResourcesTests
    {
        [TestMethod]
        public void GetContentTest()
        {
            Mock<_Assembly> assemblyMock = new Mock<_Assembly>();
            assemblyMock.Setup(a => a.GetName()).Returns(new AssemblyName("Assembly.Namespace"));
            assemblyMock.Setup(a => a.GetManifestResourceStream("Assembly.Namespace.file1.js")).Returns(new MemoryStream(new byte[] { 65 }));
            assemblyMock.Setup(a => a.GetManifestResourceStream("Assembly.Namespace.folder1.file2.js")).Returns(new MemoryStream(new byte[] { 65 }));
            AssemblyWebResources webResources = new AssemblyWebResources(assemblyMock.Object);
            webResources.GetContent("").Should().BeNull();
            webResources.GetContent("file2.js").Should().BeNull();
            webResources.GetContent("file1.js").Should().NotBeNullOrEmpty();
            webResources.GetContent("folder1/file2.js").Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void GetSystemFilePathTest()
        {
            Mock<_Assembly> assemblyMock = new Mock<_Assembly>();
            assemblyMock.Setup(a => a.GetName()).Returns(new AssemblyName("Assembly.Namespace"));
            AssemblyWebResources webResources = new AssemblyWebResources(assemblyMock.Object);
            webResources.GetSystemFilePath("").Should().BeNull();
            webResources.GetSystemFilePath("file2.js").Should().BeNull();
            webResources.GetSystemFilePath("file1.js").Should().BeNull();
            webResources.GetSystemFilePath("folder1/file2.js").Should().BeNull();
        }

        [TestMethod]
        public void ExceptionTest()
        {
            Mock<_Assembly> assemblyMock = new Mock<_Assembly>();
            assemblyMock.Setup(a => a.GetName()).Returns(new AssemblyName("Assembly.Namespace"));
            assemblyMock.Setup(a => a.GetManifestResourceStream("Assembly.Namespace.file1.js")).Throws(new Exception());
            AssemblyWebResources webResources = new AssemblyWebResources(assemblyMock.Object);
            webResources.GetContent("file1.js").Should().BeNull();
        }
    }

}