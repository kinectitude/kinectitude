//-----------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Input;

namespace Kinectitude.Editor.Base
{
    internal sealed class DelegateCommand : ICommand
    {
        private readonly Predicate<object> canExecuteDelegate;
        private readonly Action<object> executeDelegate;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

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

        public void Execute(object parameter)
        {
            if (null != executeDelegate)
            {
                executeDelegate(parameter);
            }
        }
    }
}
