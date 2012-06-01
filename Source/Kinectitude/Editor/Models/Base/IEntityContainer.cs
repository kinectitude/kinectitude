using System.Collections.Generic;

namespace Kinectitude.Editor.Models.Base
{
    internal interface IEntityContainer
    {
        IEnumerable<Entity> Entities { get; }

        void AddEntity(Entity entity);
        void RemoveEntity(Entity entity);
    }
}
