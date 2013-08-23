using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;
using TinyIoC;
using Microsoft.AspNet.SignalR;

namespace AllGreen.WebServer.Core
{
    public class SignalRDependencyResolver : DefaultDependencyResolver
    {
        private TinyIoCContainer _TinyIoCContainer;

        public SignalRDependencyResolver(TinyIoCContainer tinyIoCContainer)
        {
            _TinyIoCContainer = tinyIoCContainer;
        }

        public override object GetService(Type serviceType)
        {
            if (_TinyIoCContainer.CanResolve(serviceType))
                return _TinyIoCContainer.Resolve(serviceType);
            return base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            if (_TinyIoCContainer.CanResolve(serviceType))
                return _TinyIoCContainer.ResolveAll(serviceType);
            return base.GetServices(serviceType);
        }
    }
}