using System;

namespace Kinectitude.Editor.Base
{
    internal sealed class DelegateCommand : ICommand
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
            add { }
            remove { }
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
