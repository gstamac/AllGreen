using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using TemplateAttributes;
using TinyIoC;
using System.Windows;
using Imports;

namespace AllGreen.Runner.WPF.ViewModels
{
    internal interface IMainViewModelProperties
    {
        string ServerStatus { get; set; }

        IConfiguration Configuration { get; }
        bool ConfigurationVisible { get; set; }

        ICommand StartServerCommand { get; }
        ICommand RunAllTestsCommand { get; }
        ICommand ConfigurationCommand { get; }
        ICommand OpenFileCommand { get; }
    }

    [ImplementPropertyChangedCaliburn(typeof(IMainViewModelProperties))]
    public partial class MainViewModel : IMainViewModelProperties, IDisposable
    {
        public BindableCollection<RunnerViewModel> Runners { get { return _Reporter.Runners; } }
        public BindableCollection<SuiteViewModel> Suites { get { return _Reporter.Suites; } }

        private TinyIoCContainer _ResourceResolver;
        private ObservableReporter _Reporter;
        private IFileViewer _FileViewer;

        private FileWatcher _FileWatcher;

        public MainViewModel(TinyIoCContainer resourceResolver)
        {
            _ResourceResolver = resourceResolver;

            Configuration = _ResourceResolver.Resolve<IConfiguration>();
            _FileViewer = _ResourceResolver.Resolve<IFileViewer>();

            _Reporter = new ObservableReporter(_ResourceResolver.Resolve<IFileLocationParser>(), _ResourceResolver.Resolve<IFileLocationMapper>());

            _ResourceResolver.Register<IReporter>(_Reporter);

            StartServerCommand = new RelayCommand(StartServer);
            RunAllTestsCommand = new RelayCommand(RunAllTests);
            ConfigurationCommand = new RelayCommand(() => ConfigurationVisible = true);
            OpenFileCommand = new RelayCommand<FileLocation>(fl => _FileViewer.Open(fl.FullPath, fl.LineNumber, fl.ColumnNumber));
        }

        //ncrunch: no coverage start
        public void StartServer()
        {
            WebApp.Start(Configuration.ServerUrl, appBuilder => new OwinStartup(_ResourceResolver).Configuration(appBuilder));
            ServerStatus = "Server running at " + Configuration.ServerUrl;

            _FileWatcher = new FileWatcher(_ResourceResolver.Resolve<IRunnerClients>(), Configuration.WatchedFolderFilters.Select(ff => new FolderWatcher(Path.GetFullPath(ff.Folder), ff.FilePattern, ff.IncludeSubfolders)));
        }
        //ncrunch: no coverage end

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