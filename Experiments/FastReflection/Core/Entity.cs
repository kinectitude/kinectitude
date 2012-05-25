using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public class Entity
    {
        private readonly Dictionary<Type, Component> components;

        public string Name { get; set; }

        public Entity()
        {
            components = new Dictionary<Type, Component>();
        }

        public void AddComponent(Component component)
        {
            components.Add(component.GetType(), component);
        }
    }
}
