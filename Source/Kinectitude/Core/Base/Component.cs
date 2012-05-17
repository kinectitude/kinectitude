using System;

namespace Kinectitude.Core.Base
{

    public abstract class Component : IUpdateable
    {
        public Entity Entity { get; internal set; }

        public Component() { }

        public abstract Type ManagerType();

        public virtual void OnUpdate(float frameDelta) { }

        public virtual void Ready() { }

        public virtual Type ImplementationType()
        {
            return this.GetType();
        }
    }
}
