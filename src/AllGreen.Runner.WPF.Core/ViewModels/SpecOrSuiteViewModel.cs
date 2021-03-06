using System;
using System.Linq;
using AllGreen.Core;
using TemplateAttributes;
using Caliburn.Micro;
using System.Collections.Generic;

namespace AllGreen.Runner.WPF.Core.ViewModels
{
    [ImplementPropertyChangedCaliburn(typeof(ISpecOrSuiteViewModel))]
    public partial class SpecOrSuiteViewModel
    {
        internal interface ISpecOrSuiteViewModel
        {
            Guid Id { get; set; }
            string Name { get; set; }
            BindableDictionary<string, SpecStatusViewModel> Statuses { get; }
            string Duration { get; set; }
        }

        public SpecOrSuiteViewModel()
        {
            Statuses = new BindableDictionary<string, SpecStatusViewModel>();
        }

        public void SetStatus(string runnerId, SpecStatus specStatus, UInt64 time)
        {
            SetStatus(runnerId, specStatus, time, null, null, null);
        }

        public void SetStatus(string runnerId, SpecStatus specStatus, UInt64 time, IEnumerable<SpecStep> steps, IFileLocationParser fileLocationParser, IFileLocationMapper fileLocationMapper)
        {
            SpecStatusViewModel specStatusViewModel = null;
            if (!Statuses.TryGetValue(runnerId, out specStatusViewModel) || specStatusViewModel.Time <= time)
            {
                if (specStatusViewModel == null)
                {
                    specStatusViewModel = new SpecStatusViewModel() { Duration = 0 };
                    Statuses[runnerId] = specStatusViewModel;
                }
                else
                {
                    specStatusViewModel.Duration += (int)(time - specStatusViewModel.Time);
                }
                specStatusViewModel.Status = specStatus;
                specStatusViewModel.Time = time;
                if (steps != null)
                    specStatusViewModel.Steps = new BindableCollection<SpecStepViewModel>(steps.Select(s => SpecStepViewModel.Create(s, fileLocationParser, fileLocationMapper)));
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
