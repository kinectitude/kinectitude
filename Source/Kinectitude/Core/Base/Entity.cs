using System;
using System.Collections.Generic;
using Kinectitude.Core.Events;
using Kinectitude.Core.Data;
using SysAction = System.Action;

namespace Kinectitude.Core.Base
{
    internal sealed class Entity : DataContainer, IEntity
    {
        //used to get a specific component
        private readonly Dictionary<Type, Component> componentDictionary = new Dictionary<Type, Component>();

        //used by ComponentValueReader to get the component
        private readonly Dictionary<string, Component> componentNameDictionary = new Dictionary<string, Component>();

        //used so that all components can be ready when the entity is ready
        private readonly List<Component> componentList = new List<Component>();

        //Used to automatically unsuscribe all components' and events' change listeners
        internal readonly List<Tuple<DataContainer, string, SysAction>> Changes = 
            new List<Tuple<DataContainer, string, SysAction>>();

        private readonly List<OnCreateEvent> CreateEvents = new List<OnCreateEvent>();

        private readonly List<Event> Events = new List<Event>();

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
            if (!componentNameDictionary.TryGetValue(name, out component))
            {
                string identity = null != Name ? Name : "An unnamed entity";
                Game.CurrentGame.Die(identity + " does not have a " + name + " component");
            }
            return component;
        }

        internal override Changeable GetChangeable(string name)
        {
            return GetComponent(name);
        }

        internal Entity(int id) : base(id) { }

        internal void AddComponent(Component component, string name)
        {
            componentDictionary[component.GetType()] = component;
            componentNameDictionary[name] = component;
            componentList.Add(component);

            foreach (Type type in ClassFactory.GetProvided(component.GetType()))
            {
                componentDictionary[type] = component;
            }
            component.DataContainer = this;
        }

        internal void Ready()
        {
            foreach (Component component in componentList) component.Ready();
            foreach (OnCreateEvent evt in CreateEvents) evt.DoActions();
        } 

        internal void Destroy()
        {
            foreach (Tuple<DataContainer, string, SysAction> unsubscribe in Changes) 
                unsubscribe.Item1.StopNotifications(unsubscribe.Item2, unsubscribe.Item3);

            foreach (Tuple<DataContainer, string, SysAction> unsubscribe in PropertyChanges)
                unsubscribe.Item1.UnnotifyOfComponentChange(unsubscribe.Item2, unsubscribe.Item3);

            foreach (Event evt in Events) evt.Destroy();

            foreach (Component component in componentList)
            {
                ParameterValueReader.DeleteObject(component);
                component.Destroy();
            }

            DataContainerReader.DeleteDataContainer(this);
            Scene.DeleteEntity(this);
            Deleted = true;
        }

        internal void addEvent(Event evt)
        {
            Events.Add(evt);
        }

        internal void addOnCreate(OnCreateEvent evt)
        {
            CreateEvents.Add(evt);
        }
    }
}