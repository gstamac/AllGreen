using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Linq;
using Microsoft.Owin.Testing;
using System.Web.Http.Routing;
using TemplateAttributes;
using FluentAssertions;
using System.Reflection;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public partial class ClientControllerTests
    {
        private IRunnerResources _RunnerResources;

        private ClientController CreateClientController(string filename, string path)
        {
            IWebResources webResources = new EmbededResources(@"AllGreen.WebServer.Resources", Assembly.Load("AllGreen.WebServer.Resources"));
            _RunnerResources = Mock.Of<IRunnerResources>();
            ClientController clientController = new ClientController(webResources, _RunnerResources);
            clientController.Request = new HttpRequestMessage(HttpMethod.Get, @"http://localhost/" + filename);

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

        [TestMethod]
        public void GetNotFound()
        {
            ClientController clientController = CreateClientController(@"nonexisting.html", @"nonexisting.html");

            HttpResponseMessage httpResponseMessage = clientController.Get();

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [DataTestMethod]
        [DataRow(@"allgreen.js", @"allgreen.js", @"text/js")]
        [DataRow(@"client.html", @"client.html", @"text/html")]
        [DataRow(@"client.css", @"client.css", @"text/css")]
        [DataRow(@"runner.html", @"runner.html", @"text/html")]
        [DataRow(@"reporter.js", @"reporter.js", @"text/js")]
        [DataRow(@"ReporterAdapters/jasmineAdapter.js", @"ReporterAdapters/jasmineAdapter.js", @"text/js")]
        [DataRow(@"Client/allgreen.js", @"allgreen.js", @"text/js")]
        [DataRow(@"Client/client.html", @"client.html", @"text/html")]
        [DataRow(@"Client/client.css", @"client.css", @"text/css")]
        [DataRow(@"Client/runner.html", @"runner.html", @"text/html")]
        [DataRow(@"Client/reporter.js", @"reporter.js", @"text/js")]
        [DataRow(@"Client/ReporterAdapters/jasmineAdapter.js", @"ReporterAdapters/jasmineAdapter.js", @"text/js")]
        public void GetClientResponse(string filename, string path, string contentType)
        {
            ClientController clientController = CreateClientController(filename, path);

            HttpResponseMessage httpResponseMessage = clientController.Get();

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.Headers.ContentType.ToString().Should().Be(contentType + "; charset=utf-8");
            httpResponseMessage.Content.ReadAsStringAsync().Result.Should().NotBeNullOrEmpty();

            CheckNoCache(httpResponseMessage);
        }

        [TestMethod]
        public void RunnerScriptsInject()
        {
            ClientController clientController = CreateClientController(@"runner.html", @"runner.html");
            Mock.Get(_RunnerResources).Setup(rr => rr.GetScriptFiles()).Returns(new string[] { "Scripts/jasmine.js", "Client/ReporterAdapters/jasmineAdapter.js", "Client/testScript.js" });

            HttpResponseMessage httpResponseMessage = clientController.Get();

            string responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
            responseContent.Should().Contain("<script src=\"Scripts/jasmine.js\"></script>");
            responseContent.Should().Contain("<script src=\"Client/ReporterAdapters/jasmineAdapter.js\"></script>");
            responseContent.Should().Contain("<script src=\"Client/testScript.js\"></script>");
        }
    }
}