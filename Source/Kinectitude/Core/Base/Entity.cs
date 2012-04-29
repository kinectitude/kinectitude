using System;
using System.Collections.Generic;

namespace Kinectitude.Core
{
    public class Entity : DataContainer
    {
        private readonly Dictionary<Type, Component> components;

        public Entity(int id) : base(id)
        {
            components = new Dictionary<Type, Component>();
        }

        public void AddComponent(Component component)
        {
            components[component.GetType()] = component;
        }

        public Component GetComponent(Type type)
        {
            if (!components.ContainsKey(type))
            {
                return null;
            }
            return components[type];
        }
    }
}