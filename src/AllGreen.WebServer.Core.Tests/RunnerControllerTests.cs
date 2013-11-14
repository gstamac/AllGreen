using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using FluentAssertions;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplateAttributes;
using TinyIoC;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public partial class RunnerControllerTests : ApiControllerTestsBase
    {
        [TestInitialize]
        public void Setup()
        {
            _WebResources = Mock.Of<IWebResources>();
        }

        [TestMethod]
        public void GetNotFound()
        {
            HttpResponseMessage httpResponseMessage = GetHttpResponseMessage(@"/Runner/nonexisting.html");

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [DataTestMethod(@"Runner/Client/allgreen.js", @"text/js")]
        [DataTestMethod(@"Runner/Client/client.html", @"text/html")]
        [DataTestMethod(@"Runner/Scripts/jquery.js", @"text/js")]
        public void GetResponeTest(string path, string contentType)
        {
            Mock.Get(_WebResources).Setup(wr => wr.GetContent(path)).Returns("content");
            
            HttpResponseMessage httpResponseMessage = GetHttpResponseMessage("/" + path);

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.Headers.ContentType.ToString().Should().Be(contentType + "; charset=utf-8");
            httpResponseMessage.Content.ReadAsStringAsync().Result.Should().Be("content");

            CheckNoCache(httpResponseMessage);
        }
    }
}