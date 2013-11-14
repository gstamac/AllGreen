using System;
using System.Collections.Generic;
using System.Dynamic;
using FluentAssertions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class RunnerHubTests
    {
        [TestMethod]
        public void SendTest()
        {
            dynamic all = new ExpandoObject();
            bool reloadCalled = false;
            all.reload = new Action(() => reloadCalled = true);

            var clientsMock = new Mock<IHubCallerConnectionContext>();
            clientsMock.Setup(c => c.All).Returns((ExpandoObject)all);

            RunnerHub runnerHub = new RunnerHub(Mock.Of<IReporter>());
            runnerHub.Clients = clientsMock.Object;

            runnerHub.ReloadAll();

            reloadCalled.Should().BeTrue();
        }

        [TestClass]
        public class ReceiveTests
        {
            private Mock<IReporter> _ReporterMock;
            private RunnerHub _RunnerHub;
            private string _ConnectionId;

            [TestInitialize]
            public void Setup()
            {
                _ReporterMock = new Mock<IReporter>();
                _ConnectionId = Guid.NewGuid().ToString();
                _RunnerHub = new RunnerHub(_ReporterMock.Object) { Context = CreateContext(_ConnectionId) };
            }

            [TestMethod]
            public void ReportsResetTest()
            {
                _RunnerHub.Reset();

                _ReporterMock.Verify(r => r.Reset(_ConnectionId));
            }

            [TestMethod]
            public void ReportsStartedTest()
            {
                _RunnerHub.Started(20);

                _ReporterMock.Verify(r => r.Started(_ConnectionId, 20));
            }

            [TestMethod]
            public void ReportsSpecUpdatedTest()
            {
                Spec spec = new Spec();

                _RunnerHub.SpecUpdated(spec);

                _ReporterMock.Verify(r => r.SpecUpdated(_ConnectionId, spec));
            }

            [TestMethod]
            public void ReportsFinishedTest()
            {
                _RunnerHub.Finished();

                _ReporterMock.Verify(r => r.Finished(_ConnectionId));
            }

            [TestMethod]
            public void ReportsConnectedTest()
            {
                _RunnerHub.OnConnected();

                _ReporterMock.Verify(r => r.Connected(_ConnectionId, "Windows 7 Other Firefox 23.0"));
            }

            [TestMethod]
            public void ReportsReconnectedTest()
            {
                _RunnerHub.OnReconnected();

                _ReporterMock.Verify(r => r.Reconnected(_ConnectionId, "Windows 7 Other Firefox 23.0"));
            }

            [TestMethod]
            public void ReportsDisconnectedTest()
            {
                _RunnerHub.OnDisconnected();

                _ReporterMock.Verify(r => r.Disconnected(_ConnectionId));
            }

            [TestMethod]
            public void ReportsRegisterTest()
            {
                dynamic caller = new ExpandoObject();
                bool reloadCalled = false;
                caller.reload = new Action(() => reloadCalled = true);

                var clientsMock = new Mock<IHubCallerConnectionContext>();
                clientsMock.Setup(c => c.Caller).Returns((ExpandoObject)caller);
                _RunnerHub.Clients = clientsMock.Object;
                _RunnerHub.Register();

                _ReporterMock.Verify(r => r.Register(_ConnectionId, "Windows 7 Other Firefox 23.0"));
                reloadCalled.Should().BeTrue();
            }

            private static HubCallerContext CreateContext(string connectionId)
            {
                Mock<INameValueCollection> headersMock = new Mock<INameValueCollection>();
                headersMock.Setup(h => h["User-Agent"]).Returns(@"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:23.0) Gecko/20100101 Firefox/23.0");

                IRequest request = Mock.Of<IRequest>(r => r.Headers == headersMock.Object);

                return new HubCallerContext(request, connectionId);
            }
        }
    }
}
