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
    public class CompositeFileLocationMapperTests
    {
        [TestMethod]
        public void Test()
        {
            CompositeFileLocationMapper compositeFileLocationMapper = new CompositeFileLocationMapper();

            Mock<IFileLocationMapper> mockIFileLocationMapper1 = new Mock<IFileLocationMapper>();
            mockIFileLocationMapper1.Setup(flm => flm.Map(It.Is<FileLocation>(fl => fl.Filename == "file1.js"))).Returns(new FileLocation("file1.ts", "", 12));
            mockIFileLocationMapper1.Setup(flm => flm.Map(It.Is<FileLocation>(fl => fl.Filename == "file3.js"))).Returns(new FileLocation("file3x.js", "", 11));
            Mock<IFileLocationMapper> mockIFileLocationMapper2 = new Mock<IFileLocationMapper>();
            mockIFileLocationMapper2.Setup(flm => flm.Map(It.Is<FileLocation>(fl => fl.Filename == "file2.js"))).Returns(new FileLocation("file2.ts", "", 12));
            mockIFileLocationMapper2.Setup(flm => flm.Map(It.Is<FileLocation>(fl => fl.Filename == "file3x.js"))).Returns(new FileLocation("file3.ts", "", 12));
            compositeFileLocationMapper.Add(mockIFileLocationMapper1.Object);
            compositeFileLocationMapper.Add(mockIFileLocationMapper2.Object);

            compositeFileLocationMapper.Map(new FileLocation("file1.js", "", 10)).ShouldBeEquivalentTo(new FileLocation("file1.ts", "", 12));
            compositeFileLocationMapper.Map(new FileLocation("file2.js", "", 10)).ShouldBeEquivalentTo(new FileLocation("file2.ts", "", 12));
            compositeFileLocationMapper.Map(new FileLocation("file3.js", "", 10)).ShouldBeEquivalentTo(new FileLocation("file3.ts", "", 12));
        }
    }
}
