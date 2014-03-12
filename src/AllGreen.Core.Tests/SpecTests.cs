using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.Core.Tests
{
    [TestClass]
    public class SpecTests
    {
        [TestMethod]
        public void GetFullNameTest()
        {
            Suite suite1 = new Suite { Id = Guid.NewGuid(), Name = "Suite 1", ParentSuite = null };
            Suite suite2 = new Suite { Id = Guid.NewGuid(), Name = "Suite 2", ParentSuite = suite1 };
            Spec spec = new Spec { Id = Guid.NewGuid(), Name = "Spec", Suite = suite2 };

            spec.GetFullName().Should().Be("Suite 1 > Suite 2 > Spec");
        }
    }
}
