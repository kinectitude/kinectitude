using System.Collections.Generic;

namespace Kinectitude.Core.Base
{
    public interface IManager : IUpdateable
    {
        void Stop();
        void Start();
    }

    public class Manager<T> : IManager where T : IUpdateable
    {
        protected readonly List<T> children;
        protected readonly Game game;
        protected bool running;

        public void Add(T item)
        {
            children.Add(item);
            OnAdd(item);
        }

        public void Remove(T item)
        {
            children.Remove(item);
            OnRemove(item);
        }

        protected virtual void OnAdd(T item) { }
        protected virtual void OnRemove(T item) { }

        public Manager(Game game)
        {
            this.game = game;
            children = new List<T>();
        }

        public virtual void OnUpdate(double frameDelta) { }

        public void Stop()
        {
            if (running)
            {
                running = false;
                OnStop();
            }
        }
        public void Start()
        {
            if (!running)
            {
                running = true;
                OnStart();
            }
        }

        public virtual void OnStart() { }
        public virtual void OnStop() { }
    }
}
