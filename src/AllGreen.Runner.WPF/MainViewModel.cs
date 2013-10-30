using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using Microsoft.Owin.Hosting;
using TemplateAttributes;
using TinyIoC;

namespace AllGreen.Runner.WPF
{
    public interface IMainViewModelProperties
    {
        string ServerStatus { get; set; }

        IConfiguration Configuration { get; }
        bool ConfigurationVisible { get; set; }

        ICommand StartServerCommand { get; }
        ICommand RunAllTestsCommand { get; }
        ICommand ConfigurationCommand { get; }
    }

    [ImplementPropertyChangedCaliburn(typeof(IMainViewModelProperties))]
    public partial class MainViewModel : IMainViewModelProperties, IDisposable
    {
        public BindableCollection<RunnerViewModel> Runners { get { return _Reporter.Runners; } }
        public BindableCollection<SuiteViewModel> Suites { get { return _Reporter.Suites; } }

        private TinyIoCContainer _ResourceResolver;
        private ObservableReporter _Reporter;

        private FileWatcher _FileWatcher;

        public MainViewModel(TinyIoCContainer resourceResolver)
        {
            _Reporter = new ObservableReporter();
            StartServerCommand = new RelayCommand(StartServer);
            RunAllTestsCommand = new RelayCommand(RunAllTests);
            ConfigurationCommand = new RelayCommand(() => ConfigurationVisible = true);

            _ResourceResolver = resourceResolver;
            _ResourceResolver.Register<IReporter>(_Reporter);

            Configuration = _ResourceResolver.Resolve<IConfiguration>();
        }

        //ncrunch: no coverage start
        public void StartServer()
        {
            WebApp.Start(Configuration.ServerUrl, appBuilder => new OwinStartup(_ResourceResolver).Configuration(appBuilder));
            ServerStatus = "Server running at " + Configuration.ServerUrl;

            _FileWatcher = new FileWatcher(_ResourceResolver.Resolve<IRunnerHub>(), Configuration.WatchedFolderFilters);
        }
        //ncrunch: no coverage end

        private void RunAllTests()
        {
            IRunnerHub runnerHub = _ResourceResolver.Resolve<IRunnerHub>();
            runnerHub.ReloadAll();
        }

        public void Dispose()
        {
            _ResourceResolver.Dispose();
        }
    }

}
