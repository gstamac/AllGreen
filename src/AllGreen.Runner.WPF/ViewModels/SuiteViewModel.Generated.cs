using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF.ViewModels
{
	partial class SuiteViewModel
	{
		private System.Boolean _IsExpanded;
		public System.Boolean IsExpanded
		{
			get { return _IsExpanded; }
			set { ChangeProperty<System.Boolean>("IsExpanded", ref _IsExpanded, value); }
		}

		private Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.ViewModels.SpecViewModel> _Specs;
		public Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.ViewModels.SpecViewModel> Specs
		{
			get { return _Specs; }
			private set { ChangeProperty<Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.ViewModels.SpecViewModel>>("Specs", ref _Specs, value); }
		}

		private Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.ViewModels.SuiteViewModel> _Suites;
		public Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.ViewModels.SuiteViewModel> Suites
		{
			get { return _Suites; }
			private set { ChangeProperty<Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.ViewModels.SuiteViewModel>>("Suites", ref _Suites, value); }
		}

	}
}
