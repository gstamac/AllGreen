using System;
using AllGreen.Runner.WPF.Core.ViewModels;
using AllGreen.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TemplateAttributes;

namespace AllGreen.Runner.WPF.Core.Tests
{
    [TestClass]
    public partial class SpecTraceStepViewModelTests
    {
        /*
        Firefox:
            level2@http://localhost:8080/Test/testScript.js:28
            level1@http://localhost:8080/Test/testScript.js:24
            @http://localhost:8080/Test/testScript.js:34
        IE 9: no trace
        IE 10,11:
            Error: Something went terible wrong
               at level2 (http://localhost:8080/Test/testScript.js:28:9)
               at level1 (http://localhost:8080/Test/testScript.js:24:9)
               at Anonymous function (http://localhost:8080/Test/testScript.js:34:17)
         Chrome:
            at level2 (http://localhost:8080/Test/testScript.js:28:15)
            at level1 (http://localhost:8080/Test/testScript.js:24:16)
            at null.<anonymous> (http://localhost:8080/Test/testScript.js:34:24)
        */
        [DataTestMethod(@"level2@http://localhost:8080/Test/testScript.js:28", @"http://localhost:8080/Test/testScript.js:28", @"level2", @"/Test/testScript.js", 28)]
        [DataTestMethod(@"level1@http://localhost:8080/Test/testScript.js:24", @"http://localhost:8080/Test/testScript.js:24", @"level1", @"/Test/testScript.js", 24)]
        [DataTestMethod(@"@http://localhost:8080/Test/testScript.js:34", @"http://localhost:8080/Test/testScript.js:34", "<anonymous>", @"/Test/testScript.js", 34)]
        public void ParseFirefoxTrace(string message, string fileLocationText, string methodName, string filename, int lineNumber)
        {
            FileLocation fileLocation = new FileLocation(filename, "", lineNumber);

            Mock<IFileLocationParser> fileLocationParserMock = new Mock<IFileLocationParser>();
            fileLocationParserMock.Setup(flf => flf.Parse(fileLocationText)).Returns(fileLocation);

            Mock<IFileLocationMapper> fileLocationMapperMock = new Mock<IFileLocationMapper>();
            fileLocationMapperMock.Setup(flm => flm.Map(fileLocation)).Returns<FileLocation>(fl => fl);

            SpecTraceStepViewModel specTraceStepViewModel = SpecTraceStepViewModel.Create(message, fileLocationParserMock.Object, fileLocationMapperMock.Object);

            specTraceStepViewModel.Message.Should().Be(message);
            specTraceStepViewModel.MethodName.Should().Be(methodName);
            specTraceStepViewModel.ScriptLocation.Filename.Should().Be(filename);
            specTraceStepViewModel.ScriptLocation.LineNumber.Should().Be(lineNumber);
            specTraceStepViewModel.ScriptLocation.ColumnNumber.Should().Be(0);
            specTraceStepViewModel.MappedLocation.Filename.Should().Be(filename);
            specTraceStepViewModel.MappedLocation.LineNumber.Should().Be(lineNumber);
            specTraceStepViewModel.MappedLocation.ColumnNumber.Should().Be(0);
        }

        [DataTestMethod(@"   at level2 (http://localhost:8080/Test/testScript.js:28:9)", @"http://localhost:8080/Test/testScript.js:28:9", @"level2", @"/Test/testScript.js", 28, 9)]
        [DataTestMethod(@"   at level1 (http://localhost:8080/Test/testScript.js:24:9)", @"http://localhost:8080/Test/testScript.js:24:9", @"level1", @"/Test/testScript.js", 24, 9)]
        [DataTestMethod(@"   at Anonymous function (http://localhost:8080/Test/testScript.js:34:17)", @"http://localhost:8080/Test/testScript.js:34:17", "Anonymous function", @"/Test/testScript.js", 34, 17)]
        public void ParseIETrace(string message, string fileLocationText, string methodName, string filename, int lineNumber, int columnNumber)
        {
            FileLocation fileLocation = new FileLocation(filename, "", lineNumber, columnNumber);

            Mock<IFileLocationParser> fileLocationParserMock = new Mock<IFileLocationParser>();
            fileLocationParserMock.Setup(flf => flf.Parse(fileLocationText)).Returns(fileLocation);

            Mock<IFileLocationMapper> fileLocationMapperMock = new Mock<IFileLocationMapper>();
            fileLocationMapperMock.Setup(flm => flm.Map(fileLocation)).Returns<FileLocation>(fl => fl);

            SpecTraceStepViewModel specTraceStepViewModel = SpecTraceStepViewModel.Create(message, fileLocationParserMock.Object, fileLocationMapperMock.Object);

            specTraceStepViewModel.Message.Should().Be(message);
            specTraceStepViewModel.MethodName.Should().Be(methodName);
            specTraceStepViewModel.ScriptLocation.Filename.Should().Be(filename);
            specTraceStepViewModel.ScriptLocation.LineNumber.Should().Be(lineNumber);
            specTraceStepViewModel.ScriptLocation.ColumnNumber.Should().Be(columnNumber);
            specTraceStepViewModel.MappedLocation.Filename.Should().Be(filename);
            specTraceStepViewModel.MappedLocation.LineNumber.Should().Be(lineNumber);
            specTraceStepViewModel.MappedLocation.ColumnNumber.Should().Be(columnNumber);
        }

        [TestMethod]
        public void ParseJavascriptTraceWithoutMapping()
        {
            const string message = @"jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:114";
            const string filename = @"/Scripts/jasmine.js";
            const int lineNumber = 114;
            
            FileLocation fileLocation = new FileLocation(filename, "", lineNumber);

            Mock<IFileLocationParser> fileLocationParserMock = new Mock<IFileLocationParser>();
            fileLocationParserMock.Setup(flf => flf.Parse(@"http://localhost:8080/Scripts/jasmine.js:114")).Returns(fileLocation);

            Mock<IFileLocationMapper> fileLocationMapperMock = new Mock<IFileLocationMapper>();
            fileLocationMapperMock.Setup(flm => flm.Map(fileLocation)).Returns<FileLocation>(null);

            SpecTraceStepViewModel specTraceStepViewModel = SpecTraceStepViewModel.Create(message, fileLocationParserMock.Object, fileLocationMapperMock.Object);

            specTraceStepViewModel.Message.Should().Be(message);
            specTraceStepViewModel.MethodName.Should().Be(@"jasmine.ExpectationResult");
            specTraceStepViewModel.ScriptLocation.Filename.Should().Be(filename);
            specTraceStepViewModel.ScriptLocation.LineNumber.Should().Be(lineNumber);
            specTraceStepViewModel.MappedLocation.Should().BeNull();
        }

        [DataTestMethod(@"http://localhost:8080/Client/testScript.js:9")]
        public void ParseIncorrectJavascriptTrace(string message)
        {
            SpecTraceStepViewModel specTraceStepViewModel = SpecTraceStepViewModel.Create(message, Mock.Of<IFileLocationParser>(), Mock.Of<IFileLocationMapper>());

            specTraceStepViewModel.Message.Should().Be(message);
            specTraceStepViewModel.MethodName.Should().BeNull();
            specTraceStepViewModel.ScriptLocation.Should().BeNull();
        }
    }
}
