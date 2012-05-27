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

        private List<T> children;

        protected List<T> Children {
            get { return new List<T>(children); } 
        }

        protected bool Running { get; private set; }

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

        protected Manager()
        {
            children = new List<T>();
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

        protected U GetService<U>() where U : Service
        {
            return Game.CurrentGame.GetService<U>();
        }
    }
}
