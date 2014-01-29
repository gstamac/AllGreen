using System;
using System.Linq;
using AllGreen.Runner.WPF.ViewModels;
using AllGreen.WebServer.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public class ObservableReporterTests
    {
        protected ObservableReporter _Reporter;

        [TestInitialize]
        public void Setup()
        {
            _Reporter = new ObservableReporter(Mock.Of<IFileLocationParser>(), null);
        }

        [TestClass]
        public class RunnerTests : ObservableReporterTests
        {
            [TestMethod]
            public void RunnersCollectionChangedTests()
            {
                (new TestHelper.ObservableCollectionTester<RunnerViewModel>(_Reporter.Runners)).RunTests();
            }

            [TestMethod]
            public void ConnectedTest()
            {
                string guid = Guid.NewGuid().ToString();
                _Reporter.Connected(guid, "");
                _Reporter.Runners.Should().OnlyContain(r => r.ConnectionId == guid && r.Name == guid.ToString() && r.UserAgent == "" && r.Status == "Connected");
                _Reporter.Connected(guid, "USERAGENT");
                _Reporter.Runners.Should().OnlyContain(r => r.ConnectionId == guid && r.Name == "USERAGENT" && r.UserAgent == "USERAGENT" && r.Status == "Connected");
            }

            [TestMethod]
            public void ReconnectedTest()
            {
                string guid = Guid.NewGuid().ToString();
                _Reporter.Reconnected(guid, "");
                _Reporter.Runners.Should().OnlyContain(r => r.ConnectionId == guid && r.Name == guid.ToString() && r.Status == "Reconnected");
            }

            [TestMethod]
            public void DisconnectedTest()
            {
                string guid = Guid.NewGuid().ToString();
                _Reporter.Disconnected(guid);
                _Reporter.Runners.Should().BeEmpty();
                _Reporter.Connected(guid, "USERAGENT");
                _Reporter.Disconnected(guid);
                _Reporter.Runners.Should().BeEmpty();
            }

            [TestMethod]
            public void RegisterTest()
            {
                string guid = Guid.NewGuid().ToString();
                _Reporter.Connected(guid, "");
                _Reporter.Register(guid, "");
                _Reporter.Runners.Should().OnlyContain(r => r.ConnectionId == guid && r.Name == guid.ToString() && r.Status == "Registered");

                guid = Guid.NewGuid().ToString();
                _Reporter.Register(guid, @"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:23.0) Gecko/20100101 Firefox/23.0");
                _Reporter.Runners.Should().Contain(r => r.ConnectionId == guid && r.Name == "Mozilla/5.0" && r.Status == "Registered");

                _Reporter.Runners.Should().HaveCount(2);
            }

            [TestMethod]
            public void ResetTest()
            {
                string guid = Guid.NewGuid().ToString();
                _Reporter.Reset(guid);
                _Reporter.Runners.Should().OnlyContain(r => r.ConnectionId == guid && r.Status == "Reset");
            }

            [TestMethod]
            public void StartedTest()
            {
                string guid = Guid.NewGuid().ToString();
                _Reporter.Started(guid, 15);
                _Reporter.Runners.Should().OnlyContain(r => r.ConnectionId == guid && r.Status == "Started running 15 tests");
            }

            [TestMethod]
            public void FinishedTest()
            {
                string guid = Guid.NewGuid().ToString();
                _Reporter.Finished(guid);
                _Reporter.Runners.Should().OnlyContain(r => r.ConnectionId == guid && r.Status == "Finished");
            }
        }

        [TestClass]
        public class SpecUpdatedTests : ObservableReporterTests
        {
            private string _ConnectionId = Guid.NewGuid().ToString();
            private Guid _ParentSuiteId = Guid.NewGuid();
            private Guid _SuiteId = Guid.NewGuid();
            private Guid _SpecId = Guid.NewGuid();

            [TestMethod]
            public void SuitesCollectionChangedTests()
            {
                (new TestHelper.ObservableCollectionTester<SuiteViewModel>(_Reporter.Suites)).RunTests();
            }

            [TestMethod]
            public void NewSpec()
            {
                Spec spec = CreateSpec();
                spec.Steps = new SpecStep[] { new SpecStep { Message = "Massage 1", Status = SpecStatus.Passed, Trace = "Trace 1" } };

                _Reporter.SpecUpdated(Guid.NewGuid().ToString(), spec);

                CheckSpec(spec);
            }

            [TestMethod]
            public void ExistingSpec()
            {
                Spec spec = CreateSpec();
                _Reporter.SpecUpdated(_ConnectionId, spec);
                spec = CreateSpec(SpecStatus.Passed);
                spec.Time = 200;
                spec.Steps = new SpecStep[] { new SpecStep { Message = "Massage 1", Status = SpecStatus.Passed, Trace = "Trace 1" } };

                _Reporter.SpecUpdated(_ConnectionId, spec);

                CheckSpec(spec);
            }

            [TestMethod]
            public void MultipleRunners()
            {
                Spec spec = CreateSpec();
                _Reporter.SpecUpdated(_ConnectionId, spec);
                spec = CreateSpec(SpecStatus.Passed);
                _Reporter.SpecUpdated(Guid.NewGuid().ToString(), spec);

                _Reporter.Suites.First().Suites.First().Specs.First().Statuses.Values
                    .ShouldAllBeEquivalentTo(new SpecStatus[] { SpecStatus.Running, SpecStatus.Passed }, o => o.ExcludingMissingProperties());
            }

            [TestMethod]
            public void DisconnectedShouldRemoveSpecStatuses()
            {
                _Reporter.Connected(_ConnectionId, "USERAGENT");
                Spec spec = CreateSpec();
                _Reporter.SpecUpdated(_ConnectionId, spec);
                _Reporter.Suites.First().Suites.First().Specs.First().Statuses.Values
                    .ShouldAllBeEquivalentTo(new SpecStatus[] { SpecStatus.Running }, o => o.ExcludingMissingProperties());
                _Reporter.Disconnected(_ConnectionId);

                _Reporter.Suites.First().Suites.First().Specs.First().Statuses.Values
                    .ShouldAllBeEquivalentTo(new SpecStatus[] { }, o => o.ExcludingMissingProperties());
            }

            [TestMethod]
            public void ResetShouldRemoveSpecStatuses()
            {
                _Reporter.Connected(_ConnectionId, "USERAGENT");
                Spec spec = CreateSpec();
                _Reporter.SpecUpdated(_ConnectionId, spec);
                _Reporter.Suites.First().Suites.First().Specs.First().Statuses.Values
                    .ShouldAllBeEquivalentTo(new SpecStatus[] { SpecStatus.Running }, o => o.ExcludingMissingProperties());
                _Reporter.Reset(_ConnectionId);

                _Reporter.Suites.First().Suites.First().Specs.First().Statuses.Values
                    .ShouldAllBeEquivalentTo(new SpecStatus[] { }, o => o.ExcludingMissingProperties());
            }

            private void CheckSpec(Spec spec)
            {
                _Reporter.Suites.ShouldAllBeEquivalentTo(new Suite[] { spec.Suite.ParentSuite }, o => o.ExcludingMissingProperties());
                _Reporter.Suites.First().Suites.ShouldAllBeEquivalentTo(new Suite[] { spec.Suite }, o => o.ExcludingMissingProperties());
                _Reporter.Suites.First().Suites.First().Specs.Should().HaveCount(1);

                SpecViewModel reportersSpec = _Reporter.Suites.First().Suites.First().Specs.First();
                reportersSpec.ShouldBeEquivalentTo(spec, o => o.Excluding(si => si.PropertyPath == "Statuses" || si.PropertyPath == "Duration" || si.PropertyPath.EndsWith("IsNotifying")));
                reportersSpec.Statuses.Should().HaveCount(1);
                SpecStatusViewModel status = reportersSpec.Statuses.First().Value;
                status.Status.Should().Be(spec.Status);
                status.Description.Should().Contain(spec.Status.ToString());
                if (spec.Steps != null)
                    foreach (SpecStep step in spec.Steps)
                    {
                        status.Description.Should().Contain(step.Message);
                        status.Description.Should().Contain(step.Status.ToString());
                        status.Description.Should().Contain(step.Trace);
                    }
            }

            private Spec CreateSpec(SpecStatus specStatus = SpecStatus.Running)
            {
                Suite parentSuite = new Suite() { Id = _ParentSuiteId, Name = "Suite 1", ParentSuite = null, Status = specStatus };
                Suite suite = new Suite() { Id = _SuiteId, Name = "Suite 1", ParentSuite = parentSuite, Status = specStatus };
                return new Spec()
                {
                    Id = _SpecId,
                    Name = "Spec 1",
                    Status = specStatus,
                    Suite = suite,
                    Time = 100
                };
            }
        }
    }
}
