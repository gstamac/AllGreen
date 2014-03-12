using System;
using AllGreen.Runner.WPF.Core.ViewModels;
using AllGreen.Core;
using Caliburn.Micro;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.Runner.WPF.Core.Tests
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
                .Action(vm => vm.Trace = new BindableCollection<SpecTraceStepViewModel>()).Changes("Trace");
        }

        [TestMethod]
        public void CreateTest()
        {
            SpecStep specStep = new SpecStep
            {
                Message = "Message",
                Status = SpecStatus.Failed,
                Trace = "Trace"
            };

            SpecStepViewModel specStepViewModel = SpecStepViewModel.Create(specStep, Mock.Of<IFileLocationParser>(), Mock.Of<IFileLocationMapper>());
            specStepViewModel.ShouldBeEquivalentTo(specStep, o => o.Excluding(si => si.PropertyPath.EndsWith("IsNotifying") || si.PropertyPath == "Trace" || si.PropertyPath == "MappedLocation"));
            specStepViewModel.Trace.ShouldAllBeEquivalentTo(new object[] { SpecTraceStepViewModel.Create("Trace", null, null) });
        }

        [TestMethod]
        public void CreateAndMapFileLocation()
        {
            SpecStep specStep = new SpecStep
            {
                Message = "Message",
                ErrorLocation = "file.js:10:12",
                Status = SpecStatus.Failed
            };

            FileLocation fileLocation = new FileLocation("file.js", @"C:\content\file.js", 10, 12);

            Mock<IFileLocationParser> fileLocationParserMock = new Mock<IFileLocationParser>();
            fileLocationParserMock.Setup(flp => flp.Parse("file.js:10:12")).Returns(fileLocation);

            Mock<IFileLocationMapper> fileLocationMapperMock = new Mock<IFileLocationMapper>();
            fileLocationMapperMock.Setup(flm => flm.Map(fileLocation)).Returns<FileLocation>(fl => fl);

            SpecStepViewModel specStepViewModel = SpecStepViewModel.Create(specStep, fileLocationParserMock.Object, fileLocationMapperMock.Object);
            specStepViewModel.ShouldBeEquivalentTo(specStep, o => o.Excluding(si => si.PropertyPath.EndsWith("IsNotifying") || si.PropertyPath == "Trace" || si.PropertyPath == "ErrorLocation" || si.PropertyPath == "MappedLocation"));
            specStepViewModel.ErrorLocation.ShouldBeEquivalentTo(new { Filename = "file.js", FullPath = @"C:\content\file.js", LineNumber = 10, ColumnNumber = 12 });
            specStepViewModel.MappedLocation.ShouldBeEquivalentTo(new { Filename = "file.js", FullPath = @"C:\content\file.js", LineNumber = 10, ColumnNumber = 12 });
        }

        [TestMethod]
        public void CreateWithMultilineTrace()
        {
            SpecStep specStep = new SpecStep
            {
                Trace = "Trace line 1\nTrace line 2"
            };

            SpecStepViewModel specStepViewModel = SpecStepViewModel.Create(specStep, Mock.Of<IFileLocationParser>(), null);
            specStepViewModel.Trace.ShouldAllBeEquivalentTo(new object[] { 
                SpecTraceStepViewModel.Create("Trace line 1", null, null), SpecTraceStepViewModel.Create("Trace line 2", null, null) });
        }
    }
}
