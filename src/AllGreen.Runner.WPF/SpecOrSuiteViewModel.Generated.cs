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

		public AllGreen.Runner.WPF.BindableDictionary<System.Guid,AllGreen.Runner.WPF.SpecStatusWithTime> Statuses { get; private set; }


		#region INotifyPropertyChanged implementation

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

		#endregion
	}
}
