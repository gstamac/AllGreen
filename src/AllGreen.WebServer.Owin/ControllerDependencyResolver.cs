using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using TinyIoC;

namespace AllGreen.WebServer.Owin
{
    public class ControllerDependencyResolver : IDependencyResolver
    {
        private TinyIoCContainer _TinyIoCContainer;

        public ControllerDependencyResolver(TinyIoCContainer tinyIoCContainer)
        {
            _TinyIoCContainer = tinyIoCContainer;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            object service = null;
            if (!_TinyIoCContainer.CanResolve(serviceType) || !_TinyIoCContainer.TryResolve(serviceType, out service))
                service = null;
            return service;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new object[] { GetService(serviceType) };
        }

        public void Dispose()
        {
            // When BeginScope returns 'this', the Dispose method must be a no-op.
        }
    }
}
