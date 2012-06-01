
namespace Kinectitude.Editor.Commands.Base
{
    internal interface IUndoableCommand
    {
        string Name { get; }

        void Execute();
        void Unexecute();
    }
}
