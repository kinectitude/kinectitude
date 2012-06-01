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
        private readonly ObservableCollection<PropertyViewModel> _properties;
        private readonly ModelCollection<PropertyViewModel> properties;
        private Component component;

        public string Name
        {
            get { return descriptor.DisplayName; }
        }

        public PluginDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public IEnumerable<PropertyViewModel> Properties
        {
            get { return properties; }
        }

        public ComponentViewModel(Entity entity, PluginDescriptor descriptor)
        {
            this.entity = entity;
            this.descriptor = descriptor;

            var propertyViewModels = from propertyDescriptor in descriptor.PropertyDescriptors select new PropertyViewModel(entity, propertyDescriptor);
            _properties = new ObservableCollection<PropertyViewModel>(propertyViewModels);
            properties = new ModelCollection<PropertyViewModel>(_properties);
        }
    }
}
