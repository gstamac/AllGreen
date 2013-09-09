using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using TemplateAttributes;
using TinyIoC;
using System.Windows.Input;

namespace AllGreen.Runner.WPF
{
    public interface IMainViewModelProperties
    {
        string ServerStatus { get; set; }
    }

    [ImplementPropertyChangedCaliburn(typeof(IMainViewModelProperties))]
    public partial class MainViewModel : IMainViewModelProperties
    {
        private ObservableReporter _Reporter;
        public BindableCollection<RunnerViewModel> Runners { get { return _Reporter.Runners; } }
        public BindableCollection<SuiteViewModel> Suites { get { return _Reporter.Suites; } }
        public ICommand StartServerCommand { get; set; }

        public MainViewModel()
        {
            _Reporter = new ObservableReporter();
            StartServerCommand = new RelayCommand(StartServer);
        }

        public void StartServer()
        {
            string url = "http://localhost:8080";

            TinyIoCContainer tinyIoCContainer = new TinyIoCContainer();
            tinyIoCContainer.Register<IWebResources>(new EmbededResources(@"AllGreen.WebServer.Resources", Assembly.Load("AllGreen.WebServer.Resources")));
            tinyIoCContainer.Register<IRunnerResources, RunnerResources>();
            tinyIoCContainer.Register<IHubContext>((ioc, np) => GlobalHost.ConnectionManager.GetHubContext<RunnerHub>());
            tinyIoCContainer.Register<IReporter>(_Reporter);

            WebApp.Start(url, appBuilder => new OwinStartup(tinyIoCContainer).Configuration(appBuilder));
            ServerStatus = "Server running at " + url;
        }

    }

    public class RunnersToStatusesConverter : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return null;

            BindableCollection<RunnerViewModel> runners = values[0] as BindableCollection<RunnerViewModel>;
            BindableDictionary<Guid, SpecStatusWithTime> statuses = values[1] as BindableDictionary<Guid, SpecStatusWithTime>;
            if (runners != null)
                return GetRunnerStatuses(runners, statuses);

            return null;
        }

        private IEnumerable<SpecStatusWithTime> GetRunnerStatuses(BindableCollection<RunnerViewModel> runners, BindableDictionary<Guid, SpecStatusWithTime> statuses)
        {
            foreach (RunnerViewModel runner in runners)
            {
                if (statuses.ContainsKey(runner.ConnectionId))
                {
                    SpecStatusWithTime status = statuses[runner.ConnectionId];
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
}
