using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModels.Models
{
    internal interface IEntityContainer
    {
        IEnumerable<Entity> Entities { get; }

        void AddEntity(Entity entity);
        void RemoveEntity(Entity entity);
    }
}
