using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;

namespace AllGreen.Runner.WPF.ViewModels
{
	partial class SpecViewModel
	{
		private System.UInt64 _Time;
		public System.UInt64 Time
		{
			get { return _Time; }
			set { ChangeProperty<System.UInt64>("Time", ref _Time, value); }
		}

	}
}
