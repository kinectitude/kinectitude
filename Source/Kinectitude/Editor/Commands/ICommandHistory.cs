using System.Collections.ObjectModel;

namespace Kinectitude.Editor.Commands
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
