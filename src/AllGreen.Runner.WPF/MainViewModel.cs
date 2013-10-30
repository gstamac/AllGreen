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
    }

    [ImplementPropertyChangedCaliburn(typeof(IMainViewModelProperties))]
    public partial class MainViewModel : IMainViewModelProperties, IDisposable
    {
        public TinyIoCContainer ResourceResolver { get; private set; }
        private ObservableReporter _Reporter; 
        public BindableCollection<RunnerViewModel> Runners { get { return _Reporter.Runners; } }
        public BindableCollection<SuiteViewModel> Suites { get { return _Reporter.Suites; } }
        public ICommand StartServerCommand { get; set; }
        public ICommand RunAllTestsCommand { get; private set; }

        FileWatcher _FileWatcher;

        public MainViewModel() // TODO: provide IoC as param and override App.xaml Startup
        {
            _Reporter = new ObservableReporter();
            StartServerCommand = new RelayCommand(StartServer);
            RunAllTestsCommand = new RelayCommand(RunAllTests);

            ResourceResolver = new TinyIoCContainer();
            ResourceResolver.Register<IConfiguration>(new XmlConfiguration(""));
            ResourceResolver.Register<IReporter>(_Reporter);
        }

        public void StartServer()
        {
            Bootstrapper.RegisterServices(ResourceResolver);

            string url = "http://localhost:8080";

            WebApp.Start(url, appBuilder => new OwinStartup(ResourceResolver).Configuration(appBuilder));
            ServerStatus = "Server running at " + url;

            IConfiguration configuration = ResourceResolver.Resolve<IConfiguration>();
            _FileWatcher = new FileWatcher(ResourceResolver.Resolve<IRunnerHub>(), configuration.WatchedFolderFilters);
        }

        private void RunAllTests()
        {
            IRunnerHub runnerHub = ResourceResolver.Resolve<IRunnerHub>();
            runnerHub.ReloadAll();
        }

        public void Dispose()
        {
            ResourceResolver.Dispose();
        }
    }

    public class RunnersToStatusesConverter : MarkupExtension, IMultiValueConverter
    {
        public RunnersToStatusesConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return null;

            BindableCollection<RunnerViewModel> runners = values[0] as BindableCollection<RunnerViewModel>;
            BindableDictionary<string, SpecStatusViewModel> statuses = values[1] as BindableDictionary<string, SpecStatusViewModel>;
            if (runners != null)
                return GetRunnerStatuses(runners, statuses);

            return null;
        }

        private IEnumerable<SpecStatusViewModel> GetRunnerStatuses(BindableCollection<RunnerViewModel> runners, BindableDictionary<string, SpecStatusViewModel> statuses)
        {
            foreach (RunnerViewModel runner in runners)
            {
                if (statuses.ContainsKey(runner.ConnectionId))
                {
                    SpecStatusViewModel status = statuses[runner.ConnectionId];
                    status.Runner = runner;
                    yield return status;
                }
                else
                    yield return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class SpecStatusToImageConverter : MarkupExtension, IValueConverter
    {
        public SpecStatusToImageConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((SpecStatus)value)
            {
                case SpecStatus.Undefined:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/help.png", UriKind.Relative));
                case SpecStatus.Running:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/refresh.png", UriKind.Relative));
                case SpecStatus.Failed:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/delete.png", UriKind.Relative));
                case SpecStatus.Passed:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/check.png", UriKind.Relative));
                case SpecStatus.Skipped:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/pause.png", UriKind.Relative));
                default:
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri("icons/help.png", UriKind.Relative));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

}
