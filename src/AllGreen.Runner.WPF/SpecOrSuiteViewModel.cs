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
    public class SpecStatusWithTime
    {
        public SpecStatus Status { get; set; }
        public UInt64 Time { get; set; }
        public RunnerViewModel Runner { get; set; }

        public override string ToString()
        {
            return String.Format("{0} @ {1}", Status, Time);
        }
    }

    public interface ISpecOrSuiteViewModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        BindableDictionary<Guid, SpecStatusWithTime> Statuses { get; }
    }

    [ImplementPropertyChangedCaliburn(typeof(ISpecOrSuiteViewModel))]
    public partial class SpecOrSuiteViewModel : ISpecOrSuiteViewModel
    {
        public SpecOrSuiteViewModel()
        {
            Statuses = new BindableDictionary<Guid, SpecStatusWithTime>();
            /*Statuses.Add(1, SpecStatus.Failed);
            Statuses.Add(2, SpecStatus.Passed);
            Statuses.Add(3, SpecStatus.Running);
            Statuses.Add(4, SpecStatus.Skipped);
            Statuses.Add(5, SpecStatus.Undefined);*/
        }

        public void SetStatus(Guid runnerId, SpecStatus specStatus, UInt64 time)
        {
            SpecStatusWithTime specStatusWithTime = null;
            if (!Statuses.TryGetValue(runnerId, out specStatusWithTime) || specStatusWithTime.Time <= time)
            {
                Statuses[runnerId] = new SpecStatusWithTime() { Status = specStatus, Time = time };
            }
        }

        public void ClearStatus(Guid runnerId)
        {
            Statuses.Remove(runnerId);
        }
    }
}
