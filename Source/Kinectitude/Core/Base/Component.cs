using System;

namespace Kinectitude.Core.Base
{

    public abstract class Component : IUpdateable
    {
        internal Entity Entity;
        public IDataContainer IEntity
        {
            get { return Entity; }
        }

        public Component() { }

        public abstract Type ManagerType();

        public virtual void OnUpdate(float frameDelta) { }

        public virtual void Ready() { }

        public virtual Type ImplementationType
        {
            get { return this.GetType(); }
        }

        public T GetComponent<T>() where T : Component
        {
            return Entity.GetComponent<T>();
        }

    }
}
