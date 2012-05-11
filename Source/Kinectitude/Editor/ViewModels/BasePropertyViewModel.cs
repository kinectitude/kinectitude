using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models.Properties;

namespace Kinectitude.Editor.ViewModels
{
    public class BasePropertyViewModel
    {
        private readonly Property property;

        public Property Property
        {
            get { return property; }
        }

        public string Key
        {
            get { return property.Descriptor.DisplayName; }
        }

        private BasePropertyViewModel(Property property)
        {
            this.property = property;
        }
    }
}
