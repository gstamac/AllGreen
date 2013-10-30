
namespace AllGreen.Runner.WPF
{
	partial class SuiteViewModel
	{
		private System.Boolean _IsExpanded;
		public System.Boolean IsExpanded
		{
			get { return _IsExpanded; }
			set { ChangeProperty<System.Boolean>("IsExpanded", ref _IsExpanded, value); }
		}

		public Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.SpecViewModel> Specs { get; private set; }

		public Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.SuiteViewModel> Suites { get; private set; }

	}
}
