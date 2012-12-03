
using Kinectitude.Editor.Models.Statements;
namespace Kinectitude.Editor.Models.Interfaces
{
    internal interface IStatementScope : IScope, IPluginNamespace
    {
        void InsertBefore(AbstractStatement statement, AbstractStatement toInsert);
    }
}
