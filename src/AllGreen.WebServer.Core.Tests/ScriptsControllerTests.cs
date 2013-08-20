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
    public partial class ScriptsControllerTests
    {
        protected ScriptsController CreateScriptsController(string filename, string path)
        {
            IWebResources webResources = new EmbededResources(@"AllGreen.WebServer.Resources", Assembly.Load("AllGreen.WebServer.Resources"));
            ScriptsController scriptsController = new ScriptsController(webResources);
            scriptsController.Request = new HttpRequestMessage(HttpMethod.Get, @"http://localhost/" + filename);

            Dictionary<string, object> routeValues = new Dictionary<string, object>();
            IHttpRouteData routeData = Mock.Of<IHttpRouteData>(rd => rd.Values == routeValues);
            scriptsController.Request.SetRouteData(routeData);
            routeValues.Add("path", path);

            return scriptsController;
        }

        [DataTestMethod(@"jquery.js", @"text/js")]
        [DataTestMethod(@"jquery.signalR.js", @"text/js")]
        public void GetClientResponse(string filename, string contentType)
        {
            ScriptsController scriptsController = CreateScriptsController(filename, filename.Replace("Scripts/", ""));

            HttpResponseMessage httpResponseMessage = scriptsController.Get();

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.Headers.ContentType.ToString().Should().Be(contentType + "; charset=utf-8");
            httpResponseMessage.Content.ReadAsStringAsync().Result.Should().NotBeNullOrEmpty();
        }
    }
}
