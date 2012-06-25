using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.ViewModels
{
    internal sealed class ComponentViewModel
    {
        private readonly Entity entity;
        private readonly PluginDescriptor descriptor;
        private readonly ObservableCollection<ComponentPropertyViewModel> _properties;
        private readonly ModelCollection<ComponentPropertyViewModel> properties;
        private ComponentViewModel inheritedViewModel;
        private Component component;

        public Component Component
        {
            get { return component; }
        }

        public string Name
        {
            get { return descriptor.DisplayName; }
        }

        public PluginDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public ModelCollection<ComponentPropertyViewModel> Properties
        {
            get { return properties; }
        }

        public bool IsInherited
        {
            get { return null != inheritedViewModel && inheritedViewModel.Component == component; }
        }

        public bool IsLocal
        {
            get { return !IsInherited; }
        }

        public ComponentViewModel(Entity entity, PluginDescriptor descriptor)
        {
            this.entity = entity;
            this.descriptor = descriptor;

            component = entity.GetComponent(descriptor);

            if (null == component)
            {
                foreach (Entity prototype in entity.Prototypes)
                {
                    EntityViewModel prototypeViewModel = EntityViewModel.GetViewModel(prototype);
                    ComponentViewModel inheritedComponentViewModel = prototypeViewModel.GetComponentViewModel(descriptor);

                    if (null != inheritedComponentViewModel)
                    {
                        inheritedViewModel = inheritedComponentViewModel;
                        component = inheritedViewModel.Component;
                        break;
                    }
                }
            }

            if (null == component)
            {
                component = new Component(descriptor);
            }

            var propertyViewModels = from propertyDescriptor in descriptor.PropertyDescriptors select new ComponentPropertyViewModel(entity, descriptor, propertyDescriptor);
            _properties = new ObservableCollection<ComponentPropertyViewModel>(propertyViewModels);
            properties = new ModelCollection<ComponentPropertyViewModel>(_properties);
        }

        public void AddComponent()
        {
            if (IsLocal)
            {
                entity.AddComponent(component);
            }
        }

        public void RemoveComponent()
        {
            if (IsLocal)
            {
                entity.RemoveComponent(component);
            }
        }

        public ComponentPropertyViewModel GetPropertyViewModel(string name)
        {
            return _properties.FirstOrDefault(x => x.Name == name);
        }
    }
}
