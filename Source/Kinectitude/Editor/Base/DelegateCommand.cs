using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Editor.Base
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> canExecuteDelegate;
        private readonly Action<object> executeDelegate;

        public DelegateCommand(Predicate<object> canExecuteDelegate, Action<object> executeDelegate)
        {
            this.canExecuteDelegate = canExecuteDelegate;
            this.executeDelegate = executeDelegate;
        }

        public bool CanExecute(object parameter)
        {
            bool result = true;
            if (null != canExecuteDelegate)
            {
                result = canExecuteDelegate(parameter);
            }
            return result;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (null != executeDelegate)
            {
                executeDelegate(parameter);
            }
        }
    }
}
