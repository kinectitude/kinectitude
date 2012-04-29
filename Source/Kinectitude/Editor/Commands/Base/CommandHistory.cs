using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Editor.Base;

namespace Editor.Commands.Base
{
    public class CommandHistory : BaseModel, ICommandHistory
    {
        public event EventHandler UndoCountChanged = delegate { };
        public event EventHandler RedoCountChanged = delegate { };

        private readonly Stack<IUndoableCommand> undo;
        private readonly Stack<IUndoableCommand> redo;

        public ICommand UndoCommand
        {
            get { return new UndoCommand(this); }
        }

        public ICommand RedoCommand
        {
            get { return new RedoCommand(this); }
        }

        public int UndoCount
        {
            get { return undo.Count; }
        }

        public int RedoCount
        {
            get { return redo.Count; }
        }

        public string LastUndoableCommandName
        {
            get { return undo.Count > 0 ? undo.Peek().Name : null; }
        }

        public string LastRedoableCommandName
        {
            get { return redo.Count > 0 ? redo.Peek().Name : null; }
        }

        public CommandHistory()
        {
            undo = new Stack<IUndoableCommand>();
            redo = new Stack<IUndoableCommand>();
        }

        public void PushUndo(IUndoableCommand command)
        {
            undo.Push(command);
            raiseUndoChange();
        }

        public void PushRedo(IUndoableCommand command)
        {
            redo.Push(command);
            raiseRedoChange();
        }

        public IUndoableCommand PopUndo()
        {
            IUndoableCommand command = null;
            if (undo.Count > 0)
            {
                command = undo.Pop();
                raiseUndoChange();
            }
            return command;
        }

        public IUndoableCommand PopRedo()
        {
            IUndoableCommand command = null;
            if (redo.Count > 0)
            {
                command = redo.Pop();
                raiseRedoChange();
            }
            return command;
        }

        private void raiseUndoChange()
        {
            RaisePropertyChanged("LastUndoableCommandName");
            UndoCountChanged(this, EventArgs.Empty);
        }

        private void raiseRedoChange()
        {
            RaisePropertyChanged("LastRedoableCommandName");
            RedoCountChanged(this, EventArgs.Empty);
        }
    }
}
