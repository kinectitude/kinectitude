using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.ViewModels
{
    public class ComponentViewModel
    {
        private readonly Component component;
        private readonly List<BasePropertyViewModel> _properties;
        private readonly ReadOnlyCollection<BasePropertyViewModel> properties;

        public Component Component
        {
            get { return component; }
        }

        public string Name
        {
            get { return component.Descriptor.DisplayName; }
        }

        public IEnumerable<BasePropertyViewModel> Properties
        {
            get { return properties; }
        }

        public ComponentViewModel(Component component)
        {
            this.component = component;

            var propertyViewModels = from property in component.Properties select BasePropertyViewModel.Create(property);
            _properties = new List<BasePropertyViewModel>(propertyViewModels);
            properties = new ReadOnlyCollection<BasePropertyViewModel>(_properties);
        }
    }
}
