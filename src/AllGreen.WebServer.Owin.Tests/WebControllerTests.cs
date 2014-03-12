using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using AllGreen.Core;
using FluentAssertions;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplateAttributes;
using TinyIoC;

namespace AllGreen.WebServer.Owin.Tests
{
    [TestClass]
    public partial class WebControllerTests
    {
        protected IWebResources _WebResources;

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

        [TestMethod]
        public void GetDefaultResponse()
        {
            Mock.Get(_WebResources).Setup(wr => wr.GetContent("~internal~/Client/client.html")).Returns("content");

            HttpResponseMessage httpResponseMessage = GetHttpResponseMessage("/");

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.Headers.ContentType.ToString().Should().Be("text/html; charset=utf-8");
            httpResponseMessage.Content.ReadAsStringAsync().Result.Should().Be("content");

            CheckNoCache(httpResponseMessage);
        }

        [DataTestMethod(@"script.js", @"text/js")]
        [DataTestMethod(@"~internal~/content.html", @"text/html")]
        [DataTestMethod(@"~internal~/Client/content.html", @"text/html")]
        [DataTestMethod(@"~internal~/Client/content.css", @"text/css")]
        [DataTestMethod(@"content.html", @"text/html")]
        public void GetRespone(string path, string contentType)
        {
            Mock.Get(_WebResources).Setup(wr => wr.GetContent(path)).Returns("content");

            HttpResponseMessage httpResponseMessage = GetHttpResponseMessage("/" + path);

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.Headers.ContentType.ToString().Should().Be(contentType + "; charset=utf-8");
            httpResponseMessage.Content.ReadAsStringAsync().Result.Should().Be("content");

            CheckNoCache(httpResponseMessage);
        }

        private HttpResponseMessage GetHttpResponseMessage(string urlPath)
        {
            TinyIoCContainer ioc = new TinyIoCContainer();
            ioc.Register<IWebResources>(_WebResources);

            TestServer testServer = TestServer.Create(appBuilder => new OwinStartup(ioc).Configuration(appBuilder));
            HttpClient httpClient = testServer.HttpClient;

            HttpResponseMessage httpResponseMessage = httpClient.GetAsync(@"http://localhost" + urlPath).Result;
            return httpResponseMessage;
        }

        private void CheckNoCache(HttpResponseMessage httpResponseMessage)
        {
            httpResponseMessage.Headers.CacheControl.NoCache.Should().BeTrue();
            httpResponseMessage.Headers.Pragma.ToString().Should().Be("no-cache");
            httpResponseMessage.Content.Headers.Expires.Should().NotBeNull();
            httpResponseMessage.Content.Headers.Expires.Value.Should().BeLessOrEqualTo(DateTime.Now.AddDays(-1));
        }

    }
}
