using System.Windows.Input;
using Kinectitude.Editor.Base;

namespace Kinectitude.Editor.Commands.Base
{
    public static class CommandHistory
    {
        
        private static readonly ObservableStack<IUndoableCommand> undo;
        private static readonly ObservableStack<IUndoableCommand> redo;
        private static bool replay;

        static CommandHistory()
        {
            undo = new ObservableStack<IUndoableCommand>();
            redo = new ObservableStack<IUndoableCommand>();
            replay = false;
        }

        public static ICommand UndoCommand
        {
            get { return new DelegateCommand(CanUndo, Undo); }
        }

        public static ICommand RedoCommand
        {
            get { return new DelegateCommand(CanRedo, Redo); }
        }

        public static ObservableStack<IUndoableCommand> UndoableCommands
        {
            get { return undo; }
        }

        public static ObservableStack<IUndoableCommand> RedoableCommands
        {
            get { return redo; }
        }

        public static void LogCommand(IUndoableCommand command)
        {
            if (!replay)
            {
                undo.Push(command);
                redo.Clear();
            }
        }

        public static bool CanUndo(object parameter)
        {
            return undo.Count > 0;
        }

        public static bool CanRedo(object parameter)
        {
            return redo.Count > 0;
        }

        public static void Undo(object parameter)
        {
            replay = true;

            if (undo.Count > 0)
            {
                IUndoableCommand command = undo.Pop();
                command.Unexecute();
                redo.Push(command);
            }

            replay = false;
        }

        public static void Redo(object parameter)
        {
            replay = true;

            if (redo.Count > 0)
            {
                IUndoableCommand command = redo.Pop();
                command.Execute();
                undo.Push(command);
            }

            replay = false;
        }
    }
}
