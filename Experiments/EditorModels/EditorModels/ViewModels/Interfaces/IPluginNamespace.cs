using System;

namespace EditorModels.ViewModels
{
    internal interface IPluginNamespace
    {
        event ScopeChangedEventHandler ScopeChanged;
        event DefineAddedEventHandler DefineAdded;
        event DefinedNameChangedEventHandler DefinedNameChanged;

        string GetDefinedName(PluginViewModel plugin);
        PluginViewModel GetPlugin(string name);
    }
}
