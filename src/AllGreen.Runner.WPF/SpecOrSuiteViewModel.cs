using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using AllGreen.WebServer.Core;
using TemplateAttributes;

namespace AllGreen.Runner.WPF
{
    public interface ISpecOrSuiteViewModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        BindableDictionary<string, SpecStatusViewModel> Statuses { get; }
        string Duration { get; set; }
    }

    [ImplementPropertyChangedCaliburn(typeof(ISpecOrSuiteViewModel))]
    public partial class SpecOrSuiteViewModel : ISpecOrSuiteViewModel
    {
        public SpecOrSuiteViewModel()
        {
            Statuses = new BindableDictionary<string, SpecStatusViewModel>();
        }

        public void SetStatus(string runnerId, SpecStatus specStatus, UInt64 time)
        {
            SpecStatusViewModel specStatusWithTime = null;
            if (!Statuses.TryGetValue(runnerId, out specStatusWithTime) || specStatusWithTime.Time <= time)
            {
                int duration = 0;
                if (specStatusWithTime != null)
                    duration = specStatusWithTime.Duration + (int)(time - specStatusWithTime.Time);
                Statuses[runnerId] = new SpecStatusViewModel() { Status = specStatus, Time = time, Duration = duration };
                UpdateDuration();
            }
        }

        public void ClearStatus(string runnerId)
        {
            Statuses.Remove(runnerId);
            UpdateDuration();
        }
        private void UpdateDuration()
        {
            int durationSum = Statuses.Sum(s => s.Value.Duration);
            if (durationSum < 1000)
                Duration = String.Format("{0} ms", durationSum);
            else
                Duration = String.Format("{0:0.000} s", (float)durationSum / 1000);
        }
    }
}
