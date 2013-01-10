using Kinectitude.Editor.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Kinectitude.Editor.Commands
{
    internal sealed class CommandHistory : ICommandHistory
    {
        private bool replay;

        public event PropertyChangedEventHandler PropertyChanged;

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
                        IUndoableCommand command = PopUndo();
                        command.Unexecute();
                        PushRedo(command);
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
                        IUndoableCommand command = PopRedo();
                        command.Execute();
                        PushUndo(command);
                    }

                    replay = false;
                }
            );
        }

        private void PushUndo(IUndoableCommand command)
        {
            UndoableCommands.Add(command);
            NotifyPropertyChanged("LastUndoableCommand");
        }

        private void PushRedo(IUndoableCommand command)
        {
            RedoableCommands.Add(command);
            NotifyPropertyChanged("LastRedoableCommand");
        }

        private IUndoableCommand PopUndo()
        {
            IUndoableCommand command = UndoableCommands.Last();
            UndoableCommands.RemoveAt(UndoableCommands.Count - 1);

            NotifyPropertyChanged("LastUndoableCommand");
            return command;
        }

        private IUndoableCommand PopRedo()
        {
            IUndoableCommand command = RedoableCommands.Last();
            RedoableCommands.RemoveAt(RedoableCommands.Count - 1);

            NotifyPropertyChanged("LastRedoableCommand");
            return command;
        }

        public void Log(IUndoableCommand command)
        {
            if (!replay)
            {
                PushUndo(command);
                RedoableCommands.Clear();
            }
        }

        public void Log(string name, Action executeDelegate, Action unexecuteDelegate)
        {
            if (!replay)
            {
                Log(new DelegateUndoableCommand(name, executeDelegate, unexecuteDelegate));
            }
        }

        private void NotifyPropertyChanged(string name)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
