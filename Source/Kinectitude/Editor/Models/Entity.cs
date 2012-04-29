using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Editor.Base;

namespace Editor
{
    public class Entity : AttributeContainer, IEventContainer
    {
        private IEntityContainer parent;
        private string name;

        private readonly List<Entity> _prototypes;
        private readonly List<Component> _components;
        private readonly List<Event> _events;
        private readonly ReadOnlyCollection<Entity> prototypes;
        private readonly ReadOnlyCollection<Component> components;
        private readonly ReadOnlyCollection<Event> events;

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

        public ReadOnlyCollection<Entity> Prototypes
        {
            get { return prototypes; }
        }

        public ReadOnlyCollection<Component> Components
        {
            get { return components; }
        }

        public ReadOnlyCollection<Event> Events
        {
            get { return events; }
        }

        public Entity()
        {
            _prototypes = new List<Entity>();
            _components = new List<Component>();
            _events = new List<Event>();

            prototypes = new ReadOnlyCollection<Entity>(_prototypes);
            components = new ReadOnlyCollection<Component>(_components);
            events = new ReadOnlyCollection<Event>(_events);
        }

        public void AddPrototype(Entity entity)
        {
            _prototypes.Add(entity);
        }

        public void RemovePrototype(Entity entity)
        {
            _prototypes.Remove(entity);
        }

        public void AddComponent(Component component)
        {
            component.Parent = this;
            _components.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            component.Parent = null;
            _components.Remove(component);
        }

        public void AddEvent(Event evt)
        {
            evt.Parent = this;
            _events.Add(evt);
        }

        public void RemoveEvent(Event evt)
        {
            evt.Parent = null;
            _events.Remove(evt);
        }

        public override T GetAttribute<T>(string key)
        {
            BaseAttribute attribute = base.GetAttribute<T>(key);
            if (null == attribute)
            {
                foreach (Entity prototype in Prototypes)
                {
                    attribute = prototype.GetAttribute<T>(key);
                    if (null != attribute)
                    {
                        break;
                    }
                }
            }
            return attribute as T;
        }

        public bool HasComponent(string name)
        {
            return null != GetComponent(name);
        }

        public Component GetComponent(string name)
        {
            Component component = Components.FirstOrDefault(x => x.Descriptor.Name == name);
            if (null == component)
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

        public T GetPropertyForComponent<T>(string componentName, string key) where T : BaseProperty
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
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Prototype: {1}", Name != null ? Name : string.Empty, Prototypes[0] != null ? Prototypes[0].Name : string.Empty);
        }
    }
}
