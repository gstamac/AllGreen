using System;
using System.Linq;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using TemplateAttributes;

namespace AllGreen.Runner.WPF.ViewModels
{
    internal interface ISpecViewModel : ISpecOrSuiteViewModel
    {
        UInt64 Time { get; set; }
    }

    [ImplementPropertyChangedCaliburn(typeof(ISpecViewModel))]
    public partial class SpecViewModel : SpecOrSuiteViewModel, ISpecViewModel
    {
        public virtual bool IsSpec(Spec spec)
        {
            return Name == spec.Name;
        }

        public static SpecViewModel Create(Spec spec)
        {
            return new SpecViewModel()
            {
                Id = spec.Id,
                Name = spec.Name,
                Time = spec.Time
            };
        }
    }
}