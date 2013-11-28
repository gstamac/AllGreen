using System;
using System.Windows.Media;
using AllGreen.Runner.WPF.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                .Action(vm => vm.Status = "NEW STATUS").Changes("Status")
                .Action(vm => vm.Background = new SolidColorBrush()).Changes("Background");

            _RunnerViewModel.ConnectionId.Should().NotBeNull();
            _RunnerViewModel.UserAgent.Should().Be("NEW USER AGENT");
            _RunnerViewModel.Name.Should().Be("NEW NAME");
            _RunnerViewModel.Status.Should().Be("NEW STATUS");
            _RunnerViewModel.Background.Should().BeOfType<SolidColorBrush>();
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