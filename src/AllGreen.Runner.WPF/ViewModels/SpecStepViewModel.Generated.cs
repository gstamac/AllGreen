using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF.ViewModels
{
	partial class SpecStepViewModel: PropertyChangedBase
	{
		private System.String _Message;
		public System.String Message
		{
			get { return _Message; }
			set { ChangeProperty<System.String>("Message", ref _Message, value); }
		}

		private AllGreen.WebServer.Core.SpecStatus _Status;
		public AllGreen.WebServer.Core.SpecStatus Status
		{
			get { return _Status; }
			set { ChangeProperty<AllGreen.WebServer.Core.SpecStatus>("Status", ref _Status, value); }
		}

		private AllGreen.WebServer.Core.FileLocation _ScriptLocation;
		public AllGreen.WebServer.Core.FileLocation ScriptLocation
		{
			get { return _ScriptLocation; }
			set { ChangeProperty<AllGreen.WebServer.Core.FileLocation>("ScriptLocation", ref _ScriptLocation, value); }
		}

		private Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.ViewModels.SpecTraceStepViewModel> _Trace;
		public Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.ViewModels.SpecTraceStepViewModel> Trace
		{
			get { return _Trace; }
			set { ChangeProperty<Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.ViewModels.SpecTraceStepViewModel>>("Trace", ref _Trace, value); }
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

		public string GetPropertyName<TProperty>(Expression<Func<AllGreen.Runner.WPF.ViewModels.SpecStepViewModel, TProperty>> propertySelector)
		{
			return propertySelector.GetMemberInfo().Name;
		}

		//ncrunch: no coverage end
		#endregion
	}
}
