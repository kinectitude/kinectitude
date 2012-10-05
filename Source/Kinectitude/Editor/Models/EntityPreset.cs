using System.Collections.Generic;

namespace Kinectitude.Editor.Models
{
    internal sealed class EntityPreset
    {
        public string Name { get; private set; }

        public IEnumerable<Plugin> Components { get; private set; }

        public EntityPreset(string name, params Plugin[] components)
        {
            Name = name;
            Components = components;
        }
    }
}
