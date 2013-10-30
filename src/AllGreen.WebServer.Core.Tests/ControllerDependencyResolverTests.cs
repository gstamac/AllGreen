using System.Collections;
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
    public class ControllerDependencyResolverTests
    {
        private TinyIoC.TinyIoCContainer _IoCContainer;
        private ControllerDependencyResolver _ControllerDependencyResolver;

        [TestInitialize]
        public void Setup()
        {
            _IoCContainer = new TinyIoC.TinyIoCContainer();
            _ControllerDependencyResolver = new ControllerDependencyResolver(_IoCContainer);
        }

        [TestCleanup]
        public void TearDown()
        {
            _ControllerDependencyResolver.Dispose();
        }

        [TestMethod]
        public void BeginScopeTest()
        {
            _ControllerDependencyResolver.BeginScope().Should().Be(_ControllerDependencyResolver);
        }

        [TestMethod]
        public void GetServiceTest()
        {
            _ControllerDependencyResolver.GetService(typeof(IEnumerable)).Should().BeNull();

            IEnumerable dummy = Mock.Of<IEnumerable>();
            _IoCContainer.Register<IEnumerable>(dummy);
            _ControllerDependencyResolver.GetService(typeof(IEnumerable)).Should().Be(dummy);
        }

        [TestMethod]
        public void GetServicesTest()
        {
            IEnumerable dummy = Mock.Of<IEnumerable>();
            _IoCContainer.Register<IEnumerable>(dummy);
            _ControllerDependencyResolver.GetServices(typeof(IEnumerable)).ShouldAllBeEquivalentTo(new object[] { dummy });
        }
    }
}
