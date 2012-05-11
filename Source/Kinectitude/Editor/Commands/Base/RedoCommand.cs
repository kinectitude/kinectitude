using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Commands.Base
{
    public class RedoCommand : ICommand
    {
        private bool canExecute;

        public RedoCommand()
        {
            CommandHistory.Instance.RedoCountChanged += OnRedoCountChanged;
            canExecute = CommandHistory.Instance.RedoCount > 0;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (CommandHistory.Instance.RedoCount > 0)
            {
                IUndoableCommand command = CommandHistory.Instance.PopRedo();
                command.Execute();
            }
        }

        private void OnRedoCountChanged(object sender, EventArgs args)
        {
            bool oldCanExecuteValue = canExecute;
            canExecute = (CommandHistory.Instance.RedoCount > 0);
            if (null != CanExecuteChanged && oldCanExecuteValue != canExecute)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
