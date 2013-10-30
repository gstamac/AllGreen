using System;
using System.Collections.Generic;
using AllGreen.WebServer.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public class SpecStatusViewModelTests
    {
        [TestMethod]
        public void ToStringTest()
        {
            SpecStatusViewModel specStatusViewModel = new SpecStatusViewModel
            {
                Status = SpecStatus.Passed,
                Time = 10,
                Duration = 11
            };
            specStatusViewModel.ToString().Should().Be("Passed in 11 ms");

            specStatusViewModel = new SpecStatusViewModel
            {
                Status = SpecStatus.Passed,
                Time = 10,
                Duration = 11123
            };
            specStatusViewModel.ToString().Should().Be("Passed in 11,123 s");
        }
    }
}
