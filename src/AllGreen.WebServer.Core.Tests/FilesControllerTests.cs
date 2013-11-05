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
    public partial class FilesControllerTests
    {
        [DataTestMethod(@"script.js", @"text/js")]
        [DataTestMethod(@"content.html", @"text/html")]
        public void GetClientResponse(string filename, string contentType)
        {
            IWebResources webResources = Mock.Of<IWebResources>();
            Mock.Get(webResources).Setup(rr => rr.GetContent("Files/" + filename)).Returns("content");
            FilesController filesController = new FilesController(webResources)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, @"http://localhost/" + filename)
            };

            Dictionary<string, object> routeValues = new Dictionary<string, object>();
            IHttpRouteData routeData = Mock.Of<IHttpRouteData>(rd => rd.Values == routeValues);
            filesController.Request.SetRouteData(routeData);
            routeValues.Add("path", filename);

            HttpResponseMessage httpResponseMessage = filesController.Get();

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.Headers.ContentType.ToString().Should().Be(contentType + "; charset=utf-8");
            httpResponseMessage.Content.ReadAsStringAsync().Result.Should().NotBeNullOrEmpty();
        }

    }
}
