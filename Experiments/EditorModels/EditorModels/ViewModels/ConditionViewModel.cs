using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal class ConditionViewModel : BaseViewModel, IActionScope
    {
        public event ScopeChangedEventHandler ScopeChanged;

        public event DefineAddedEventHandler DefineAdded;

        public event DefinedNameChangedEventHandler DefineChanged;

        public string GetDefinedName(PluginViewModel plugin)
        {
            throw new System.NotImplementedException();
        }

        public PluginViewModel GetPlugin(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
