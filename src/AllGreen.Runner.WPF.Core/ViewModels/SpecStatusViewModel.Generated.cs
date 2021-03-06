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
	partial class SpecStatusViewModel: PropertyChangedBase
	{
		#region ISpecStatusViewModel

		private AllGreen.Core.SpecStatus _Status;
		public AllGreen.Core.SpecStatus Status
		{
			get { return _Status; }
			set { ChangeProperty<AllGreen.Core.SpecStatus>("Status", ref _Status, value, changedCallback: OnStatusChanged); }
		}

		private System.UInt64 _Time;
		public System.UInt64 Time
		{
			get { return _Time; }
			set { ChangeProperty<System.UInt64>("Time", ref _Time, value); }
		}

		private System.Int32 _Duration;
		public System.Int32 Duration
		{
			get { return _Duration; }
			set { ChangeProperty<System.Int32>("Duration", ref _Duration, value, changedCallback: OnDurationChanged); }
		}

		private System.String _DurationText;
		public System.String DurationText
		{
			get { return _DurationText; }
			private set { ChangeProperty<System.String>("DurationText", ref _DurationText, value); }
		}

		private AllGreen.Runner.WPF.Core.ViewModels.RunnerViewModel _Runner;
		public AllGreen.Runner.WPF.Core.ViewModels.RunnerViewModel Runner
		{
			get { return _Runner; }
			set { ChangeProperty<AllGreen.Runner.WPF.Core.ViewModels.RunnerViewModel>("Runner", ref _Runner, value); }
		}

		private Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.Core.ViewModels.SpecStepViewModel> _Steps;
		public Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.Core.ViewModels.SpecStepViewModel> Steps
		{
			get { return _Steps; }
			set { ChangeProperty<Caliburn.Micro.BindableCollection<AllGreen.Runner.WPF.Core.ViewModels.SpecStepViewModel>>("Steps", ref _Steps, value, changedCallback: OnStepsChanged); }
		}

		private System.String _Description;
		public System.String Description
		{
			get { return _Description; }
			private set { ChangeProperty<System.String>("Description", ref _Description, value); }
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
		public string GetPropertyName<TProperty>(Expression<Func<AllGreen.Runner.WPF.Core.ViewModels.SpecStatusViewModel, TProperty>> propertySelector)
		{
			return propertySelector.GetMemberInfo().Name;
		}

		#endregion
	}
}
