using System;
using AllGreen.WebServer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public class SpecViewModelTests
    {
        SpecViewModel _SpecViewModel;

        [TestInitialize]
        public void Setup()
        {
            _SpecViewModel = new SpecViewModel();
        }

        [TestMethod]
        public void PropertyChangedTests()
        {
            TestHelper.TestPropertyChanged(_SpecViewModel)
                .Action(vm => vm.Time = 200).Changes("Time");
        }

        [TestMethod]
        public void SpecStepPropertyChangedTest()
        {
        }

        [TestMethod]
        public void IsSpecTests()
        {
            Guid guid = Guid.NewGuid();
            Spec spec = new Spec()
            {
                Id = guid,
                Name = "Test 1"
            };
            _SpecViewModel.IsSpec(spec).Should().BeFalse();
            _SpecViewModel.Id = guid;
            _SpecViewModel.Name = "Test 1";
            _SpecViewModel.IsSpec(spec).Should().BeTrue();
        }
    }
}
