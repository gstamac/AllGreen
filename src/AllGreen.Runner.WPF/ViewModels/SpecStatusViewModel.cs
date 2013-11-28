using System;
using System.Linq;
using System.Linq.Expressions;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using TemplateAttributes;

namespace AllGreen.Runner.WPF.ViewModels
{
    public interface ISpecStatusViewModel
    {
        SpecStatus Status { get; set; }
        UInt64 Time { get; set; }
        int Duration { get; set; }
        string DurationText { get; }
        RunnerViewModel Runner { get; set; }
        BindableCollection<SpecStepViewModel> Steps { get; set; }
        string Description { get; }
    }

    [ImplementPropertyChangedCaliburn(typeof(ISpecStatusViewModel))]
    public partial class SpecStatusViewModel : ISpecStatusViewModel
    {
        private void OnStatusChanged(SpecStatus oldValue, SpecStatus newValue)
        {
            UpdateDescription();
        }

        private void OnDurationChanged(int oldValue, int newValue)
        {
            UpdateDirationText();
            UpdateDescription();
        }

        private void OnStepsChanged(object oldValue, object newValue)
        {
            UpdateDescription();
        }

        private void UpdateDirationText()
        {
            DurationText = FormattedDuration();
        }

        private void UpdateDescription()
        {
            Description = FormattedDescription();
        }

        private string FormattedDuration()
        {
            if (Duration < 1000)
                return String.Format("{0} ms", Duration);
            else
                return String.Format("{0:0.000} s", (float)Duration / 1000);
        }

        private string FormattedDescription()
        {
            string formattedString = String.Format("{0} in {1}", Status, DurationText);
            if (Steps != null)
                formattedString += Environment.NewLine + FormatSteps();
            return formattedString;
        }

        private string FormatSteps()
        {
            return String.Join(Environment.NewLine, Steps.Select(s => s.ToString().Trim('\n', '\r')));
        }
    }
}
