﻿
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF.Core.ViewModels
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("ImplementPropertyChangedCaliburn.tt", "")]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	partial class MainViewModel: PropertyChangedBase
	{
		#region IMainViewModelProperties

		private System.String _ServerStatus;
		public System.String ServerStatus
		{
			get { return _ServerStatus; }
			set { ChangeProperty<System.String>("ServerStatus", ref _ServerStatus, value); }
		}

		private AllGreen.Core.IConfiguration _Configuration;
		public AllGreen.Core.IConfiguration Configuration
		{
			get { return _Configuration; }
			private set { ChangeProperty<AllGreen.Core.IConfiguration>("Configuration", ref _Configuration, value); }
		}

		private System.Boolean _ConfigurationVisible;
		public System.Boolean ConfigurationVisible
		{
			get { return _ConfigurationVisible; }
			set { ChangeProperty<System.Boolean>("ConfigurationVisible", ref _ConfigurationVisible, value); }
		}

		private System.Windows.Input.ICommand _StartServerCommand;
		public System.Windows.Input.ICommand StartServerCommand
		{
			get { return _StartServerCommand; }
			private set { ChangeProperty<System.Windows.Input.ICommand>("StartServerCommand", ref _StartServerCommand, value); }
		}

		private System.Windows.Input.ICommand _RunAllTestsCommand;
		public System.Windows.Input.ICommand RunAllTestsCommand
		{
			get { return _RunAllTestsCommand; }
			private set { ChangeProperty<System.Windows.Input.ICommand>("RunAllTestsCommand", ref _RunAllTestsCommand, value); }
		}

		private System.Windows.Input.ICommand _CopyServerUrlCommand;
		public System.Windows.Input.ICommand CopyServerUrlCommand
		{
			get { return _CopyServerUrlCommand; }
			private set { ChangeProperty<System.Windows.Input.ICommand>("CopyServerUrlCommand", ref _CopyServerUrlCommand, value); }
		}

		private System.Windows.Input.ICommand _ConfigurationCommand;
		public System.Windows.Input.ICommand ConfigurationCommand
		{
			get { return _ConfigurationCommand; }
			private set { ChangeProperty<System.Windows.Input.ICommand>("ConfigurationCommand", ref _ConfigurationCommand, value); }
		}

		private System.Windows.Input.ICommand _OpenFileCommand;
		public System.Windows.Input.ICommand OpenFileCommand
		{
			get { return _OpenFileCommand; }
			private set { ChangeProperty<System.Windows.Input.ICommand>("OpenFileCommand", ref _OpenFileCommand, value); }
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
		public string GetPropertyName<TProperty>(Expression<Func<AllGreen.Runner.WPF.Core.ViewModels.MainViewModel, TProperty>> propertySelector)
		{
			return propertySelector.GetMemberInfo().Name;
		}

		#endregion
	}
}
