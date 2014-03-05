using System;
using AllGreen.WebServer.Core;
using Microsoft.Owin.Hosting;
using Owin;
using TinyIoC;

namespace AllGreen.WebServer.Owin
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
    public class OwinServerStarter : IServerStarter
    {
        private readonly TinyIoCContainer _ResourceResolver;

        public OwinServerStarter(TinyIoCContainer resourceResolver)
        {
            _ResourceResolver = resourceResolver;
        }

        public void Start()
        {
            WebApp.Start(_ResourceResolver.Resolve<IConfiguration>().ServerUrl, appBuilder => new OwinStartup(_ResourceResolver).Configuration(appBuilder));
        }
    }
}
