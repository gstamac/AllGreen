using System;
using AllGreen.WebServer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Collections.Generic;

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
                .Action(vm => vm.Name = "NEW NAME").Changes("Name");
        }

        [TestMethod]
        public void StatusesPropertyChangedTest()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            TestHelper.TestCollectionChanged<BindableDictionary<Guid, SpecStatusWithTime>, KeyValuePair<Guid, SpecStatusWithTime>>(_SpecOrSuiteViewModel.Statuses)
                .Action(c => c.Add(guid1, CreateStatusWithTime(SpecStatus.Failed, 1))).Adds(CreateKeyValue(guid1, SpecStatus.Failed, 1)).CountIs(1)
                .Action(c => c.Add(guid2, CreateStatusWithTime(SpecStatus.Skipped, 2))).Adds(CreateKeyValue(guid2, SpecStatus.Skipped, 2)).CountIs(2)
                .Action(c => c.Remove(guid2)).Removes(CreateKeyValue(guid2, SpecStatus.Skipped, 2)).CountIs(1)
                .Action(c => c.Clear()).Resets().CountIs(0);
        }

        [TestMethod]
        public void SetStatusTest()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            TestHelper.TestCollectionChanged<BindableDictionary<Guid, SpecStatusWithTime>, KeyValuePair<Guid, SpecStatusWithTime>>(_SpecOrSuiteViewModel.Statuses)
                .Action(c => _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Failed, 1)).Adds(CreateKeyValue(guid1, SpecStatus.Failed, 1)).CountIs(1)
                .Action(c => _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Skipped, 2)).Adds(CreateKeyValue(guid2, SpecStatus.Skipped, 2)).CountIs(2)
                .Action(c => _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Skipped, 3)).Replaces(CreateKeyValue(guid1, SpecStatus.Failed, 1), CreateKeyValue(guid1, SpecStatus.Skipped, 3)).CountIs(2);
        }

        [TestMethod]
        public void ClearStatusTest()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Failed, 1);
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Skipped, 2);
            TestHelper.TestCollectionChanged<BindableDictionary<Guid, SpecStatusWithTime>, KeyValuePair<Guid, SpecStatusWithTime>>(_SpecOrSuiteViewModel.Statuses)
                .CountIs(2)
                .Action(c => _SpecOrSuiteViewModel.ClearStatus(guid1)).Removes(CreateKeyValue(guid1, SpecStatus.Failed, 1)).CountIs(1);
        }

        private SpecStatusWithTime CreateStatusWithTime(SpecStatus specStatus, UInt64 time)
        {
            return new SpecStatusWithTime() { Status = specStatus, Time = time };
        }

        private KeyValuePair<Guid, SpecStatusWithTime> CreateKeyValue(Guid guid, SpecStatus specStatus, UInt64 time)
        {
            return new KeyValuePair<Guid, SpecStatusWithTime>(guid, CreateStatusWithTime(specStatus, time));
        }

        [TestMethod]
        public void ShouldHandleTimeOfStatus()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Undefined, 1);
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Undefined, 2);
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Passed, 5);
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Running, 4);
            _SpecOrSuiteViewModel.SetStatus(guid1, SpecStatus.Running, 3);
            _SpecOrSuiteViewModel.SetStatus(guid2, SpecStatus.Passed, 6);
            _SpecOrSuiteViewModel.Statuses[guid1].ShouldBeEquivalentTo(new SpecStatusWithTime() { Status = SpecStatus.Passed, Time = 5 });
            _SpecOrSuiteViewModel.Statuses[guid2].ShouldBeEquivalentTo(new SpecStatusWithTime() { Status = SpecStatus.Passed, Time = 6 });
        }
    }
}
