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
	partial class RunnerViewModel: PropertyChangedBase
	{
		#region IRunnerViewModel

		private System.String _ConnectionId;
		public System.String ConnectionId
		{
			get { return _ConnectionId; }
			set { ChangeProperty<System.String>("ConnectionId", ref _ConnectionId, value, changedCallback: OnConnectionIdChanged); }
		}

		private System.String _Name;
		public System.String Name
		{
			get { return _Name; }
			set { ChangeProperty<System.String>("Name", ref _Name, value); }
		}

		private System.String _UserAgent;
		public System.String UserAgent
		{
			get { return _UserAgent; }
			set { ChangeProperty<System.String>("UserAgent", ref _UserAgent, value, changedCallback: OnUserAgentChanged); }
		}

		private System.String _Status;
		public System.String Status
		{
			get { return _Status; }
			set { ChangeProperty<System.String>("Status", ref _Status, value); }
		}

		private System.Windows.Media.Brush _Background;
		public System.Windows.Media.Brush Background
		{
			get { return _Background; }
			set { ChangeProperty<System.Windows.Media.Brush>("Background", ref _Background, value); }
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
		public string GetPropertyName<TProperty>(Expression<Func<AllGreen.Runner.WPF.Core.ViewModels.RunnerViewModel, TProperty>> propertySelector)
		{
			return propertySelector.GetMemberInfo().Name;
		}

		#endregion
	}
}
