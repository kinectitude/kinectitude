using System.Collections.ObjectModel;
using Kinectitude.Editor.Models.Interfaces;

namespace Kinectitude.Editor.Models
{
    internal abstract class AbstractCondition : AbstractAction, IActionScope
    {
        public event DefineAddedEventHandler DefineAdded;
        public event DefinedNameChangedEventHandler DefineChanged;
        public override event PluginAddedEventHandler PluginAdded;

        public abstract string If { get; set; }

        public override string Type
        {
            get { return null; }
        }

        public ObservableCollection<AbstractAction> Actions
        {
            get;
            private set;
        }

        protected AbstractCondition()
        {
            Actions = new ObservableCollection<AbstractAction>();
        }

        public void AddAction(AbstractAction action)
        {
            action.SetScope(this);
            Actions.Add(action);
            action.PluginAdded += OnActionPluginAdded;

            if (null != PluginAdded)
            {
                foreach (Plugin plugin in action.Plugins)
                {
                    PluginAdded(plugin);
                }
            }
        }

        public void RemoveAction(AbstractAction action)
        {
            if (action.IsLocal)
            {
                PrivateRemoveAction(action);
            }
        }

        protected void PrivateRemoveAction(AbstractAction action)
        {
            action.SetScope(null);
            Actions.Remove(action);
            action.PluginAdded -= OnActionPluginAdded;
        }

        private void OnActionPluginAdded(Plugin plugin)
        {
            if (null != PluginAdded)
            {
                PluginAdded(plugin);
            }
        }

        string IPluginNamespace.GetDefinedName(Plugin plugin)
        {
            return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName;
        }

        Plugin IPluginNamespace.GetPlugin(string name)
        {
            return null != scope ? scope.GetPlugin(name) : Workspace.Instance.GetPlugin(name);
        }
    }
}
