using System;
using AllGreen.WebServer.Core;
using TemplateAttributes;

namespace AllGreen.Runner.WPF
{
    public interface ISpecViewModel : ISpecOrSuiteViewModel
    {
        UInt64 Time { get; set; }
        //string[] Steps { get; set; }
    }

    [ImplementPropertyChangedCaliburn(typeof(ISpecViewModel))]
    public partial class SpecViewModel : SpecOrSuiteViewModel, ISpecViewModel
    {
        public virtual bool IsSpec(Spec spec)
        {
            return Name == spec.Name;
        }
    }
}