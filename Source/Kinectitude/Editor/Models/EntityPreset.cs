using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal sealed class EntityPreset
    {
        public string Name { get; private set; }

        public IEnumerable<Plugin> Plugins { get; private set; }

        public EntityPreset(string name, params Plugin[] plugins)
        {
            Name = name;
            Plugins = plugins;
        }
    }
}
