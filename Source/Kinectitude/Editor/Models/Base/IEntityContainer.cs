using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Kinectitude.Editor.Models.Base
{
    public interface IEntityContainer
    {
        IEnumerable<Entity> Entities { get; }

        void AddEntity(Entity entity);
        void RemoveEntity(Entity entity);
    }
}
