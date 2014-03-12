using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Markup;
using AllGreen.Runner.WPF.Core.ViewModels;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF.Core.ValueConverters
{
    public class RunnersToStatusesConverter : MarkupExtension, IMultiValueConverter
    {
        public RunnersToStatusesConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return null;

            BindableCollection<RunnerViewModel> runners = values[0] as BindableCollection<RunnerViewModel>;
            BindableDictionary<string, SpecStatusViewModel> statuses = values[1] as BindableDictionary<string, SpecStatusViewModel>;
            if (runners != null && statuses != null)
                return GetRunnerStatuses(runners, statuses);

            return null;
        }

        private IEnumerable<SpecStatusViewModel> GetRunnerStatuses(BindableCollection<RunnerViewModel> runners, BindableDictionary<string, SpecStatusViewModel> statuses)
        {
            foreach (RunnerViewModel runner in runners)
            {
                if (statuses.ContainsKey(runner.ConnectionId))
                {
                    SpecStatusViewModel status = statuses[runner.ConnectionId];
                    status.Runner = runner;
                    yield return status;
                }
                else
                    yield return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
