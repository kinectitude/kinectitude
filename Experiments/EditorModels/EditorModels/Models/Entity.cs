using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal sealed class Entity : IAttributeContainer
    {
        private readonly List<string> prototypes;
        private readonly List<Attribute> attributes;
        private readonly List<Component> components;
        private readonly List<Event> events;

        public string Name
        {
            get;
            set;
        }

        public IEnumerable<string> Prototypes
        {
            get { return prototypes; }
        }

        public IEnumerable<Attribute> Attributes
        {
            get { return attributes; }
        }

        public IEnumerable<Component> Components
        {
            get { return components; }
        }

        public IEnumerable<Event> Events
        {
            get { return events; }
        }

        public Entity()
        {
            prototypes = new List<string>();
            attributes = new List<Attribute>();
            components = new List<Component>();
            events = new List<Event>();
        }

        public void AddPrototype(string name)
        {
            prototypes.Add(name);
        }

        public void RemovePrototype(string name)
        {
            prototypes.Remove(name);
        }

        public void ReplacePrototype(string oldName, string newName)
        {
            int index = prototypes.IndexOf(oldName);
            prototypes[index] = newName;
        }

        public void AddAttribute(Attribute attribute)
        {
            attributes.Add(attribute);
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attributes.Remove(attribute);
        }

        public void AddComponent(Component component)
        {
            components.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);
        }

        public void AddEvent(Event evt)
        {
            events.Add(evt);
        }

        public void RemoveEvent(Event evt)
        {
            events.Remove(evt);
        }
    }
}
