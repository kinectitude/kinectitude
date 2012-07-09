using System;

namespace EditorModels.ViewModels.Interfaces
{
    internal interface IPluginNamespace
    {
        event DefineAddedEventHandler DefineAdded;
        event DefinedNameChangedEventHandler DefineChanged;

        string GetDefinedName(PluginViewModel plugin);
        PluginViewModel GetPlugin(string name);
    }
}
