using System;
using System.Collections.Generic;

namespace Kinectitude.Core.Base
{
    public class Entity : DataContainer
    {
        //used to get a specific component
        private readonly Dictionary<Type, Component> componentDictionary;
        //used so that all components can be ready when the entity is ready
        private readonly List<Component> componentList = new List<Component>();

        public Entity(int id) : base(id)
        {
            componentDictionary = new Dictionary<Type, Component>();
        }

        public void AddComponent(Component component)
        {
            componentDictionary[component.ImplementationType()] = component;
            componentList.Add(component);
        }

        public Component GetComponent(Type type)
        {
            if (!componentDictionary.ContainsKey(type))
            {
                return null;
            }
            return componentDictionary[type];
        }

        internal void Ready()
        {
            foreach (Component component in componentList)
            {
                component.Ready();
            }
        }
    }
}