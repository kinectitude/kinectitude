using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IStatementScope : IScope, IPluginNamespace
    {
        bool ShouldCopyStatement { get; }

        int IndexOf(AbstractStatement statement);
        void RemoveStatement(AbstractStatement statement);
        void InsertAt(int idx, AbstractStatement statement);
        void InsertBefore(AbstractStatement statement, AbstractStatement toInsert);
    }
}
