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
    public class SignalRDependencyResolverTests
    {
        private TinyIoC.TinyIoCContainer _IoCContainer;
        private SignalRDependencyResolver _SignalRDependencyResolver;

        [TestInitialize]
        public void Setup()
        {
            _IoCContainer = new TinyIoC.TinyIoCContainer();
            _SignalRDependencyResolver = new SignalRDependencyResolver(_IoCContainer);
        }

        [TestMethod]
        public void GetServiceTest()
        {
            _SignalRDependencyResolver.GetService(typeof(IEnumerable)).Should().BeNull();

            IEnumerable dummy = Mock.Of<IEnumerable>();
            _IoCContainer.Register<IEnumerable>(dummy);
            _SignalRDependencyResolver.GetService(typeof(IEnumerable)).Should().Be(dummy);
        }

        [TestMethod]
        public void GetServicesTest()
        {
            _SignalRDependencyResolver.GetServices(typeof(IEnumerable)).Should().BeNull();

            IEnumerable dummy = Mock.Of<IEnumerable>();
            _IoCContainer.Register<IEnumerable>(dummy);
            _SignalRDependencyResolver.GetServices(typeof(IEnumerable)).ShouldAllBeEquivalentTo(new object[] { dummy });
        }
    }
}
