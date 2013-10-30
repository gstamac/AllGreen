using FluentAssertions;
using Microsoft.AspNet.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class BootstrapperTests
    {
        [TestMethod]
        public void ConfigurationTest()
        {
            
        }

        [TestMethod]
        public void RegisterServicesTest()
        {
            TinyIoCContainer tinyIoCContainer = new TinyIoCContainer();
            tinyIoCContainer.Register<IConfiguration>(Mock.Of<IConfiguration>(c => c.WatchedFolderFilters == new FolderFilter[0]));
            tinyIoCContainer.Register<IReporter>(Mock.Of<IReporter>());

            Bootstrapper.RegisterServices(tinyIoCContainer);

            tinyIoCContainer.CanResolve<IWebResources>(ResolveOptions.FailUnregisteredAndNameNotFound).Should().BeTrue();
            tinyIoCContainer.CanResolve<IRunnerResources>(ResolveOptions.FailUnregisteredAndNameNotFound).Should().BeTrue();
            tinyIoCContainer.CanResolve<IHubContext>(ResolveOptions.FailUnregisteredAndNameNotFound).Should().BeTrue();
            tinyIoCContainer.CanResolve<IRunnerHub>(ResolveOptions.FailUnregisteredAndNameNotFound).Should().BeTrue();
        }
    }
}
