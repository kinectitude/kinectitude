using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Kinectitude.Editor.Base;

namespace Kinectitude.Editor.Commands.Base
{
    public class CommandHistory : BaseModel, ICommandHistory
    {
        private static readonly CommandHistory instance;

        static CommandHistory()
        {
            instance = new CommandHistory();
        }

        public static CommandHistory Instance
        {
            get { return instance; }
        }

        public event EventHandler UndoCountChanged = delegate { };
        public event EventHandler RedoCountChanged = delegate { };

        private readonly Stack<IUndoableCommand> undo;
        private readonly Stack<IUndoableCommand> redo;

        public ICommand UndoCommand
        {
            get { return new UndoCommand(); }
        }

        public ICommand RedoCommand
        {
            get { return new RedoCommand(); }
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

        private CommandHistory()
        {
            undo = new Stack<IUndoableCommand>();
            redo = new Stack<IUndoableCommand>();
        }

        public void PushUndo(IUndoableCommand command)
        {
            undo.Push(command);
            RaiseUndoChange();
        }

        public void PushRedo(IUndoableCommand command)
        {
            redo.Push(command);
            RaiseRedoChange();
        }

        public IUndoableCommand PopUndo()
        {
            IUndoableCommand command = null;
            if (undo.Count > 0)
            {
                command = undo.Pop();
                RaiseUndoChange();
            }
            return command;
        }

        public IUndoableCommand PopRedo()
        {
            IUndoableCommand command = null;
            if (redo.Count > 0)
            {
                command = redo.Pop();
                RaiseRedoChange();
            }
            return command;
        }

        private void RaiseUndoChange()
        {
            RaisePropertyChanged("LastUndoableCommandName");
            UndoCountChanged(this, EventArgs.Empty);
        }

        private void RaiseRedoChange()
        {
            RaisePropertyChanged("LastRedoableCommandName");
            RedoCountChanged(this, EventArgs.Empty);
        }
    }
}
