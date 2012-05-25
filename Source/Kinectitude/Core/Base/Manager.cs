using System.Collections.Generic;
using Kinectitude.Attributes;

namespace Kinectitude.Core.Base
{

    public interface IManager : IUpdateable
    {
        /// <summary>
        /// Used to stop the IManager.
        /// </summary>
        void Stop();

        /// <summary>
        /// Used to start the IManager.
        /// </summary>
        void Start();
    }

    public class Manager<T> : IManager where T : IUpdateable
    {
        protected readonly List<T> Children;

        protected bool Running { get; private set; }

        public void Add(T item)
        {
            Children.Add(item);
            OnAdd(item);
        }

        public void Remove(T item)
        {
            Children.Remove(item);
            OnRemove(item);
        }

        protected virtual void OnAdd(T item) { }
        protected virtual void OnRemove(T item) { }

        protected Manager()
        {
            Children = new List<T>();
        }

        public virtual void OnUpdate(float frameDelta) { }

        public void Start()
        {
            if (!Running)
            {
                Running = true;
                OnStart();
            }
        }

        protected virtual void OnStart() { }
        
        public void Stop()
        {
            if (Running)
            {
                Running = false;
                OnStop();
            }
        }
        
        protected virtual void OnStop() { }

        protected T GetService<T>() where T : Service
        {
            return Game.CurrentGame.GetService<T>();
        }
    }
}
