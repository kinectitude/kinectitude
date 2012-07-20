
namespace Kinectitude.Editor.Commands
{
    internal interface IUndoableCommand
    {
        string Name { get; }

        void Execute();
        void Unexecute();
    }
}
