using System;
using AllGreen.WebServer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public class SuiteViewModelTests
    {
        SuiteViewModel _SuiteViewModel;

        [TestInitialize]
        public void Setup()
        {
            _SuiteViewModel = new SuiteViewModel();
        }

        [TestMethod]
        public void PropertyChangedTests()
        {
            TestHelper.TestPropertyChanged(_SuiteViewModel)
                .Action(vm => vm.IsExpanded = !vm.IsExpanded).Changes("IsExpanded");
        }

        [TestMethod]
        public void SpecsCollectionChangedTests()
        {
            (new TestHelper.ObservableCollectionTester<SpecViewModel>(_SuiteViewModel.Specs)).RunTests();
        }

        [TestMethod]
        public void SuitesCollectionChangedTests()
        {
            (new TestHelper.ObservableCollectionTester<SuiteViewModel>(_SuiteViewModel.Suites)).RunTests();
        }

        [TestMethod]
        public void IsSuiteTests()
        {
            Guid guid = Guid.NewGuid();
            Suite suite = new Suite()
            {
                Id = guid,
                Name = "Suite 1"
            };
            _SuiteViewModel.IsSuite(suite).Should().BeFalse();
            _SuiteViewModel.Id = guid;
            _SuiteViewModel.Name = "Suite 1";
            _SuiteViewModel.IsSuite(suite).Should().BeTrue();
        }

    }
}
