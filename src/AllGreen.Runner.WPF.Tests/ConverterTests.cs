using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AllGreen.Runner.WPF.ValueConverters;
using AllGreen.Runner.WPF.ViewModels;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AllGreen.Runner.WPF.Tests
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void NotConverterTest()
        {
            NotConverter notConverter = new NotConverter();

            notConverter.ProvideValue(null).Should().Be(notConverter);

            bool value = true;

            object result = notConverter.Convert(value, typeof(bool), null, null);
            result.Should().BeAssignableTo<bool>();
            ((bool)result).Should().BeFalse();

            result = notConverter.ConvertBack(value, typeof(bool), null, null);
            result.Should().BeAssignableTo<bool>();
            ((bool)result).Should().BeFalse();
        }

        [TestMethod]
        public void ObjectToVisibilityConverterTest()
        {
            ObjectToVisibilityConverter objectToVisibilityConverter = new ObjectToVisibilityConverter();

            objectToVisibilityConverter.ProvideValue(null).Should().Be(objectToVisibilityConverter);

            objectToVisibilityConverter.Convert(null, typeof(Visibility), null, null).Should().Be(Visibility.Collapsed);
            objectToVisibilityConverter.Convert("something", typeof(Visibility), null, null).Should().Be(Visibility.Visible);

            objectToVisibilityConverter.ConvertBack(null, typeof(object), null, null).Should().BeNull();
        }

        [TestMethod]
        public void RunnersToStatusesConverterTest()
        {
            RunnersToStatusesConverter runnersToStatusesConverter = new RunnersToStatusesConverter();

            runnersToStatusesConverter.ProvideValue(null).Should().Be(runnersToStatusesConverter);

            runnersToStatusesConverter.Convert(new object[] { null, null }, typeof(IEnumerable), null, null).Should().BeNull();

            BindableCollection<RunnerViewModel> runners = new BindableCollection<RunnerViewModel>();
            RunnerViewModel runner1 = new RunnerViewModel { ConnectionId = "conn1", Name = "Runner 1" };
            RunnerViewModel runner2 = new RunnerViewModel { ConnectionId = "conn2", Name = "Runner 2" };
            runners.Add(runner1);
            runners.Add(runner2);
            BindableDictionary<string, SpecStatusViewModel> statuses = new BindableDictionary<string, SpecStatusViewModel>();
            statuses.Add("conn1", new SpecStatusViewModel { Status = SpecStatus.Passed, Time = 1, Duration = 1, Runner = runner1 });
            statuses.Add("conn2", new SpecStatusViewModel { Status = SpecStatus.Failed, Time = 2, Duration = 2, Runner = runner2 });

            object result = runnersToStatusesConverter.Convert(new object[] { runners, statuses }, typeof(IEnumerable), null, null);
            result.Should().BeAssignableTo<IEnumerable<SpecStatusViewModel>>();
            (result as IEnumerable<SpecStatusViewModel>).ShouldAllBeEquivalentTo(new object[] { 
                new { Status = SpecStatus.Passed, Time = 1, Duration = 1, Runner = runner1 }, 
                new { Status = SpecStatus.Failed, Time = 2, Duration = 2, Runner = runner2 } },
                o => o.Excluding(si => si.PropertyPath.EndsWith("IsNotifying") || si.PropertyPath.EndsWith("Steps") || si.PropertyPath.EndsWith("Description") || si.PropertyPath.EndsWith("DurationText")));

            runners.Add(new RunnerViewModel { ConnectionId = "conn3", Name = "Runner 3" });

            result = runnersToStatusesConverter.Convert(new object[] { runners, statuses }, typeof(IEnumerable), null, null);
            result.Should().BeAssignableTo<IEnumerable<SpecStatusViewModel>>();
            (result as IEnumerable<SpecStatusViewModel>).ShouldAllBeEquivalentTo(new object[] { 
                new { Status = SpecStatus.Passed, Time = 1, Duration = 1, Runner = runner1 }, 
                new { Status = SpecStatus.Failed, Time = 2, Duration = 2, Runner = runner2 },
                null },
                o => o.Excluding(si => si.PropertyPath.EndsWith("IsNotifying") || si.PropertyPath.EndsWith("Steps") || si.PropertyPath.EndsWith("Description") || si.PropertyPath.EndsWith("DurationText")));

            runnersToStatusesConverter.ConvertBack(null, null, null, null).Should().BeNull();
        }

        [TestMethod]
        public void SpecStatusToImageConverterTest()
        {
            SpecStatusToImageConverter specStatusToImageConverter = new SpecStatusToImageConverter();

            specStatusToImageConverter.ProvideValue(null).Should().Be(specStatusToImageConverter);

            specStatusToImageConverter.Convert(SpecStatus.Undefined, typeof(ImageSource), null, null).Should().BeAssignableTo<BitmapImage>();
            specStatusToImageConverter.Convert(SpecStatus.Running, typeof(ImageSource), null, null).Should().BeAssignableTo<BitmapImage>();
            specStatusToImageConverter.Convert(SpecStatus.Passed, typeof(ImageSource), null, null).Should().BeAssignableTo<BitmapImage>();
            specStatusToImageConverter.Convert(SpecStatus.Failed, typeof(ImageSource), null, null).Should().BeAssignableTo<BitmapImage>();
            specStatusToImageConverter.Convert(SpecStatus.Skipped, typeof(ImageSource), null, null).Should().BeAssignableTo<BitmapImage>();

            specStatusToImageConverter.ConvertBack(null, null, null, null).Should().BeNull();
        }
    }
}
