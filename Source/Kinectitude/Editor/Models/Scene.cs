using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Base;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Editor
{
    public class Scene : AttributeContainer, IEntityContainer, IEventContainer
    {
        private Game parent;
        private string name;

        private readonly List<Entity> _entities;
        private readonly List<Event> _events;
        private readonly ReadOnlyCollection<Entity> entities;
        private readonly ReadOnlyCollection<Event> events;

        public Game Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ReadOnlyCollection<Entity> Entities
        {
            get { return entities; }
        }

        public ReadOnlyCollection<Event> Events
        {
            get { return events; }
        }

        public Scene()
        {
            _entities = new List<Entity>();
            entities = new ReadOnlyCollection<Entity>(_entities);

            _events = new List<Event>();
            events = new ReadOnlyCollection<Event>(_events);
        }

        public Entity GetPrototypeEntityByName(string name)
        {
            Entity prototype = null;
            if (null != Parent)
            {
                prototype = Parent.GetPrototypeEntityByName(name);
            }
            return prototype;
        }

        public void AddEntity(Entity entity)
        {
            entity.Parent = this;
            _entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Parent = null;
            _entities.Remove(entity);
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

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }
    }
}
