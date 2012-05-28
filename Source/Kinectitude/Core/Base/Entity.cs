using System;
using System.Collections.Generic;

namespace Kinectitude.Core.Base
{
    internal class Entity : DataContainer, IDataContainer
    {
        //used to get a specific component
        private readonly Dictionary<Type, Component> componentDictionary;
        //used so that all components can be ready when the entity is ready
        private readonly List<Component> componentList = new List<Component>();

        internal Scene Scene { get; set; }

        public T GetComponent<T>() where T : Component
        {
            if (!componentDictionary.ContainsKey(typeof(T)))
            {
                return null;
            }
            return componentDictionary[typeof(T)] as T;
        }

        internal Entity(int id) : base(id)
        {
            componentDictionary = new Dictionary<Type, Component>();
        }

        internal void AddComponent(Component component)
        {
            componentDictionary[component.ImplementationType] = component;
            componentList.Add(component);
        }

        internal void Ready()
        {
            foreach (Component component in componentList)
            {
                component.Ready();
            }
        }

        internal void Destroy()
        {
            foreach (Component component in componentList)
            {
                component.Destroy();
            }
        }
    }
}