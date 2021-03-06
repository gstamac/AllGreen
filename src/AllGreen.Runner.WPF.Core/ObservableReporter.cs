using System;
using System.Linq;
using System.Windows.Threading;
using AllGreen.Core;
using AllGreen.Runner.WPF.Core.ViewModels;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF.Core
{
    public class ObservableReporter : IReporter
    {
        private Dispatcher _Dispatcher;
        IFileLocationParser _FileLocationParser;
        IFileLocationMapper _FileLocationMapper;

        public BindableCollection<RunnerViewModel> Runners { get; private set; }
        public BindableCollection<SuiteViewModel> Suites { get; private set; }

        public ObservableReporter(IFileLocationParser fileLocationParser, IFileLocationMapper fileLocationMapper)
        {
            _Dispatcher = Dispatcher.CurrentDispatcher;
            _FileLocationParser = fileLocationParser;
            _FileLocationMapper = fileLocationMapper;
            Runners = new BindableCollection<RunnerViewModel>();
            Suites = new BindableCollection<SuiteViewModel>();
        }

        public void Connected(string connectionId, string userAgent)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.UserAgent = userAgent;
            runner.Status = "Connected";
        }

        public void Reconnected(string connectionId, string userAgent)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.UserAgent = userAgent;
            runner.Status = "Reconnected";
        }

        public void Disconnected(string connectionId)
        {
            RunnerViewModel runner = Runners.Where(r => r.ConnectionId == connectionId).FirstOrDefault();
            if (runner != null)
            {
                runner.Status = "Disconnected";
                RemoveRunner(runner);
            }
        }

        public void Register(string connectionId, string userAgent)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            if (String.IsNullOrEmpty(runner.UserAgent))
                runner.UserAgent = userAgent;
            runner.Status = "Registered";
        }

        public void Reset(string connectionId)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.Status = "Reset";
            _Dispatcher.Invoke(() => ClearStatuses(runner.ConnectionId));
        }

        public void Started(string connectionId, int totalTests)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.Status = String.Format("Started running {0} tests", totalTests);
        }

        public void Finished(string connectionId)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.Status = "Finished";
        }

        private RunnerViewModel GetRunner(string connectionId)
        {
            RunnerViewModel runner = Runners.Where(r => r.ConnectionId == connectionId).FirstOrDefault();
            if (runner == null)
            {
                runner = new RunnerViewModel() { ConnectionId = connectionId };
                AddRunner(runner);
            }
            return runner;
        }


        private void AddRunner(RunnerViewModel runner)
        {
            _Dispatcher.Invoke(() => Runners.Add(runner));
        }

        private void RemoveRunner(RunnerViewModel runner)
        {
            _Dispatcher.Invoke(() => Runners.Remove(runner));
            _Dispatcher.Invoke(() => ClearStatuses(runner.ConnectionId));
        }

        public void SpecUpdated(string connectionId, Spec spec)
        {
            _Dispatcher.Invoke(() => GetSpecViewModel(connectionId, spec));
        }

        private void ClearStatuses(string connectionId)
        {
            foreach (SuiteViewModel suite in Suites)
            {
                ClearStatuses(suite, connectionId);
            }
        }

        private void ClearStatuses(SuiteViewModel suite, string connectionId)
        {
            ClearStatus(suite, connectionId);
            foreach (SuiteViewModel childSuite in suite.Suites)
            {
                ClearStatuses(childSuite, connectionId);
            }
            foreach (SpecViewModel spec in suite.Specs)
            {
                ClearStatus(spec, connectionId);
            }
        }

        private void ClearStatus(SpecOrSuiteViewModel specOrSuiteViewModel, string connectionId)
        {
            specOrSuiteViewModel.ClearStatus(connectionId);
        }

        private SuiteViewModel GetSuiteViewModel(string runnerId, Suite suite, UInt64 time)
        {
            if (suite == null) return null;

            SuiteViewModel suiteViewModel = null;
            BindableCollection<SuiteViewModel> suites;

            if (suite.ParentSuite == null)
            {
                suites = Suites;
            }
            else
            {
                suites = GetSuiteViewModel(runnerId, suite.ParentSuite, time).Suites;
            }

            suiteViewModel = suites.FirstOrDefault(s => s.IsSuite(suite));
            if (suiteViewModel == null)
            {
                suiteViewModel = SuiteViewModel.Create(suite);
                suites.Add(suiteViewModel);
            }
            suiteViewModel.SetStatus(runnerId, suite.Status, time);
            return suiteViewModel;
        }

        private SpecViewModel GetSpecViewModel(string runnerId, Spec spec)
        {
            if (spec == null) return null;

            SuiteViewModel suiteViewModel = GetSuiteViewModel(runnerId, spec.Suite, spec.Time);
            if (suiteViewModel == null) return null;

            SpecViewModel specViewModel = suiteViewModel.Specs.FirstOrDefault(s => s.IsSpec(spec));
            if (specViewModel == null)
            {
                specViewModel = SpecViewModel.Create(spec);
                suiteViewModel.Specs.Add(specViewModel);
            }
            else
            {
                specViewModel.Time = spec.Time;
            }
            specViewModel.SetStatus(runnerId, spec.Status, spec.Time, spec.Steps, _FileLocationParser, _FileLocationMapper);
            return specViewModel;
        }
    }
}