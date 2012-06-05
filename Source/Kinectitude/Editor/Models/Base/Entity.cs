using System.Collections.Generic;
using Kinectitude.Editor.Models.Plugins;

namespace Kinectitude.Editor.Models.Base
{
    internal sealed class Entity : AttributeContainer, IEventContainer
    {
        private IEntityContainer parent;
        private string name;

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
            components.Add(component.Descriptor.ClassName, component);
        }

        public void RemoveComponent(Component component)
        {
            component.Parent = null;
            components.Remove(component.Descriptor.ClassName);
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

        public Component GetComponent(PluginDescriptor descriptor)
        {
            Component ret = null;

            foreach (Component component in components.Values)
            {
                if (component.Descriptor == descriptor)
                {
                    ret = component;
                    break;
                }
            }

            return ret;
        }
    }
}
