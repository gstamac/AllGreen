using AllGreen.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TinyIoC;
using FluentAssertions;
using AllGreen.Runner.WPF.Core.ViewModels;
using TemplateAttributes;

namespace AllGreen.Runner.WPF.Core.Tests
{
    [TestClass]
    public partial class JsMapFileMapperTests
    {
        IFileSystem _FileSystem;

        [TestInitialize]
        public void Setup()
        {
            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(fs => fs.FileExists(@"C:\Resources\file.js.map")).Returns(true);
            fileSystemMock.Setup(fs => fs.FileExists(@"C:\Resources\file1.js.map")).Returns(true);
            fileSystemMock.Setup(fs => fs.FileExists(@"C:\Resources\file2.js.map")).Returns(true);
            fileSystemMock.Setup(fs => fs.ReadAllText(@"C:\Resources\file.js.map")).Returns("{\"version\":3,\"file\":\"file.js\",\"sourceRoot\":\"\",\"sources\":[\"file.ts\"],\"names\":[],"
                + "\"mappings\":\"AAEA,gEAFgE;AAE5D,IAAA,CAAC,GAAG,EAAE,CAAC;AAAC,CAAC,IAAI,EAAE,CAAC\"}");
            fileSystemMock.Setup(fs => fs.ReadAllText(@"C:\Resources\file1.js.map")).Returns("incorrect map file format");

            _FileSystem = fileSystemMock.Object;
        }

        [DataTestMethod(@"/file.js", @"C:\Resources\file.js", 1, @"/file.ts", @"C:\Resources\file.ts", 3, 1)]
        public void Test(string inputFilename, string inputFullPath, int inputLineNumber, string outputFilename, string outputFullPath, int outputLineNumber, int outputColumnNumber)
        {
            JsMapFileMapper jsMapFileMapper = new JsMapFileMapper(_FileSystem);

            jsMapFileMapper.Map(new FileLocation(inputFilename, inputFullPath, inputLineNumber)).ShouldBeEquivalentTo(new { Filename = outputFilename, FullPath = outputFullPath, LineNumber = outputLineNumber, ColumnNumber = outputColumnNumber });
        }

        [DataTestMethod("", null, 20)]
        [DataTestMethod(@"/file1.js", @"C:\Resources\file1.js", 20)]
        [DataTestMethod(@"/file2.js", @"C:\Resources\file2.js", 20)]
        public void NotFoundTest(string inputFilename, string inputFullPath, int inputLineNumber)
        {
            JsMapFileMapper jsMapFileMapper = new JsMapFileMapper(_FileSystem);

            jsMapFileMapper.Map(new FileLocation(inputFilename, inputFullPath, inputLineNumber)).Should().BeNull();
        }

        [TestMethod]
        public void MappingNotFound()
        {
            JsMapFileMapper jsMapFileMapper = new JsMapFileMapper(_FileSystem);

            jsMapFileMapper.Map(new FileLocation(null, @"C:\Resources\file.js", 0)).Should().BeNull();
        }

        [TestMethod]
        public void ExceptionTest()
        {
            JsMapFileMapper jsMapFileMapper = new JsMapFileMapper(_FileSystem);

            jsMapFileMapper.Map(new FileLocation(null, @"C:\Resources\file.js", 3)).Should().BeNull();
        }
    }
}
