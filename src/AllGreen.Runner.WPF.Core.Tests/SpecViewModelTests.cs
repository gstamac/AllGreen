using System;
using AllGreen.Core;
using AllGreen.Runner.WPF.Core.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.Runner.WPF.Core.Tests
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

        [TestMethod]
        public void CreateTest()
        {
            Guid specId = Guid.NewGuid();
            Spec spec = new Spec()
            {
                Id = specId,
                Name = "Spec 1",
                Status = SpecStatus.Passed,
                Suite = null,
                Time = 100,
                Steps = new SpecStep[] 
                    { 
                        new SpecStep { Message = "Step 1", Status = SpecStatus.Running, Trace = "Step 1 Trace line 1\nTrace line 2\nTrace line 3" },
                        new SpecStep { Message = "Step 2", Status = SpecStatus.Failed, Trace = "Step 2 Trace line 1\nTrace line 2\nTrace line 3" },
                    }
            };

            SpecViewModel.Create(spec).ShouldBeEquivalentTo(spec, o => o
                .Excluding(si => si.PropertyPath.EndsWith("IsNotifying"))
                .Excluding(si => si.PropertyPath == "Statuses")
                .Excluding(si => si.PropertyPath == "Duration"));
        }
    }
}
