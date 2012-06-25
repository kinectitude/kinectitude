using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class ComponentPropertyViewModel : BaseModel
    {
        private readonly Entity entity;
        private readonly PluginDescriptor componentDescriptor;
        private readonly PropertyDescriptor propertyDescriptor;
        private ComponentPropertyViewModel inheritedViewModel;
        private Property property;

        public Property Property
        {
            get { return property; }
        }

        public string Name
        {
            get { return propertyDescriptor.Name; }
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
            get { return  null == property || null != inheritedViewModel && property == inheritedViewModel.Property; }
            set
            {

            }
        }

        public string Value
        {
            get { return null != property ? property.ToString() : propertyDescriptor.DefaultValue; }
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

        public ComponentPropertyViewModel(Entity entity, PluginDescriptor componentDescriptor, PropertyDescriptor propertyDescriptor)
        {
            this.entity = entity;
            this.componentDescriptor = componentDescriptor;
            this.propertyDescriptor = propertyDescriptor;

            Component component = entity.GetComponent(componentDescriptor);
            if (null != component)
            {
                property = component.GetProperty<Property>(Name);
            }

            foreach (Entity prototype in entity.Prototypes)
            {
                EntityViewModel prototypeViewModel = EntityViewModel.GetViewModel(prototype);
                ComponentViewModel componentViewModel = prototypeViewModel.GetComponentViewModel(componentDescriptor);

                if (null != componentViewModel)
                {
                    inheritedViewModel = componentViewModel.GetPropertyViewModel(Name);
                    break;
                }
            }

            if (null != inheritedViewModel)
            {
                if (null == property)
                {
                    property = inheritedViewModel.Property;
                }
            }
        }
    }
}
