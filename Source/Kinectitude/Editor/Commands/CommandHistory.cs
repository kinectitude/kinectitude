using System.Collections.ObjectModel;
using System.Linq;
using Kinectitude.Editor.Base;

namespace Kinectitude.Editor.Commands
{
    internal sealed class CommandHistory : ICommandHistory
    {
        private bool replay;

        public ObservableCollection<IUndoableCommand> UndoableCommands
        {
            get;
            private set;
        }

        public ObservableCollection<IUndoableCommand> RedoableCommands
        {
            get;
            private set;
        }

        public IUndoableCommand LastUndoableCommand
        {
            get { return UndoableCommands.LastOrDefault(); }
        }

        public IUndoableCommand LastRedoableCommand
        {
            get { return RedoableCommands.LastOrDefault(); }
        }

        public ICommand UndoCommand
        {
            get;
            private set;
        }

        public ICommand RedoCommand
        {
            get;
            private set;
        }

        public CommandHistory()
        {
            UndoableCommands = new ObservableCollection<IUndoableCommand>();
            RedoableCommands = new ObservableCollection<IUndoableCommand>();

            UndoCommand = new DelegateCommand(
                (parameter) => UndoableCommands.Count > 0,
                (parameter) =>
                {
                    replay = true;

                    if (UndoableCommands.Count > 0)
                    {
                        IUndoableCommand command = UndoableCommands.Last();
                        UndoableCommands.RemoveAt(UndoableCommands.Count - 1);
                        command.Unexecute();
                        RedoableCommands.Add(command);
                    }

                    replay = false;
                }
            );

            RedoCommand = new DelegateCommand(
                (parameter) => RedoableCommands.Count > 0,
                (parameter) =>
                {
                    replay = true;

                    if (RedoableCommands.Count > 0)
                    {
                        IUndoableCommand command = RedoableCommands.Last();
                        RedoableCommands.RemoveAt(RedoableCommands.Count - 1);
                        command.Execute();
                        UndoableCommands.Add(command);
                    }

                    replay = false;
                }
            );
        }

        public void Log(IUndoableCommand command)
        {
            if (!replay)
            {
                UndoableCommands.Add(command);
                RedoableCommands.Clear();
            }
        }
    }
}
