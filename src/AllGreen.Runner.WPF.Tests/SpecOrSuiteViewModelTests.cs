using System;
using System.Collections.Generic;
using System.Linq;
using AllGreen.WebServer.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Caliburn.Micro;
using AllGreen.Runner.WPF.ViewModels;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public class SpecOrSuiteViewModelTests
    {
        SpecOrSuiteViewModel _SpecOrSuiteViewModel;

        [TestInitialize]
        public void Setup()
        {
            _SpecOrSuiteViewModel = new SpecOrSuiteViewModel();
        }

        [TestMethod]
        public void PropertyChangedTests()
        {
            TestHelper.TestPropertyChanged(_SpecOrSuiteViewModel)
                .Action(vm => vm.Id = Guid.NewGuid()).Changes("Id")
                .Action(vm => vm.Name = "NEW NAME").Changes("Name")
                .Action(vm => vm.Duration = "10 ms").Changes("Duration");
        }

        [TestMethod]
        public void StatusesPropertyChangedTest()
        {
            string guid1 = Guid.NewGuid().ToString();
            string guid2 = Guid.NewGuid().ToString();
            TestHelper.TestCollectionChanged<BindableDictionary<string, SpecStatusViewModel>, KeyValuePair<string, SpecStatusViewModel>>(_SpecOrSuiteViewModel.Statuses)
                .Action(c => c.Add(guid1, CreateSpecStatusViewModel(SpecStatus.Failed, 1, 0))).Adds(CreateKeyValue(guid1, SpecStatus.Failed, 1, 0)).CountIs(1)
                .Action(c => c.Add(guid2, CreateSpecStatusViewModel(SpecStatus.Skipped, 2, 1))).Adds(CreateKeyValue(guid2, SpecStatus.Skipped, 2, 1)).CountIs(2)
                .Action(c => c.Remove(guid2)).Removes(CreateKeyValue(guid2, SpecStatus.Skipped, 2, 1)).CountIs(1)
                .Action(c => c.Clear()).Resets().CountIs(0);
        }

        [TestMethod]
        public void SetStatusTest()
        {
            string guid1 = Guid.NewGuid().ToString();
            string guid2 = Guid.NewGuid().ToString();
            SpecStep[] steps = new SpecStep[] { 
                new SpecStep() { Message = "Step 1 message", Status = SpecStatus.Passed, Trace = "Step 1 trace" },
                new SpecStep() { Message = "Step 2 message", Status = SpecStatus.Failed, Trace = "Step 2 trace" } 
            };
            TestHelper.TestCollectionChanged<BindableDictionary<string, SpecStatusViewModel>, KeyValuePair<string, SpecStatusViewModel>>(_SpecOrSuiteViewModel.Statuses)
                .Action(c => _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Failed, 1)).Adds(CreateKeyValue(guid1, SpecStatus.Failed, 1, 0)).CountIs(1)
                .Action(c => _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Skipped, 2)).Adds(CreateKeyValue(guid2, SpecStatus.Skipped, 2, 0)).CountIs(2)
                .Action(c => _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Skipped, 3, steps, null)).DoesNoChange();

            _SpecOrSuiteViewModel.Statuses.ShouldAllBeEquivalentTo(new KeyValuePair<string, SpecStatusViewModel>[] { 
                CreateKeyValue(guid1, SpecStatus.Skipped, 3, 2, steps),
                CreateKeyValue(guid2, SpecStatus.Skipped, 2, 0)},
                o => o.Excluding(si => si.PropertyPath.EndsWith("IsNotifying")));
        }

        [TestMethod]
        public void ClearStatusTest()
        {
            string guid1 = Guid.NewGuid().ToString();
            string guid2 = Guid.NewGuid().ToString();
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Failed, 1);
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Skipped, 2);
            TestHelper.TestCollectionChanged<BindableDictionary<string, SpecStatusViewModel>, KeyValuePair<string, SpecStatusViewModel>>(_SpecOrSuiteViewModel.Statuses)
                .CountIs(2)
                .Action(c => _SpecOrSuiteViewModel.ClearStatus(guid1)).Removes(CreateKeyValue(guid1, SpecStatus.Failed, 1, 0)).CountIs(1);
        }

        private SpecStatusViewModel CreateSpecStatusViewModel(SpecStatus specStatus, UInt64 time, int duration, SpecStep[] steps = null)
        {
            SpecStatusViewModel vm = new SpecStatusViewModel() { Status = specStatus, Time = time, Duration = duration };
            if (steps != null)
                vm.Steps = new BindableCollection<SpecStepViewModel>(steps.Select(s => new SpecStepViewModel
                {
                    Message = s.Message,
                    Status = s.Status,
                    Trace = new BindableCollection<SpecTraceStepViewModel>(new SpecTraceStepViewModel[] { SpecTraceStepViewModel.Create(s.Trace, null) })
                }));
            return vm;
        }

        private KeyValuePair<string, SpecStatusViewModel> CreateKeyValue(string guid, SpecStatus specStatus, UInt64 time, int duration, SpecStep[] steps = null)
        {
            return new KeyValuePair<string, SpecStatusViewModel>(guid, CreateSpecStatusViewModel(specStatus, time, duration, steps));
        }

        [TestMethod]
        public void ShouldHandleTimeOfStatus()
        {
            string guid1 = Guid.NewGuid().ToString();
            string guid2 = Guid.NewGuid().ToString();
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Undefined, 1);
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Undefined, 2);
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Passed, 5);
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Running, 4);
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Running, 3);
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Passed, 6);
            _SpecOrSuiteViewModel.Statuses[guid1].ShouldBeEquivalentTo(new SpecStatusViewModel() { Status = SpecStatus.Passed, Time = 5, Duration = 4 }, o => o.Excluding(pi => pi.PropertyPath == "Steps"));
            _SpecOrSuiteViewModel.Statuses[guid2].ShouldBeEquivalentTo(new SpecStatusViewModel() { Status = SpecStatus.Passed, Time = 6, Duration = 4 }, o => o.Excluding(pi => pi.PropertyPath == "Steps"));
        }

        [TestMethod]
        public void ShouldCalculateTotalTime()
        {
            string guid1 = Guid.NewGuid().ToString();
            string guid2 = Guid.NewGuid().ToString();
            _SpecOrSuiteViewModel.Duration.Should().Be(null);
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Undefined, 1);
            _SpecOrSuiteViewModel.Duration.Should().Be("0 ms");
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Undefined, 2);
            _SpecOrSuiteViewModel.Duration.Should().Be("0 ms");
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Passed, 5);
            _SpecOrSuiteViewModel.Duration.Should().Be("4 ms");
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Running, 4);
            _SpecOrSuiteViewModel.Duration.Should().Be("6 ms");
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Running, 3);
            _SpecOrSuiteViewModel.Duration.Should().Be("6 ms");
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Passed, 6000);
            _SpecOrSuiteViewModel.Duration.Should().Be("6,002 s");
        }

    }
}
