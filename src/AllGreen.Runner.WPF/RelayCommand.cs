using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using AllGreen.WebServer.Core;
using Caliburn.Micro;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using TemplateAttributes;
using TinyIoC;
using System.Windows.Input;

namespace AllGreen.Runner.WPF
{
    public class RelayCommand<T> : ICommand where T : class
    {
        private readonly Predicate<T> _CanExecute;
        private readonly Action<T> _Execute;

        public RelayCommand(Predicate<T> canExecute, Action<T> execute)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }

        public RelayCommand(Action<T> execute)
            : this(o => { return true; }, execute)
        {
        }

        public void Execute(object parameter)
        {
            _Execute(parameter as T);
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute(parameter as T);
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Predicate<object> canExecute, Action<object> execute)
            : base(canExecute, execute)
        {
        }
        public RelayCommand(Action<object> execute)
            : base(execute)
        {
        }
        public RelayCommand(System.Action execute)
            : base(o => execute())
        {
        }
    }
}
