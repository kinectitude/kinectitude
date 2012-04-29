using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Editor.Commands.Base
{
    public class UndoCommand : ICommand
    {
        private readonly ICommandHistory history;

        private bool canExecute;

        public UndoCommand(ICommandHistory history)
        {
            this.history = history;
            this.history.UndoCountChanged += onUndoCountChanged;
            canExecute = (history.UndoCount > 0);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (history.UndoCount > 0)
            {
                IUndoableCommand command = history.PopUndo();
                command.Unexecute();
            }
        }

        private void onUndoCountChanged(object sender, EventArgs args)
        {
            bool oldCanExecuteValue = canExecute;
            canExecute = (history.UndoCount > 0);
            if (null != CanExecuteChanged && oldCanExecuteValue != canExecute)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
