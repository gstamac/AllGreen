using System;
using System.Collections.Generic;
using System.Dynamic;
using FluentAssertions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.WebServer.Owin.Tests
{
    [TestClass]
    public class RunnerClientsTest
    {
        [TestMethod]
        public void SendTest()
        {
            dynamic all = new ExpandoObject();
            bool reloadCalled = false;
            all.reload = new Action(() => reloadCalled = true);

            var clientsMock = new Mock<IHubCallerConnectionContext>();
            clientsMock.Setup(c => c.All).Returns((ExpandoObject)all);

            RunnerClients runnerClients = new RunnerClients(clientsMock.Object);

            runnerClients.ReloadAll();

            reloadCalled.Should().BeTrue();
        }

    }
}
