using System.Collections.Generic;
using System.Collections.ObjectModel;
using EditorModels.Models;
using System.Linq;

namespace EditorModels.ViewModels
{
    internal sealed class EventViewModel : BaseViewModel
    {
        private readonly PluginViewModel plugin;
        private readonly Event evt;
        private Entity entity;

        public event PluginAddedEventHandler PluginAdded;

        public ObservableCollection<ActionViewModel> Actions
        {
            get;
            private set;
        }

        public IEnumerable<PluginViewModel> Plugins
        {
            get { return Actions.SelectMany(x => x.Plugins).Union(Enumerable.Repeat(plugin, 1)); }
        }

        public EventViewModel(PluginViewModel plugin)
        {
            this.plugin = plugin;
            Actions = new ObservableCollection<ActionViewModel>();
        }

        public void SetEntity(Entity entity)
        {
            if (null != this.entity)
            {
                this.entity.RemoveEvent(evt);
            }

            this.entity = entity;

            if (null != this.entity)
            {
                this.entity.AddEvent(evt);
            }
        }

        public void AddAction(ActionViewModel action)
        {
            // TODO: Notify plugin added
        }

        private void RaisePluginAdded(PluginViewModel plugin)
        {
            if (null != PluginAdded)
            {
                PluginAdded(plugin);
            }
        }
    }
}
