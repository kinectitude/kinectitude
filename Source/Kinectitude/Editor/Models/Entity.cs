using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Plugins;
using Kinectitude.Editor.Models.Properties;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.Models
{
    public class Entity : AttributeContainer, IEventContainer
    {
        private IEntityContainer parent;
        private string name;

        /*private readonly List<Entity> _prototypes;
        private readonly List<Component> _components;
        private readonly List<Event> _events;
        private readonly ReadOnlyCollection<Entity> prototypes;
        private readonly ReadOnlyCollection<Component> components;
        private readonly ReadOnlyCollection<Event> events;*/

        private readonly SortedDictionary<string, Entity> prototypes;
        private readonly SortedDictionary<string, Component> components;
        private readonly List<Event> events;

        public IEntityContainer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public IEnumerable<Entity> Prototypes
        {
            get { return prototypes.Values; }
        }

        public IEnumerable<Component> Components
        {
            get { return components.Values; }
        }

        public IEnumerable<Event> Events
        {
            get { return events; }
        }

        public Entity()
        {
            //_prototypes = new List<Entity>();
            //_components = new List<Component>();
            //_events = new List<Event>();

            //prototypes = new ReadOnlyCollection<Entity>(_prototypes);
            //components = new ReadOnlyCollection<Component>(_components);
            //events = new ReadOnlyCollection<Event>(_events);

            prototypes = new SortedDictionary<string, Entity>();
            components = new SortedDictionary<string, Component>();
            events = new List<Event>();
        }

        public void AddPrototype(Entity entity)
        {
            prototypes.Add(entity.Name, entity);
        }

        public void RemovePrototype(Entity entity)
        {
            prototypes.Remove(entity.Name);
        }

        public void AddComponent(Component component)
        {
            component.Parent = this;
            components.Add(component.Descriptor.Name, component);
        }

        public void RemoveComponent(Component component)
        {
            component.Parent = null;
            components.Remove(component.Descriptor.Name);
        }

        public void AddEvent(Event evt)
        {
            evt.Parent = this;
            events.Add(evt);
        }

        public void RemoveEvent(Event evt)
        {
            evt.Parent = null;
            events.Remove(evt);
        }

        public override Attribute GetAttribute(string key)
        {
            Attribute attribute = base.GetAttribute(key);
            if (null == attribute)
            {
                foreach (Entity prototype in Prototypes)
                {
                    attribute = prototype.GetAttribute(key);
                    if (null != attribute)
                    {
                        break;
                    }
                }
            }
            return attribute;
        }

        /*public bool HasComponent(string name)
        {
            return null != GetComponent(name);
        }*/

        public Component GetComponent(string name)
        {
            Component component = null;
            if (components.ContainsKey(name))
            {
                component = components[name];
            }
            else
            {
                foreach (Entity prototype in Prototypes)
                {
                    component = prototype.GetComponent(name);
                    if (null != component)
                    {
                        break;
                    }
                }
            }
            return component;
        }

        /*public T GetPropertyForComponent<T>(string componentName, string key) where T : BaseProperty
        {
            BaseProperty property = null;
            Component component = GetComponent(componentName);
            if (null != component)
            {
                property = component.GetProperty(key);
            }
            if (null == property || property.IsInherited)
            {
                foreach (Entity prototype in Prototypes)
                {
                    component = prototype.GetComponent(componentName);
                    if (null != component)
                    {
                        property = component.GetProperty(key);
                        if (null != property)
                        {
                            break;
                        }
                    }
                }
            }
            return property as T;
        }*/
    }
}
