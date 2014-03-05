using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class FileSystemResourcesTests
    {
        [TestMethod]
        public void GetContentTest()
        {
            Mock<IScriptList> scriptListMock = new Mock<IScriptList>();
            scriptListMock.Setup(sl => sl.Scripts).Returns(new string[] { "test.js", "test1.js", "test3.js" });

            IFileSystem fileSystem = Mock.Of<IFileSystem>();
            Mock.Get<IFileSystem>(fileSystem).Setup(fr => fr.FileExists(@"C:\content\test.js")).Returns(true);
            Mock.Get<IFileSystem>(fileSystem).Setup(fr => fr.ReadAllText(@"C:\content\test.js")).Returns("content");
            Mock.Get<IFileSystem>(fileSystem).Setup(fr => fr.FileExists(@"C:\content\test1.js")).Returns(true);
            Mock.Get<IFileSystem>(fileSystem).Setup(fr => fr.FileExists(@"C:\content\test2.js")).Returns(true);
            Mock.Get<IFileSystem>(fileSystem).Setup(fr => fr.ReadAllText(@"C:\content\test2.js")).Returns("content");

            FileSystemResources fileSystemResources = new FileSystemResources(@"C:\content\", scriptListMock.Object, fileSystem);

            fileSystemResources.GetContent("test.js").Should().NotBeNull();
            fileSystemResources.GetContent("test1.js").Should().BeNull();
            fileSystemResources.GetContent("test2.js").Should().BeNull();
            fileSystemResources.GetContent("test3.js").Should().BeNull();
        }

        [TestMethod]
        public void GetSystemFilePathTest()
        {
            Mock<IScriptList> scriptListMock = new Mock<IScriptList>();
            scriptListMock.Setup(sl => sl.Scripts).Returns(new string[] { "test.js", "folder/test.js" });

            IFileSystem fileSystem = Mock.Of<IFileSystem>();
            Mock.Get<IFileSystem>(fileSystem).Setup(fr => fr.FileExists(@"C:\content\test.js")).Returns(true);
            Mock.Get<IFileSystem>(fileSystem).Setup(fr => fr.FileExists(@"C:\content\folder\test.js")).Returns(true);

            FileSystemResources fileSystemResources = new FileSystemResources(@"C:\content\", scriptListMock.Object, fileSystem);

            fileSystemResources.GetSystemFilePath("test.js").Should().Be(@"C:\content\test.js");
            fileSystemResources.GetSystemFilePath("/test.js").Should().Be(@"C:\content\test.js");
            fileSystemResources.GetSystemFilePath("folder/test.js").Should().Be(@"C:\content\folder\test.js");
            fileSystemResources.GetSystemFilePath("test1.js").Should().BeNull();
        }

        [TestMethod]
        public void RemoveAllGreenAppSoICanTestMyself()
        {
            Mock<IScriptList> scriptListMock = new Mock<IScriptList>();
            scriptListMock.Setup(sl => sl.Scripts).Returns(new string[] { "allgreen.js" });

            IFileSystem fileSystem = Mock.Of<IFileSystem>();
            Mock.Get<IFileSystem>(fileSystem).Setup(fr => fr.ReadAllText(@"C:\content\allgreen.js")).Returns("before; var AllGreenApp = null; after;");
            Mock.Get<IFileSystem>(fileSystem).Setup(fr => fr.FileExists(@"C:\content\allgreen.js")).Returns(true);

            FileSystemResources fileSystemResources = new FileSystemResources(@"C:\content\", scriptListMock.Object, fileSystem);

            fileSystemResources.GetContent("allgreen.js").Should().Be("before;  after;");
        }
    }
}
