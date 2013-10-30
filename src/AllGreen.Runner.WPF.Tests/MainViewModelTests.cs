using AllGreen.WebServer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;
using FluentAssertions;

namespace AllGreen.Runner.WPF.Tests
{

    [TestClass]
    public class MainViewModelTests
    {
        protected TinyIoCContainer _ResourceResolver;
        protected MainViewModel _MainViewModel;
        private IConfiguration _Configuration;

        [TestInitialize]
        public void Setup()
        {
            _ResourceResolver = new TinyIoCContainer();
            _Configuration = Mock.Of<IConfiguration>();
            _ResourceResolver.Register<IConfiguration>(_Configuration);
            _MainViewModel = new MainViewModel(_ResourceResolver);
        }

        [TestCleanup]
        public void TearDown()
        {
            _MainViewModel.Dispose();
        }

        [TestMethod]
        public void PropertyChangedTests()
        {
            TestHelper.TestPropertyChanged(_MainViewModel)
                .Action(() => _MainViewModel.ServerStatus = "NEW STATUS").Changes("ServerStatus")
                .Action(() => _MainViewModel.ConfigurationVisible = !_MainViewModel.ConfigurationVisible).Changes("ConfigurationVisible");

            _MainViewModel.ServerStatus.Should().Be("NEW STATUS");
            _MainViewModel.Configuration.Should().Be(_Configuration);
            _MainViewModel.ConfigurationVisible.Should().BeTrue();
            _MainViewModel.StartServerCommand.Should().NotBeNull();
            _MainViewModel.RunAllTestsCommand.Should().NotBeNull();
            _MainViewModel.ConfigurationCommand.Should().NotBeNull();
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
            _ResourceResolver.Register<IRunnerHub>(mockOfRunnerHub.Object);
            _MainViewModel.RunAllTestsCommand.Execute(null);

            mockOfRunnerHub.Verify(rh => rh.ReloadAll());
        }

        [TestMethod]
        public void ConfigurationCommandShouldMakeConfigurationVisible()
        {
            _MainViewModel.ConfigurationVisible.Should().BeFalse();
            _MainViewModel.ConfigurationCommand.Execute(null);
            _MainViewModel.ConfigurationVisible.Should().BeTrue();
        }
    }
}