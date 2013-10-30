using System;
using System.Linq;
using System.Windows.Data;
using AllGreen.WebServer.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void ChildrenTest()
        {
            SpecViewModel spec = new SpecViewModel();
            _SuiteViewModel.Specs.Add(spec);
            SuiteViewModel suite = new SuiteViewModel();
            _SuiteViewModel.Suites.Add(suite);
            ((CollectionContainer)_SuiteViewModel.Children[0]).Collection.Cast<object>()
                .Union(((CollectionContainer)_SuiteViewModel.Children[1]).Collection.Cast<object>())
                .ShouldAllBeEquivalentTo(new object[] { spec, suite });
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
