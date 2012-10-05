using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Views
{
    internal class DragDropData
    {
        public Plugin Plugin
        {
            get;
            private set;
        }

        public DragDropData(Plugin plugin)
        {
            Plugin = plugin;
        }
    }
}
