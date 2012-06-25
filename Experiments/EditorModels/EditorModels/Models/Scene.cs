using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal sealed class Scene : IEntityContainer, IAttributeContainer
    {
        private readonly List<Attribute> attributes;
        private readonly List<Manager> managers;
        private readonly List<Entity> entities;

        public string Name
        {
            get;
            set;
        }

        public IEnumerable<Attribute> Attributes
        {
            get { return attributes; }
        }

        public IEnumerable<Manager> Managers
        {
            get { return managers; }
        }

        public IEnumerable<Entity> Entities
        {
            get { return entities; }
        }

        public Scene()
        {
            attributes = new List<Attribute>();
            managers = new List<Manager>();
            entities = new List<Entity>();
        }

        public void AddAttribute(Attribute attribute)
        {
            attributes.Add(attribute);
        }

        public void RemoveAttribute(Attribute attribute)
        {
            attributes.Remove(attribute);
        }

        public void AddManager(Manager manager)
        {
            managers.Add(manager);
        }

        public void RemoveManager(Manager manager)
        {
            managers.Remove(manager);
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }
    }
}
