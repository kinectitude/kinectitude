using System;
using System.Collections.Generic;

namespace Kinectitude.Core.Base
{
    internal class Entity : DataContainer, IDataContainer
    {
        //used to get a specific component
        private readonly Dictionary<Type, Component> componentDictionary = new Dictionary<Type, Component>();

        //used by ComponentValueReader to get the component
        private readonly Dictionary<string, Component> componentNameDictionary = new Dictionary<string, Component>();

        //used so that all components can be ready when the entity is ready
        private readonly List<Component> componentList = new List<Component>();

        //Used to automatically unsuscribe all components' and events' change listeners
        internal readonly List<Tuple<DataContainer, string, Action<string>>> Changes = 
            new List<Tuple<DataContainer, string, Action<string>>>();

        internal Scene Scene { get; set; }

        internal T GetComponent<T>() where T : Component
        {
            Component component = null;
            componentDictionary.TryGetValue(typeof(T), out component);
            return component as T;
        }

        //used by ComponentValueReader
        internal Component GetComponent(string name)
        {
            Component component = null;
            componentNameDictionary.TryGetValue(name, out component);
            return component;
        }

        internal Entity(int id) : base(id) { }

        internal void AddComponent(Component component, string name)
        {
            componentDictionary[component.ImplementationType] = component;
            componentNameDictionary[name] = component;
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
            foreach (Tuple<DataContainer, string, Action<string>> unsubscribe in Changes)
            {
                unsubscribe.Item1.StopNotifications(unsubscribe.Item2, unsubscribe.Item3);
            }

            foreach (Component component in componentList)
            {
                component.Destroy();
            }
        }
    }
}