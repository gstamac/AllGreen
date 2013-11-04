using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Owin;
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
            public async Task Test()
            {
                TinyIoCContainer ioc = new TinyIoCContainer();
                Mock<IWebResources> webResourcesMock = new Mock<IWebResources>();
                webResourcesMock.Setup(wr => wr.GetContent(It.IsAny<string>())).Returns("");
                ioc.Register<ClientController>(new ClientController(webResourcesMock.Object, Mock.Of<IRunnerResources>()));
                TestServer testServer = TestServer.Create(appBuilder => new OwinStartup(ioc).Configuration(appBuilder));
                HttpClient httpClient = testServer.HttpClient;
                HttpResponseMessage response = await httpClient.GetAsync(@"http://localhost");
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }
    }
}
