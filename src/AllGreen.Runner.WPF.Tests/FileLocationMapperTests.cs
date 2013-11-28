using AllGreen.WebServer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;
using FluentAssertions;
using AllGreen.Runner.WPF.ViewModels;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public class UrlToPathMapperTests
    {
        [TestMethod]
        public void Test()
        {
            Mock<IWebResources> webResourcesMock = new Mock<IWebResources>();
            webResourcesMock.Setup(wr => wr.GetSystemFilePath("/file.js")).Returns(@"C:\Resources\file.js");

            UrlToPathMapper urlToPathMapper = new UrlToPathMapper(@"http://localhost:8080", webResourcesMock.Object);

            urlToPathMapper.Map(@"http://localhost:8080/file.js", 20).ShouldBeEquivalentTo(new { Filename = @"/file.js", FullPath = @"C:\Resources\file.js", LineNumber = 20 });
            urlToPathMapper.Map(@"http://localhost:8080/file1.js", 20).ShouldBeEquivalentTo(new { Filename = @"/file1.js", FullPath = (string)null, LineNumber = 20 });
        }
    }
}
