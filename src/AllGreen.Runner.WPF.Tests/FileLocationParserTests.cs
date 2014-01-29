using AllGreen.WebServer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;
using FluentAssertions;
using AllGreen.Runner.WPF.ViewModels;
using TemplateAttributes;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public partial class FileLocationParserTests
    {
        [DataTestMethod(@"file1.js:10", "file1.js", 10, 0)]
        [DataTestMethod(@"/folder1/file1.js:10", @"/folder1/file1.js", 10, 0)]
        [DataTestMethod(@"file1.js:10:20", "file1.js", 10, 20)]
        [DataTestMethod(@"file1.js", "file1.js", 0, 0)]
        public void Test(string fileLocationText, string fileName, int lineNumber, int columnNumber)
        {
            FileLocation fileLocation = new FileLocationParser(null, Mock.Of<IWebResources>()).Parse(fileLocationText);
            fileLocation.ShouldBeEquivalentTo(new { Filename = fileName, FullPath = (string)null, LineNumber = lineNumber, ColumnNumber = columnNumber });
        }

        [DataTestMethod(@"http://localhost:8080/file1.js:10", "/file1.js", 10, 0)]
        [DataTestMethod(@"http://localhost:8080/folder1/file1.js:10", @"/folder1/file1.js", 10, 0)]
        [DataTestMethod(@"http://localhost:8080/file1.js:10:20", "/file1.js", 10, 20)]
        [DataTestMethod(@"http://localhost:8080/Test/testScript.js:24:9", "/Test/testScript.js", 24, 9)]
        [DataTestMethod(@"http://localhost:8080/file1.js", "/file1.js", 0, 0)]
        public void ServerUrlTest(string fileLocationText, string fileName, int lineNumber, int columnNumber)
        {
            FileLocation fileLocation = new FileLocationParser("http://localhost:8080", Mock.Of<IWebResources>()).Parse(fileLocationText);
            fileLocation.ShouldBeEquivalentTo(new { Filename = fileName, FullPath = (string)null, LineNumber = lineNumber, ColumnNumber = columnNumber });
        }

        [DataTestMethod(@"/file1.js:10", @"/file1.js", @"C:\content\file1.js")]
        [DataTestMethod(@"/folder1/file1.js:10", @"/folder1/file1.js", @"C:\content\folder1/file1.js")]
        [DataTestMethod(@"/file1.js:10:20", @"/file1.js", @"C:\content\file1.js")]
        [DataTestMethod(@"/file1.js", @"/file1.js", @"C:\content\file1.js")]
        public void WebResourcesTest(string fileLocationText, string filename, string fullPath)
        {
            Mock<IWebResources> webResourcesMock = new Mock<IWebResources>();
            webResourcesMock.Setup(wr => wr.GetSystemFilePath(filename)).Returns(fullPath);

            FileLocation fileLocation = new FileLocationParser(null, webResourcesMock.Object).Parse(fileLocationText);
            fileLocation.FullPath.Should().Be(fullPath);
        }

        public void NullTest()
        {
            new FileLocationParser("", Mock.Of<IWebResources>()).Parse(null).Should().BeNull();
            new FileLocationParser("", Mock.Of<IWebResources>()).Parse("").Should().BeNull();
        }
    }
}
