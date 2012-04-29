using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Editor.Commands.Base
{
    public class RedoCommand : ICommand
    {
        private readonly ICommandHistory history;

        private bool canExecute;

        public RedoCommand(ICommandHistory history)
        {
            this.history = history;
            this.history.RedoCountChanged += onRedoCountChanged;
            canExecute = (history.RedoCount > 0);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (history.RedoCount > 0)
            {
                IUndoableCommand command = history.PopRedo();
                command.Execute();
            }
        }

        private void onRedoCountChanged(object sender, EventArgs args)
        {
            bool oldCanExecuteValue = canExecute;
            canExecute = (history.RedoCount > 0);
            if (null != CanExecuteChanged && oldCanExecuteValue != canExecute)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
