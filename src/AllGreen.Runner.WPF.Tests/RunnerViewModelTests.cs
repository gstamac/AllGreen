using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public class RunnerViewModelTests
    {
        RunnerViewModel _RunnerViewModel;

        [TestInitialize]
        public void Setup()
        {
            _RunnerViewModel = new RunnerViewModel();
        }

        [TestMethod]
        public void PropertyChangedTests()
        {
            TestHelper.TestPropertyChanged(_RunnerViewModel)
                .Action(vm => vm.ConnectionId = Guid.NewGuid().ToString()).Changes("ConnectionId").Changes("Name")
                .Action(vm => vm.UserAgent = "NEW USER AGENT").Changes("UserAgent").Changes("Name")
                .Action(vm => vm.Name = "NEW NAME").Changes("Name")
                .Action(vm => vm.Status = "NEW STATUS").Changes("Status");
        }

        [TestMethod]
        public void ConnectionIdShouldBeUsedForName()
        {
            _RunnerViewModel.ConnectionId = Guid.NewGuid().ToString();
            _RunnerViewModel.Name.Should().Be(_RunnerViewModel.ConnectionId.ToString());
        }

        [TestMethod]
        public void UserAgentShouldBeUsedForName()
        {
            _RunnerViewModel.UserAgent = "USERAGENT1";
            _RunnerViewModel.Name.Should().Be("USERAGENT1");
            _RunnerViewModel.UserAgent = "USERAGENT2 (some params)";
            _RunnerViewModel.Name.Should().Be("USERAGENT2");
            _RunnerViewModel.UserAgent = "";
            _RunnerViewModel.Name.Should().Be("USERAGENT2");
        }
    }
}