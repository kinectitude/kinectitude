using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.ComponentModel;

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

        void Log(IUndoableCommand command);
        void Log(string name, Action executeDelegate, Action unexecuteDelegate);
    }
}
