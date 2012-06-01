using Kinectitude.Editor.Models.Base;

namespace Kinectitude.Editor.Models.Plugins
{
    internal sealed class Action : Plugin, IAction
    {
        private IActionContainer parent;

        public IActionContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Action(PluginDescriptor descriptor) : base(descriptor) { }
    }
}
