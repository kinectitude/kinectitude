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

        public override T GetProperty<T>(string name)
        {
            Property property = base.GetProperty<T>(name);

            if (null == property)
            {
                foreach (Entity prototype in parent.Prototypes)
                {
                    Component ancestor = prototype.GetComponent(Descriptor.ClassName);
                    if (null != ancestor)
                    {
                        property = ancestor.GetProperty<T>(name);
                        if (null != property)
                        {
                            break;
                        }
                    }
                }
            }
            return property as T;
        }
    }
}
