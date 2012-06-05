using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class ComponentPropertyViewModel : BaseModel
    {
        private readonly Entity entity;
        private readonly PluginDescriptor descriptor;
        private ComponentPropertyViewModel inheritedViewModel;
        private Property property;

        public Property Property
        {
            get { return property; }
        }

        public string Name
        {
            get { return descriptor.DisplayName; }
        }

        public bool CanInherit
        {
            get { return true; }
        }

        public bool IsLocal
        {
            get { return !IsInherited; }
        }

        public bool IsInherited
        {
            get { return null != inheritedViewModel && property == inheritedViewModel.Property; }
            set
            {

            }
        }

        public string Value
        {
            get { return property.ToString(); }
            set
            {
                if (!IsInherited)
                {
                    bool success = property.TryParse(value);
                    if (success)
                    {
                        RaisePropertyChanged("Value");
                    }
                }
            }
        }

        public ComponentPropertyViewModel(Entity entity, string name, PluginDescriptor descriptor)
        {
            this.entity = entity;
            this.descriptor = descriptor;
        }
    }
}
