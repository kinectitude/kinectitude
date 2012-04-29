using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public interface IEntityContainer
    {
        void AddEntity(Entity entity);
        void RemoveEntity(Entity entity);
    }
}
