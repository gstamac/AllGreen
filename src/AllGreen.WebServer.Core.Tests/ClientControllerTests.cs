using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Routing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplateAttributes;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public partial class ClientControllerTests
    {
        private IWebResources _WebResources;
        private IRunnerResources _RunnerResources;

        [TestMethod]
        public void GetNotFound()
        {
            ClientController clientController = CreateClientController(@"nonexisting.html", @"nonexisting.html");

            HttpResponseMessage httpResponseMessage = clientController.Get();

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [DataTestMethod]
        [DataRow(@"/Client/allgreen.js", @"Client/allgreen.js", @"text/js")]
        [DataRow(@"/Client/client.html", @"Client/client.html", @"text/html")]
        public void GetClientResponse(string urlPath, string path, string contentType)
        {
            ClientController clientController = CreateClientController(urlPath, path);
            Mock.Get(_WebResources).Setup(rr => rr.GetContent(path)).Returns("content");

            HttpResponseMessage httpResponseMessage = clientController.Get();

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.Headers.ContentType.ToString().Should().Be(contentType + "; charset=utf-8");
            httpResponseMessage.Content.ReadAsStringAsync().Result.Should().NotBeNullOrEmpty();

            CheckNoCache(httpResponseMessage);
        }

        [TestMethod]
        public void RunnerScriptsInject()
        {
            ClientController clientController = CreateClientController(@"/Client/runner.html", @"Client/runner.html");
            Mock.Get(_WebResources).Setup(rr => rr.GetContent(@"Client/runner.html")).Returns("content <!--%SCRIPTS%-->");
            Mock.Get(_RunnerResources).Setup(rr => rr.GetScriptFiles()).Returns(new string[] { "Scripts/jasmine.js", "Client/ReporterAdapters/jasmineAdapter.js", "Client/testScript.js" });

            HttpResponseMessage httpResponseMessage = clientController.Get();

            string responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
            responseContent.Should().Contain("<script src=\"/Scripts/jasmine.js\"></script>");
            responseContent.Should().Contain("<script src=\"/Client/ReporterAdapters/jasmineAdapter.js\"></script>");
            responseContent.Should().Contain("<script src=\"/Client/testScript.js\"></script>");
        }

        private ClientController CreateClientController(string filename, string path)
        {
            _WebResources = Mock.Of<IWebResources>();
            _RunnerResources = Mock.Of<IRunnerResources>();
            ClientController clientController = new ClientController(_WebResources, _RunnerResources)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, @"http://localhost" + filename)
            };

            Dictionary<string, object> routeValues = new Dictionary<string, object>();
            IHttpRouteData routeData = Mock.Of<IHttpRouteData>(rd => rd.Values == routeValues);
            clientController.Request.SetRouteData(routeData);
            routeValues.Add("path", path);

            return clientController;
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