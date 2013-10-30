using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF
{
	partial class SpecOrSuiteViewModel: PropertyChangedBase
	{
		private System.Guid _Id;
		public System.Guid Id
		{
			get { return _Id; }
			set { ChangeProperty<System.Guid>("Id", ref _Id, value); }
		}

		private System.String _Name;
		public System.String Name
		{
			get { return _Name; }
			set { ChangeProperty<System.String>("Name", ref _Name, value); }
		}

		private AllGreen.Runner.WPF.BindableDictionary<System.String,AllGreen.Runner.WPF.SpecStatusViewModel> _Statuses;
		public AllGreen.Runner.WPF.BindableDictionary<System.String,AllGreen.Runner.WPF.SpecStatusViewModel> Statuses
		{
			get { return _Statuses; }
			private set { ChangeProperty<AllGreen.Runner.WPF.BindableDictionary<System.String,AllGreen.Runner.WPF.SpecStatusViewModel>>("Statuses", ref _Statuses, value); }
		}

		private System.String _Duration;
		public System.String Duration
		{
			get { return _Duration; }
			set { ChangeProperty<System.String>("Duration", ref _Duration, value); }
		}


		#region INotifyPropertyChanged implementation
		//ncrunch: no coverage start

		protected virtual void ChangeProperty<T>(string propertyName, ref T propertyValue, T newValue, Action<T, T> changedCallback = null)
		{
			if ((propertyValue == null && newValue == null) || (propertyValue != null && propertyValue.Equals(newValue))) return;
			T oldValue = propertyValue;
			propertyValue = newValue;
			NotifyOfPropertyChange(propertyName);
			if (changedCallback != null) changedCallback(oldValue, newValue);
		}

		public string GetPropertyName<TProperty>(Expression<Func<AllGreen.Runner.WPF.SpecOrSuiteViewModel, TProperty>> propertySelector)
		{
			return propertySelector.GetMemberInfo().Name;
		}

		//ncrunch: no coverage end
		#endregion
	}
}
