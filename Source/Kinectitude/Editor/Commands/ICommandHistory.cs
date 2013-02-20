using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Kinectitude.Editor.Commands
{
    internal interface ICommandHistory : INotifyPropertyChanged
    {
        ObservableCollection<IUndoableCommand> UndoableCommands { get; }
        ObservableCollection<IUndoableCommand> RedoableCommands { get; }

        IUndoableCommand LastUndoableCommand { get; }
        IUndoableCommand LastRedoableCommand { get; }

        ICommand UndoCommand { get; }
        ICommand RedoCommand { get; }

        void Undo();
        void Redo();
        void Clear();

        void Log(IUndoableCommand command);
        void Log(string name, Action executeDelegate, Action unexecuteDelegate);
        void WithoutLogging(Action action);
    }
}
