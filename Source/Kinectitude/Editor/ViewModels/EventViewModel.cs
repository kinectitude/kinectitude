using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class EventViewModel
    {
        private readonly Event evt;

        public EventViewModel(Event evt)
        {
            this.evt = evt;
        }
    }
}
