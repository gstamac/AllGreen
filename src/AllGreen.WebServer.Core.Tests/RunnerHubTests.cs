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
using Microsoft.AspNet.SignalR.Hosting;

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
            public void ReportsResetTest()
            {
                _RunnerHub.Reset(_ConnectionId);

                _ReporterMock.Verify(r => r.Reset(_ConnectionId));
            }

            [TestMethod]
            public void ReportsStartedTest()
            {
                _RunnerHub.Started(_ConnectionId, 20);

                _ReporterMock.Verify(r => r.Started(_ConnectionId, 20));
            }

            [TestMethod]
            public void ReportsSpecUpdatedTest()
            {
                Spec spec = new Spec();

                _RunnerHub.SpecUpdated(_ConnectionId, spec);

                _ReporterMock.Verify(r => r.SpecUpdated(_ConnectionId, spec));
            }

            [TestMethod]
            public void ReportsFinishedTest()
            {
                _RunnerHub.Finished(_ConnectionId);

                _ReporterMock.Verify(r => r.Finished(_ConnectionId));
            }

            [TestMethod]
            public void ReportsConnectedTest()
            {
                INameValueCollection headers = Mock.Of<INameValueCollection>();
                IRequest request = Mock.Of<IRequest>(r => r.Headers == headers);
                _RunnerHub.Context = new HubCallerContext(request, Guid.NewGuid().ToString());
                
                _RunnerHub.OnConnected();

                _ReporterMock.Verify(r => r.Connected(It.IsAny<Guid>(), It.IsAny<String>()));
            }

            [TestMethod]
            public void ReportsReconnectedTest()
            {
                _RunnerHub.Context = new HubCallerContext(null, Guid.NewGuid().ToString());
                _RunnerHub.OnReconnected();

                _ReporterMock.Verify(r => r.Reconnected(It.IsAny<Guid>()));
            }

            [TestMethod]
            public void ReportsDisconnectedTest()
            {
                _RunnerHub.Context = new HubCallerContext(null, Guid.NewGuid().ToString());
                _RunnerHub.OnDisconnected();

                _ReporterMock.Verify(r => r.Disconnected(It.IsAny<Guid>()));
            }

            [TestMethod]
            public void ReportsRegisterTest()
            {
                var clientsMock = new Mock<IHubCallerConnectionContext>();
                ReloadSender reloadSender = new ReloadSender();
                clientsMock.Setup(c => c.Caller).Returns(reloadSender);
                _RunnerHub.Clients = clientsMock.Object;
                _RunnerHub.Register(_ConnectionId, @"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:23.0) Gecko/20100101 Firefox/23.0");

                _ReporterMock.Verify(r => r.Register(_ConnectionId, "Windows 7  Firefox 23.0"));
                reloadSender.reloadCalled.Should().BeTrue();
            }

        }
    }
}
