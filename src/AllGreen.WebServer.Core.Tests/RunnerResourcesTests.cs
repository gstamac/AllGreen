using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.WebServer.Core.Tests
{
    [TestClass]
    public class RunnerResourcesTests
    {
        [TestMethod]
        public void Test()
        {
            IScriptList fileList = Mock.Of<IScriptList>(fl => fl.Files == new string[] { "file1", @"folder\file2", @"folder\file3" });
            RunnerResources runnerResources = new RunnerResources(fileList);
            runnerResources.GetScriptFiles().ShouldAllBeEquivalentTo(new string[] { "file1", @"folder\file2", @"folder\file3" });
        }
    }
}
