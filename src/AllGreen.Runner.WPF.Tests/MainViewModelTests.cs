using AllGreen.WebServer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;
using FluentAssertions;
using AllGreen.Runner.WPF.ViewModels;

namespace AllGreen.Runner.WPF.Tests
{

    [TestClass]
    public class MainViewModelTests
    {
        protected TinyIoCContainer _ResourceResolver;
        protected MainViewModel _MainViewModel;
        private IConfiguration _Configuration;
        private IFileViewer _FileViewer;

        [TestInitialize]
        public void Setup()
        {
            _ResourceResolver = new TinyIoCContainer();
            _Configuration = Mock.Of<IConfiguration>();
            _ResourceResolver.Register<IConfiguration>(_Configuration);
            _FileViewer = Mock.Of<IFileViewer>();
            _ResourceResolver.Register<IFileViewer>(_FileViewer);
            _ResourceResolver.Register<IFileLocationParser>(Mock.Of<IFileLocationParser>());
            _ResourceResolver.Register<IFileLocationMapper>(Mock.Of<IFileLocationMapper>());
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
            _MainViewModel.OpenFileCommand.Should().NotBeNull();
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
            Mock<IRunnerClients> mockOfRunnerClients = new Mock<IRunnerClients>();
            _ResourceResolver.Register<IRunnerClients>(mockOfRunnerClients.Object);
            _MainViewModel.RunAllTestsCommand.Execute(null);

            mockOfRunnerClients.Verify(rh => rh.ReloadAll());
        }

        [TestMethod]
        public void ConfigurationCommandShouldMakeConfigurationVisible()
        {
            _MainViewModel.ConfigurationVisible.Should().BeFalse();
            _MainViewModel.ConfigurationCommand.Execute(null);
            _MainViewModel.ConfigurationVisible.Should().BeTrue();
        }

        [TestMethod]
        public void OpenFileCommandShouldOpenFileViewer()
        {
            FileLocation fileLocation = new FileLocation("file", "fullPath", 10, 20);

            _MainViewModel.OpenFileCommand.Execute(fileLocation);

            Mock.Get(_FileViewer).Verify(fv => fv.Open("fullPath", 10, 20));
        }
    }
}