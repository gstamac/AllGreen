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
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class RunnerHubTests
    {
        public class ReloadSender
        {
            public bool reloadCalled = false;
            public void reload()
            {
                reloadCalled = true;
            }
        }

        [TestMethod]
        public void SendTest()
        {
            var clientsMock = new Mock<IHubConnectionContext>();
            ReloadSender reloadSender = new ReloadSender();
            clientsMock.Setup(c => c.All).Returns(reloadSender);
            IHubContext hubContext = Mock.Of<IHubContext>(hc => hc.Clients == clientsMock.Object);
            RunnerHub _RunnerHub = new RunnerHub(hubContext, Mock.Of<IReporter>());

            _RunnerHub.Reload();

            reloadSender.reloadCalled.Should().BeTrue();
        }

        [TestClass]
        public class ReceiveTests
        {
            private Mock<IReporter> _ReporterMock;
            private RunnerHub _RunnerHub;
            private Guid _ConnectionId;

            [TestInitialize]
            public void Setup()
            {
                IHubContext hubContext = Mock.Of<IHubContext>();
                _ReporterMock = new Mock<IReporter>();
                _RunnerHub = new RunnerHub(hubContext, _ReporterMock.Object);

                _ConnectionId = Guid.NewGuid();
            }

            [TestMethod]
            public void ReceiveResetTest()
            {
                _RunnerHub.Reset(_ConnectionId);

                _ReporterMock.Verify(r => r.Reset(_ConnectionId));
            }

            [TestMethod]
            public void ReceiveStartedTest()
            {
                _RunnerHub.Started(_ConnectionId, 20);

                _ReporterMock.Verify(r => r.Started(_ConnectionId, 20));
            }

            [TestMethod]
            public void ReceiveSpecResultTest()
            {
                Spec spec = new Spec();

                _RunnerHub.SpecUpdated(_ConnectionId, spec);

                _ReporterMock.Verify(r => r.SpecUpdated(_ConnectionId, spec));
            }

            [TestMethod]
            public void ReceiveFinishedTest()
            {
                _RunnerHub.Finished(_ConnectionId);

                _ReporterMock.Verify(r => r.Finished(_ConnectionId));
            }

            [TestMethod]
            public void ReceiveRegisterTest()
            {
                var clientsMock = new Mock<IHubCallerConnectionContext>();
                ReloadSender reloadSender = new ReloadSender();
                clientsMock.Setup(c => c.Caller).Returns(reloadSender);
                _RunnerHub.Clients = clientsMock.Object;
                _RunnerHub.Register(_ConnectionId, "userAgent");

                _ReporterMock.Verify(r => r.Register(_ConnectionId, "userAgent"));
                reloadSender.reloadCalled.Should().BeTrue();
            }
        }
    }
}
