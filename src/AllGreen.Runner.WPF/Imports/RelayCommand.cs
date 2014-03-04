using System;
using System.Windows.Input;

namespace Imports
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

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;
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
        public RelayCommand(Func<bool> canExecute, System.Action execute)
            : base(o => canExecute(), o => execute())
        {
        }
        public RelayCommand(System.Action execute)
            : base(o => execute())
        {
        }
    }
}
