using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Editor.Commands.Base
{
    public interface ICommandHistory
    {
        int UndoCount { get; }
        int RedoCount { get; }
        ICommand UndoCommand { get; }
        ICommand RedoCommand { get; }

        event EventHandler UndoCountChanged;
        event EventHandler RedoCountChanged;

        void PushUndo(IUndoableCommand command);
        void PushRedo(IUndoableCommand command);
        IUndoableCommand PopUndo();
        IUndoableCommand PopRedo();
    }
}
