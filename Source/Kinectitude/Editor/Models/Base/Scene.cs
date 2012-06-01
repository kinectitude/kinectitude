using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Base
{
    internal sealed class Scene : AttributeContainer, IEntityContainer
    {
        private Game parent;
        private string name;
        private readonly List<Entity> entities;

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

        public IEnumerable<Entity> Entities
        {
            get { return entities; }
        }

        public Scene()
        {
            entities = new List<Entity>();
        }

        public void AddEntity(Entity entity)
        {
            entity.Parent = this;
            entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Parent = null;
            entities.Remove(entity);
        }
    }
}
