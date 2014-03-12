
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF.Core.ViewModels
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("ImplementPropertyChangedCaliburn.tt", "")]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	partial class SpecStepViewModel: PropertyChangedBase
	{
		#region ISpecStepViewModel

		private System.String _Message;
		public System.String Message
		{
			get { return _Message; }
			set { ChangeProperty<System.String>("Message", ref _Message, value); }
		}

		private AllGreen.Core.FileLocation _ErrorLocation;
		public AllGreen.Core.FileLocation ErrorLocation
		{
			get { return _ErrorLocation; }
			set { ChangeProperty<AllGreen.Core.FileLocation>("ErrorLocation", ref _ErrorLocation, value); }
		}

		private AllGreen.Core.FileLocation _MappedLocation;
		public AllGreen.Core.FileLocation MappedLocation
		{
			get { return _MappedLocation; }
			set { ChangeProperty<AllGreen.Core.FileLocation>("MappedLocation", ref _MappedLocation, value); }
		}

		private AllGreen.Core.SpecStatus _Status;
		public AllGreen.Core.SpecStatus Status
		{
			get { return _Status; }
			set { ChangeProperty<AllGreen.Core.SpecStatus>("Status", ref _Status, value); }
		}

		private Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.Core.ViewModels.SpecTraceStepViewModel> _Trace;
		public Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.Core.ViewModels.SpecTraceStepViewModel> Trace
		{
			get { return _Trace; }
			set { ChangeProperty<Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.Core.ViewModels.SpecTraceStepViewModel>>("Trace", ref _Trace, value); }
		}

		#endregion

		#region INotifyPropertyChanged implementation

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
		protected virtual void ChangeProperty<T>(string propertyName, ref T propertyValue, T newValue, Action<T, T> changedCallback = null)
		{
			if ((propertyValue == null && newValue == null) || (propertyValue != null && propertyValue.Equals(newValue))) return;
			T oldValue = propertyValue;
			propertyValue = newValue;
			NotifyOfPropertyChange(propertyName);
			if (changedCallback != null) changedCallback(oldValue, newValue);
		}

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
		public string GetPropertyName<TProperty>(Expression<Func<AllGreen.Runner.WPF.Core.ViewModels.SpecStepViewModel, TProperty>> propertySelector)
		{
			return propertySelector.GetMemberInfo().Name;
		}

		#endregion
	}
}
