using Kinectitude.Editor.Models.Plugins;
using System.Collections.ObjectModel;
using Kinectitude.Editor.Base;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class EventViewModel
    {
        private readonly Event evt;
        private readonly ObservableCollection<ActionViewModel> _actions;
        private readonly ModelCollection<ActionViewModel> actions;

        public string Name
        {
            get { return evt.Descriptor.DisplayName; }
        }

        public ModelCollection<ActionViewModel> Actions
        {
            get { return actions; }
        }

        public EventViewModel(Event evt)
        {
            this.evt = evt;

            //var actionViewModels = from action in evt.Actions select new ActionViewModel(action);
        }
    }
}
