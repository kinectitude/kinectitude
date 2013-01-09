using Kinectitude.Editor.Models.Statements.Base;

namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IStatementScope : IScope, IPluginNamespace
    {
        void RemoveStatement(AbstractStatement statement);
        void InsertBefore(AbstractStatement statement, AbstractStatement toInsert);
    }
}
