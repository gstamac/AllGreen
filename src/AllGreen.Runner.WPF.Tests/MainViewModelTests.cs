using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using AllGreen.WebServer.Core;
using System.Text.RegularExpressions;
using TinyIoC;
using Moq;

namespace AllGreen.Runner.WPF.Tests
{

    [TestClass]
    public class MainViewModelTests
    {
        protected MainViewModel _MainViewModel;

        [TestInitialize]
        public void Setup()
        {
            _MainViewModel = new MainViewModel();
        }

        [TestMethod]
        public void PropertyChangedTests()
        {
            TestHelper.TestPropertyChanged(_MainViewModel)
                .Action(() => _MainViewModel.ServerStatus = "NEW STATUS").Changes("ServerStatus");
        }

        [TestMethod]
        public void RunnersCollectionChangedTests()
        {
            (new TestHelper.ObservableCollectionTester<RunnerViewModel>(_MainViewModel.Runners)).RunTests();
        }

        [TestMethod]
        public void SuitesCollectionChangedTests()
        {
            (new TestHelper.ObservableCollectionTester<SuiteViewModel>(_MainViewModel.Suites)).RunTests();
        }

        [TestMethod]
        public void RunAllTestsCommandShouldFireReloadOnAllClients()
        {
            Mock<IRunnerHub> mockOfRunnerHub = new Mock<IRunnerHub>();
            _MainViewModel.ResourceResolver.Register<IRunnerHub>(mockOfRunnerHub.Object);
            _MainViewModel.RunAllTestsCommand.Execute(null);

            mockOfRunnerHub.Verify(rh => rh.ReloadAll());
        }
    }
}