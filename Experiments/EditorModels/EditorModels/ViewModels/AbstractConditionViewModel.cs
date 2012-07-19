using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorModels.ViewModels.Interfaces;
using System.Collections.ObjectModel;

namespace EditorModels.ViewModels
{
    internal abstract class AbstractConditionViewModel : AbstractActionViewModel, IActionScope
    {
        public event DefineAddedEventHandler DefineAdded;
        public event DefinedNameChangedEventHandler DefineChanged;

        public abstract string If { get; set; }

        public override string Type
        {
            get { return null; }
        }

        public ObservableCollection<AbstractActionViewModel> Actions
        {
            get;
            private set;
        }

        protected AbstractConditionViewModel()
        {
            Actions = new ObservableCollection<AbstractActionViewModel>();
        }

        public void AddAction(AbstractActionViewModel action)
        {
            action.SetScope(this);
            Actions.Add(action);
        }

        public void RemoveAction(AbstractActionViewModel action)
        {
            if (action.IsLocal)
            {
                PrivateRemoveAction(action);
            }
        }

        protected void PrivateRemoveAction(AbstractActionViewModel action)
        {
            action.SetScope(null);
            Actions.Remove(action);
        }

        string IPluginNamespace.GetDefinedName(PluginViewModel plugin)
        {
            return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        PluginViewModel IPluginNamespace.GetPlugin(string name)
        {
            return null != scope ? scope.GetPlugin(name) : Workspace.Instance.GetPlugin(name);
        }
    }
}
