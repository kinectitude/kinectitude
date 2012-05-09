using System;

namespace Kinectitude.Core.Base
{
    public abstract class Component : IUpdateable
    {
        public Entity Entity { get; private set; }

        public Component(Entity entity)
        {
            Entity = entity;
        }

        public abstract Type ManagerType();

        public virtual void OnUpdate(double frameDelta) { }

        public virtual void Ready() { }
    }
}
