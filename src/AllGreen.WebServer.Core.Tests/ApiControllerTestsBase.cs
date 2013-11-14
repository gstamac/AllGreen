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
    public class ApiControllerTestsBase
    {
        protected IWebResources _WebResources;

        protected HttpResponseMessage GetHttpResponseMessage(string urlPath)
        {
            TinyIoCContainer ioc = new TinyIoCContainer();
            ioc.Register<IWebResources>(_WebResources);

            TestServer testServer = TestServer.Create(appBuilder => new OwinStartup(ioc).Configuration(appBuilder));
            HttpClient httpClient = testServer.HttpClient;

            HttpResponseMessage httpResponseMessage = httpClient.GetAsync(@"http://localhost" + urlPath).Result;
            return httpResponseMessage;
        }

        protected void CheckNoCache(HttpResponseMessage httpResponseMessage)
        {
            httpResponseMessage.Headers.CacheControl.NoCache.Should().BeTrue();
            httpResponseMessage.Headers.Pragma.ToString().Should().Be("no-cache");
            httpResponseMessage.Content.Headers.Expires.Should().NotBeNull();
            httpResponseMessage.Content.Headers.Expires.Value.Should().BeLessOrEqualTo(DateTime.Now.AddDays(-1));
        }
    }
}
