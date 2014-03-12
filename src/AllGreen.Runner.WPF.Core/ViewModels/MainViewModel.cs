using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using AllGreen.Core;
using Caliburn.Micro;
using TemplateAttributes;
using TinyIoC;
using System.Windows;
using Imports;

namespace AllGreen.Runner.WPF.Core.ViewModels
{
    [ImplementPropertyChangedCaliburn(typeof(IMainViewModelProperties))]
    public partial class MainViewModel : IDisposable
    {
        interface IMainViewModelProperties
        {
            string ServerStatus { get; set; }

            IConfiguration Configuration { get; }
            bool ConfigurationVisible { get; set; }

            ICommand StartServerCommand { get; }
            ICommand RunAllTestsCommand { get; }
            ICommand CopyServerUrlCommand { get; }
            ICommand ConfigurationCommand { get; }
            ICommand OpenFileCommand { get; }
        }

        public BindableCollection<RunnerViewModel> Runners { get { return _Reporter.Runners; } }
        public BindableCollection<SuiteViewModel> Suites { get { return _Reporter.Suites; } }

        private TinyIoCContainer _ResourceResolver;
        private ObservableReporter _Reporter;
        private IFileViewer _FileViewer;

        public MainViewModel(TinyIoCContainer resourceResolver)
        {
            _ResourceResolver = resourceResolver;

            Configuration = _ResourceResolver.Resolve<IConfiguration>();
            _FileViewer = _ResourceResolver.Resolve<IFileViewer>();

            _Reporter = new ObservableReporter(_ResourceResolver.Resolve<IFileLocationParser>(), _ResourceResolver.Resolve<IFileLocationMapper>());

            _ResourceResolver.Register<IReporter>(_Reporter);

            StartServerCommand = new RelayCommand(StartServer);
            RunAllTestsCommand = new RelayCommand(RunAllTests);
            CopyServerUrlCommand = new RelayCommand(() => Clipboard.SetText(Configuration.ServerUrl));
            ConfigurationCommand = new RelayCommand(() => ConfigurationVisible = true);
            OpenFileCommand = new RelayCommand<FileLocation>(fl => _FileViewer.Open(fl.FullPath, fl.LineNumber, fl.ColumnNumber));
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
        private void StartServer()
        {
            _ResourceResolver.Resolve<IServerStarter>().Start();
            ServerStatus = "Server running at " + Configuration.ServerUrl;
        }

        private void RunAllTests()
        {
            IRunnerClients runnerClients = _ResourceResolver.Resolve<IRunnerClients>();
            runnerClients.ReloadAll();
        }

        public void Dispose()
        {
            _ResourceResolver.Dispose();
        }
    }
}