using System;
using AllGreen.Runner.WPF.ViewModels;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public class SpecStepViewModelTests
    {
        [TestMethod]
        public void PropertyChangedTests()
        {
            TestHelper.TestPropertyChanged(new SpecStepViewModel())
                .Action(vm => vm.Message = "new message").Changes("Message")
                .Action(vm => vm.Status = SpecStatus.Passed).Changes("Status")
                .Action(vm => vm.ScriptLocation = new FileLocation("", "", 0)).Changes("ScriptLocation")
                .Action(vm => vm.Trace = new BindableCollection<SpecTraceStepViewModel>()).Changes("Trace");
        }

        [TestMethod]
        public void CreateTest()
        {
            SpecStep specStep = new SpecStep
            {
                Message = "Message",
                Status = SpecStatus.Failed,
                Filename = "file.js",
                LineNumber = 11,
                Trace = "Trace"
            };

            Mock<IFileLocationMapper> fileLocationMapperMock = new Mock<IFileLocationMapper>();
            fileLocationMapperMock.Setup(flm => flm.Map(It.IsAny<string>(), It.IsAny<int>())).Returns<string, int>((fn, ln) => new FileLocation(fn, "", ln));

            SpecStepViewModel specStepViewModel = SpecStepViewModel.Create(specStep, fileLocationMapperMock.Object);
            specStepViewModel.ShouldBeEquivalentTo(specStep, o => o.Excluding(si => si.PropertyPath.EndsWith("IsNotifying") || si.PropertyPath == "Trace" || si.PropertyPath == "ScriptLocation"));
            specStepViewModel.ScriptLocation.ShouldBeEquivalentTo(new { Filename = "file.js", FullPath = "", LineNumber = 11 });
            specStepViewModel.Trace.ShouldAllBeEquivalentTo(new object[] { SpecTraceStepViewModel.Create("Trace", null) });
        }

        [TestMethod]
        public void CreateWithMultilineTrace()
        {
            SpecStep specStep = new SpecStep
            {
                Trace = "Trace line 1\nTrace line 2"
            };

            SpecStepViewModel specStepViewModel = SpecStepViewModel.Create(specStep, null);
            specStepViewModel.Trace.ShouldAllBeEquivalentTo(new object[] { SpecTraceStepViewModel.Create("Trace line 1", null), SpecTraceStepViewModel.Create("Trace line 2", null) });
        }
    }
}
