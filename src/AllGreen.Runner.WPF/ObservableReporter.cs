using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using TemplateAttributes;
using TinyIoC;

namespace AllGreen.Runner.WPF
{
    public class ObservableReporter : IReporter
    {
        private Dispatcher _Dispatcher;
        public BindableCollection<RunnerViewModel> Runners { get; private set; }
        public BindableCollection<SuiteViewModel> Suites { get; private set; }

        public ObservableReporter()
        {
            _Dispatcher = Dispatcher.CurrentDispatcher;
            Runners = new BindableCollection<RunnerViewModel>();
            Suites = new BindableCollection<SuiteViewModel>();
        }

        public void Connected(Guid connectionId, string userAgent)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.UserAgent = userAgent;
            runner.Status = "Connected";
        }

        public void Reconnected(Guid connectionId)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.Status = "Reconnected";
        }

        public void Disconnected(Guid connectionId)
        {
            RunnerViewModel runner = Runners.Where(r => r.ConnectionId == connectionId).FirstOrDefault();
            if (runner != null)
            {
                runner.Status = "Disconnected";
                RemoveRunner(runner);
            }
        }

        public void Register(Guid connectionId, string userAgent)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            if (String.IsNullOrEmpty(runner.UserAgent))
                runner.UserAgent = userAgent;
            runner.Status = "Registered";
        }

        public void Reset(Guid connectionId)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.Status = "Reset";
        }

        public void Started(Guid connectionId, int totalTests)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.Status = String.Format("Started running {0} tests", totalTests);
        }

        public void Finished(Guid connectionId)
        {
            RunnerViewModel runner = GetRunner(connectionId);
            runner.Status = "Finished";
        }

        private RunnerViewModel GetRunner(Guid connectionId)
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

        public void SpecUpdated(Guid connectionId, Spec spec)
        {
            _Dispatcher.Invoke(() => GetSpecViewModel(connectionId, spec));
        }

        private void ClearStatuses(Guid connectionId)
        {
            foreach (SuiteViewModel suite in Suites)
            {
                ClearStatuses(suite, connectionId);
            }
        }

        private void ClearStatuses(SuiteViewModel suite, Guid connectionId)
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

        private void ClearStatus(SpecOrSuiteViewModel specOrSuiteViewModel, Guid connectionId)
        {
            specOrSuiteViewModel.ClearStatus(connectionId);
        }

        private SuiteViewModel GetSuiteViewModel(Guid runnerId, Suite suite, UInt64 time)
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
                suiteViewModel = new SuiteViewModel()
                {
                    Id = suite.Id,
                    Name = suite.Name
                };
                suiteViewModel.SetStatus(runnerId, suite.Status, time);
                suites.Add(suiteViewModel);
            }
            else
            {
                suiteViewModel.SetStatus(runnerId, suite.Status, time);
            }
            return suiteViewModel;
        }

        private SpecViewModel GetSpecViewModel(Guid runnerId, Spec spec)
        {
            if (spec == null) return null;

            SuiteViewModel suiteViewModel = GetSuiteViewModel(runnerId, spec.Suite, spec.Time);
            if (suiteViewModel == null) return null;

            SpecViewModel specViewModel = suiteViewModel.Specs.FirstOrDefault(s => s.IsSpec(spec));
            if (specViewModel == null)
            {
                specViewModel = new SpecViewModel()
                {
                    Id = spec.Id,
                    Name = spec.Name,
                    Time = spec.Time
                };
                specViewModel.SetStatus(runnerId, spec.Status, spec.Time);
                suiteViewModel.Specs.Add(specViewModel);
            }
            else
            {
                specViewModel.SetStatus(runnerId, spec.Status, spec.Time);
                specViewModel.Time = spec.Time;
            }
            return specViewModel;
        }
    }
}
