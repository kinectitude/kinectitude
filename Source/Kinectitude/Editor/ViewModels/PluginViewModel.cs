using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.ViewModels
{
    public class PluginViewModel : BaseModel
    {
        private readonly PluginDescriptor descriptor;

        public PluginDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public string Name
        {
            get { return descriptor.DisplayName; }
        }

        public string Type
        {
            get { return Enum.GetName(typeof(PluginDescriptor.PluginType), descriptor.Type); }
        }

        public PluginViewModel(PluginDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }
    }
}
