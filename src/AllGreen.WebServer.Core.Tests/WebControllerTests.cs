using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplateAttributes;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public partial class WebControllerTests : ApiControllerTestsBase
    {
        [TestInitialize]
        public void Setup()
        {
            _WebResources = Mock.Of<IWebResources>();
        }

        [TestMethod]
        public void GetNotFound()
        {
            HttpResponseMessage httpResponseMessage = GetHttpResponseMessage(@"/nonexisting.html");

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [DataTestMethod(@"script.js", @"text/js")]
        [DataTestMethod(@"content.html", @"text/html")]
        [DataTestMethod(@"Client/content.html", @"text/html")]
        [DataTestMethod(@"Client/content.css", @"text/css")]
        [DataTestMethod(@"Files/content.html", @"text/html")]
        public void GetRespone(string path, string contentType)
        {
            Mock.Get(_WebResources).Setup(wr => wr.GetContent(path)).Returns("content");

            HttpResponseMessage httpResponseMessage = GetHttpResponseMessage("/" + path);

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.Headers.ContentType.ToString().Should().Be(contentType + "; charset=utf-8");
            httpResponseMessage.Content.ReadAsStringAsync().Result.Should().Be("content");

            CheckNoCache(httpResponseMessage);
        }

        [TestMethod]
        public void GetDefaultResponse()
        {
            Mock.Get(_WebResources).Setup(wr => wr.GetContent("Client/client.html")).Returns("content");

            HttpResponseMessage httpResponseMessage = GetHttpResponseMessage("/");

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.Headers.ContentType.ToString().Should().Be("text/html; charset=utf-8");
            httpResponseMessage.Content.ReadAsStringAsync().Result.Should().Be("content");

            CheckNoCache(httpResponseMessage);
        }
    }
}
