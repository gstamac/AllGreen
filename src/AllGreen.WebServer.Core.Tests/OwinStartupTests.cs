using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;
using FluentAssertions;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class OwinStartupTests
    {
        protected OwinStartup _OwinStartup;

        [TestInitialize]
        public void Setup()
        {
            _OwinStartup = new OwinStartup(new TinyIoCContainer());
        }

        [TestClass]
        public class ConfigurationTests : OwinStartupTests
        {
            [TestMethod]
            public void Test()
            {
                TinyIoCContainer ioc = new TinyIoCContainer();
                Mock<IWebResources> webResourcesMock = new Mock<IWebResources>();
                webResourcesMock.Setup(wr => wr.GetContent("~internal~/Client/client.html")).Returns("content");
                ioc.Register<IWebResources>(webResourcesMock.Object);

                TestServer testServer = TestServer.Create(appBuilder => new OwinStartup(ioc).Configuration(appBuilder));
                HttpClient httpClient = testServer.HttpClient;

                HttpResponseMessage response = httpClient.GetAsync(@"http://localhost").Result;

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }
    }
}
