using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Kinectitude.Editor.Base;

namespace Kinectitude.Editor.Commands.Base
{
    public class CommandHistory : BaseModel
    {
        private static readonly CommandHistory instance;

        public static CommandHistory Instance
        {
            get { return instance; }
        }

        static CommandHistory()
        {
            instance = new CommandHistory();
        }

        private readonly Stack<IUndoableCommand> undo;
        private readonly Stack<IUndoableCommand> redo;
        //private static event EventHandler undoCountChanged;
        //private static event EventHandler redoCountChanged;
        private bool replay;

        private CommandHistory()
        {
            undo = new Stack<IUndoableCommand>();
            redo = new Stack<IUndoableCommand>();
            //undoCountChanged = delegate { };
            //redoCountChanged = delegate { };
            replay = false;
        }

        public ICommand UndoCommand
        {
            get { return new DelegateCommand(CanUndo, Undo); }
        }

        public ICommand RedoCommand
        {
            get { return new DelegateCommand(CanRedo, Redo); }
        }

        /*public static int UndoCount
        {
            get { return undo.Count; }
        }

        public static int RedoCount
        {
            get { return redo.Count; }
        }*/

        public string LastUndoableCommandName
        {
            get { return undo.Count > 0 ? undo.Peek().Name : null; }
        }

        public string LastRedoableCommandName
        {
            get { return redo.Count > 0 ? redo.Peek().Name : null; }
        }

        /*public static event EventHandler UndoCountChanged
        {
            add { undoCountChanged += value; }
            remove { undoCountChanged -= value; }
        }

        public static event EventHandler RedoCountChanged
        {
            add { redoCountChanged += value; }
            remove { redoCountChanged -= value; }
        }*/

        public void LogCommand(IUndoableCommand command)
        {
            if (!replay)
            {
                undo.Push(command);
                redo.Clear();
                RaisePropertyChanges();
            }
        }

        public bool CanUndo(object parameter)
        {
            return undo.Count > 0;
        }

        public bool CanRedo(object parameter)
        {
            return redo.Count > 0;
        }

        public void Undo(object parameter)
        {
            replay = true;

            if (undo.Count > 0)
            {
                IUndoableCommand command = undo.Pop();
                command.Unexecute();
                redo.Push(command);
                RaisePropertyChanges();
            }

            replay = false;
        }

        public void Redo(object parameter)
        {
            replay = true;

            if (redo.Count > 0)
            {
                IUndoableCommand command = redo.Pop();
                command.Execute();
                undo.Push(command);
                RaisePropertyChanges();
            }

            replay = false;
        }

        private void RaisePropertyChanges()
        {
            RaisePropertyChanged("LastUndoableCommandName");
            RaisePropertyChanged("LastRedoableCommandName");
        }

        /*public static void PushRedo(IUndoableCommand command)
        {
            redo.Push(command);
            RaiseRedoChange();
        }*/

        /*public IUndoableCommand PopUndo()
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
        }*/

        /*private static void RaiseUndoChange()
        {
            RaisePropertyChanged("LastUndoableCommandName");
            UndoCountChanged(this, EventArgs.Empty);
        }

        private static void RaiseRedoChange()
        {
            RaisePropertyChanged("LastRedoableCommandName");
            RedoCountChanged(this, EventArgs.Empty);
        }*/
    }
}
