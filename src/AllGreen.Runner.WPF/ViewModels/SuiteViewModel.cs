using AllGreen.WebServer.Core;
using Caliburn.Micro;
using TemplateAttributes;

namespace AllGreen.Runner.WPF.ViewModels
{
    [ImplementPropertyChangedCaliburn(typeof(ISuiteViewModel))]
    public partial class SuiteViewModel : SpecOrSuiteViewModel
    {
        interface ISuiteViewModel : ISpecOrSuiteViewModel
        {
            bool IsExpanded { get; set; }
            BindableCollection<SpecViewModel> Specs { get; }
            BindableCollection<SuiteViewModel> Suites { get; }
        }

        public SuiteViewModel()
        {
            Specs = new BindableCollection<SpecViewModel>();
            Suites = new BindableCollection<SuiteViewModel>();
        }

        private System.Windows.Data.CompositeCollection _Children = null;
        public System.Windows.Data.CompositeCollection Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = new System.Windows.Data.CompositeCollection();
                    _Children.Add(new System.Windows.Data.CollectionContainer() { Collection = Specs });
                    _Children.Add(new System.Windows.Data.CollectionContainer() { Collection = Suites });
                }
                return _Children;
            }
        }

        public virtual bool IsSuite(Suite suite)
        {
            return Name == suite.Name;
        }

        public static SuiteViewModel Create(Suite suite)
        {
            return new SuiteViewModel()
            {
                Id = suite.Id,
                Name = suite.Name
            };
        }
    }
}
