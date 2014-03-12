using System;
using System.Linq;
using AllGreen.Core;
using Caliburn.Micro;
using TemplateAttributes;

namespace AllGreen.Runner.WPF.Core.ViewModels
{
    [ImplementPropertyChangedCaliburn(typeof(ISpecViewModel))]
    public partial class SpecViewModel : SpecOrSuiteViewModel
    {
        interface ISpecViewModel : ISpecOrSuiteViewModel
        {
            UInt64 Time { get; set; }
        }

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