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
        protected Game Game { get;  private set; }
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
            Game = game;
            children = new List<T>();
        }

        public virtual void OnUpdate(float frameDelta) { }

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
        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
    }
}
