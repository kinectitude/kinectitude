using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace EditorModels.Commands
{
    internal interface ICommandHistory
    {
        ObservableCollection<IUndoableCommand> UndoableCommands { get; }
        ObservableCollection<IUndoableCommand> RedoableCommands { get; }

        IUndoableCommand LastUndoableCommand { get; }
        IUndoableCommand LastRedoableCommand { get; }

        ICommand UndoCommand { get; }
        ICommand RedoCommand { get; }

        void Log(IUndoableCommand command);
    }
}
