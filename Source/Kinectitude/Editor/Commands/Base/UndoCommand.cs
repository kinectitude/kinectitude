using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Commands.Base
{
    public class UndoCommand : ICommand
    {
        private bool canExecute;

        public UndoCommand()
        {
            CommandHistory.Instance.UndoCountChanged += OnUndoCountChanged;
            canExecute = CommandHistory.Instance.UndoCount > 0;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (CommandHistory.Instance.UndoCount > 0)
            {
                IUndoableCommand command = CommandHistory.Instance.PopUndo();
                command.Unexecute();
            }
        }

        private void OnUndoCountChanged(object sender, EventArgs args)
        {
            bool oldCanExecuteValue = canExecute;
            canExecute = (CommandHistory.Instance.UndoCount > 0);
            if (null != CanExecuteChanged && oldCanExecuteValue != canExecute)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
