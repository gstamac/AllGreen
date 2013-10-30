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
            [Ignore]
            public void HubsTest()
            {
                Dictionary<string, object> appProperties = new Dictionary<string, object>();
                //appProperties.Add("builder.DefaultApp", "default");
                appProperties.Add("builder.AddSignatureConversion", new Action<Delegate>((d) => { }));

                IAppBuilder app = Mock.Of<IAppBuilder>(a => a.Properties == appProperties);

                _OwinStartup.Configuration(app);

                //app.BuildNew
            }

            [TestMethod]
            [Ignore]
            public async Task Test()
            {
                TestServer testServer = TestServer.Create(appBuilder => new OwinStartup(new TinyIoCContainer()).Configuration(appBuilder));
                HttpClient httpClient = testServer.HttpClient;
                HttpResponseMessage response = await httpClient.GetAsync(@"http://localhost");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
