using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Models.Properties;

namespace Kinectitude.Editor.Models.Plugins
{
    internal sealed class Component : Plugin
    {
        private Entity parent;

        public Entity Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Component(PluginDescriptor descriptor) : base(descriptor) { }
    }
}
