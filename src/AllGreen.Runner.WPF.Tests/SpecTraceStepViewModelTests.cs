using System;
using AllGreen.Runner.WPF.ViewModels;
using AllGreen.WebServer.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplateAttributes;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public partial class SpecTraceStepViewModelTests
    {
        [DataTestMethod(@"jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:114", @"jasmine.ExpectationResult", @"/Scripts/jasmine.js", 114)]
        [DataTestMethod(@"jasmine.Matchers.matcherFn_/<@http://localhost:8080/Scripts/jasmine.js:1240", @"jasmine.Matchers.matcherFn_/<", @"/Scripts/jasmine.js", 1240)]
        [DataTestMethod(@"@http://localhost:8080/Client/testScript.js:9", "", @"/Client/testScript.js", 9)]
        public void ParseJavascriptTrace(string message, string methodName, string filename, int lineNumber)
        {
            Mock<IFileLocationMapper> fileLocationMapperMock = new Mock<IFileLocationMapper>();
            fileLocationMapperMock.Setup(flm => flm.Map(It.IsAny<string>(), It.IsAny<int>())).Returns<string, int>((fn, ln) => new FileLocation(fn.Replace("http://localhost:8080", ""), "", ln));

            SpecTraceStepViewModel specTraceStepViewModel = SpecTraceStepViewModel.Create(message, fileLocationMapperMock.Object);

            specTraceStepViewModel.Message.Should().Be(message);
            specTraceStepViewModel.MethodName.Should().Be(methodName);
            specTraceStepViewModel.ScriptLocation.Filename.Should().Be(filename);
            specTraceStepViewModel.ScriptLocation.LineNumber.Should().Be(lineNumber);
        }

        [DataTestMethod(@"http://localhost:8080/Client/testScript.js:9")]
        [DataTestMethod(@"@http://localhost:8080/Client/testScript.js.9")]
        [DataTestMethod(@"jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:X")]
        public void ParseIncorrectJavascriptTrace(string message)
        {
            SpecTraceStepViewModel specTraceStepViewModel = SpecTraceStepViewModel.Create(message, null);

            specTraceStepViewModel.Message.Should().Be(message);
            specTraceStepViewModel.MethodName.Should().BeNull();
            specTraceStepViewModel.ScriptLocation.Should().BeNull();
        }
    }
}
