using System;
using System.Collections.Generic;
using Kinectitude.Core.Exceptions;

namespace Kinectitude.Core.Base
{
    internal sealed class Entity : DataContainer, IEntity
    {
        //used to get a specific component
        private readonly Dictionary<Type, Component> componentDictionary = new Dictionary<Type, Component>();

        //used by ComponentValueReader to get the component
        private readonly Dictionary<string, Component> componentNameDictionary = new Dictionary<string, Component>();

        //used to see that everything needed is provided
        private readonly List<Type> needs = new List<Type>();

        //used so that all components can be ready when the entity is ready
        private readonly List<Component> componentList = new List<Component>();

        //Used to automatically unsuscribe all components' and events' change listeners
        internal readonly List<Tuple<DataContainer, string, Action<string>>> Changes = 
            new List<Tuple<DataContainer, string, Action<string>>>();

        internal Scene Scene { get; set; }

        internal T GetComponent<T>() where T : class
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
            componentDictionary[component.GetType()] = component;
            componentNameDictionary[name] = component;
            componentList.Add(component);

            foreach (Type type in ClassFactory.GetRequirements(component.GetType()))
            {
                needs.Add(type);
            }

            foreach (Type type in ClassFactory.GetProvided(component.GetType()))
            {
                componentDictionary[type] = component;
            }
        }

        internal void Ready()
        {
            List<Type> missing = new List<Type>();
            foreach (Type type in needs)
            {
                if (!componentDictionary.ContainsKey(type))
                {
                    if (!missing.Contains(type))
                    {
                        missing.Add(type);
                    }
                }
            }

            if (missing.Count != 0)
            {
                string identity = null != Name ? Name : "Entity " + Id.ToString();
                throw MissingRequirementsException.MissingRequirement(identity, missing);
            }

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

            foreach (Tuple<DataContainer, string, Action<string>> unsubscribe in PropertyChanges)
            {
                unsubscribe.Item1.UnnotifyOfComponentChange(unsubscribe.Item2, unsubscribe.Item3);
            }

            foreach (Component component in componentList)
            {
                component.Destroy();
            }
        }

        internal override object GetComponentOrManager(string name)
        {
            return GetComponent(name);
        }
    }
}