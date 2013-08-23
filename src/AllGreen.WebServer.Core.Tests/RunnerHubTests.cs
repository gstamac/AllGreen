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
        public class AllClientsMock
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
            AllClientsMock allClientsMock = new AllClientsMock();
            clientsMock.Setup(c => c.All).Returns(allClientsMock);
            IHubContext hubContext = Mock.Of<IHubContext>(hc => hc.Clients == clientsMock.Object);
            RunnerHub runnerHub = new RunnerHub(hubContext, Mock.Of<IReporter>());
            
            runnerHub.Reload();
            
            allClientsMock.reloadCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ReceiveSpecResultTest()
        {
            IHubContext hubContext = Mock.Of<IHubContext>();
            Spec reportedSpec = null;
            var reporterMock = new Mock<IReporter>();
            reporterMock.Setup(r => r.SpecUpdated(It.IsAny<Spec>())).Callback<Spec>(s => reportedSpec = s);
            RunnerHub runnerHub = new RunnerHub(hubContext, reporterMock.Object);
            Spec spec = Mock.Of<Spec>();
            
            runnerHub.SpecUpdated(spec);

            reportedSpec.Should().BeSameAs(spec);
        }
    }
}
