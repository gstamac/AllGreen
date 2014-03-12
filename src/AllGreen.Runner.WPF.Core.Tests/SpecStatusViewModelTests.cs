using System;
using System.Collections.Generic;
using AllGreen.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Caliburn.Micro;
using AllGreen.Runner.WPF.Core.ViewModels;

namespace AllGreen.Runner.WPF.Core.Tests
{
    [TestClass]
    public class SpecStatusViewModelTests
    {
        [TestMethod]
        public void PropertyChangedTests()
        {
            TestHelper.TestPropertyChanged(new SpecStatusViewModel())
                .Action(vm => vm.Status = SpecStatus.Passed).Changes("Status").Changes("Description")
                .Action(vm => vm.Time = 3000).Changes("Time")
                .Action(vm => vm.Duration = 773).Changes("Duration").Changes("DurationText").Changes("Description")
                .Action(vm => vm.Runner = new RunnerViewModel()).Changes("Runner")
                .Action(vm => vm.Steps = new BindableCollection<SpecStepViewModel>()).Changes("Steps").Changes("Description");
        }

        [TestMethod]
        public void DurationText()
        {
            SpecStatusViewModel specStatusViewModel = new SpecStatusViewModel()
            {
                Status = SpecStatus.Passed,
                Time = 10,
                Duration = 11
            };
            specStatusViewModel.DurationText.Should().Be("11 ms");

            specStatusViewModel = new SpecStatusViewModel
            {
                Status = SpecStatus.Passed,
                Time = 10,
                Duration = 11123
            };
            specStatusViewModel.DurationText.Should().Be("11,123 s");
        }

        [TestMethod]
        public void DescriptionTest()
        {
            SpecStatusViewModel specStatusViewModel = new SpecStatusViewModel()
            {
                Status = SpecStatus.Passed,
                Time = 10,
                Duration = 11
            };
            specStatusViewModel.Description.Replace("\r", "").Should().Be("Passed in 11 ms");

            specStatusViewModel.Steps = new BindableCollection<SpecStepViewModel>(new SpecStepViewModel[] { 
                    new SpecStepViewModel { Message = "Step 1 message", Status = SpecStatus.Passed, 
                        Trace = new BindableCollection<SpecTraceStepViewModel>(new SpecTraceStepViewModel[] { SpecTraceStepViewModel.Create( "Step 1 trace", null, null), SpecTraceStepViewModel.Create("Step 1 trace continued", null, null)}) },
                    new SpecStepViewModel { Message = "Step 2 message", Status = SpecStatus.Failed, 
                        Trace = new BindableCollection<SpecTraceStepViewModel>(new SpecTraceStepViewModel[]{SpecTraceStepViewModel.Create("Step 2 trace", null, null)}) }
                });

            specStatusViewModel.Description.Replace("\r", "").Should().Be("Passed in 11 ms\nStep 1 message Passed\nStep 1 trace\nStep 1 trace continued\nStep 2 message Failed\nStep 2 trace");

            specStatusViewModel = new SpecStatusViewModel
            {
                Status = SpecStatus.Passed,
                Time = 10,
                Duration = 11123
            };
            specStatusViewModel.Description.Should().Be("Passed in 11,123 s");
        }
    }
}
