using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Base;

namespace Editor.ViewModels
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
            get { return descriptor.Name; }
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
