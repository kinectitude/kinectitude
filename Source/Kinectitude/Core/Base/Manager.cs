using System.Collections.Generic;

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

        /// <summary>
        /// Returns the components that have suscribed to the manager.
        /// </summary>
        protected List<T> Children {
            get { return new List<T>(children); } 
        }

        /// <summary>
        /// Tells a manager if it is currently in the running scene
        /// </summary>
        protected bool Running { get; private set; }

        /// <summary>
        /// Adds a component of type T to the manager
        /// </summary>
        /// <param name="item">The component to add</param>
        public void Add(T item)
        {
            children.Add(item);
            OnAdd(item);
        }

        /// <summary>
        /// This is called after the item is added.
        /// Custom logic can be overwritten here.
        /// </summary>
        /// <param name="item">The item that was added</param>
        protected virtual void OnAdd(T item) { }

        /// <summary>
        /// Removes a component of type T to the manager
        /// </summary>
        /// <param name="item">The component to remove</param>
        public void Remove(T item)
        {
            children.Remove(item);
            OnRemove(item);
        }

        /// <summary>
        /// This is called after the item is removed.
        /// Custom logic can be overwritten here.
        /// </summary>
        /// <param name="item">The item that was removed</param>
        protected virtual void OnRemove(T item) { }

        protected Manager()
        {
            children = new List<T>();
        }

        /// <summary>
        /// Called to notify the manager that the game has continued
        /// </summary>
        /// <param name="frameDelta">The amount of time in seconds that has passed since last update</param>
        public virtual void OnUpdate(float frameDelta) { }

        /// <summary>
        /// Used to start a manager
        /// </summary>
        public void Start()
        {
            if (!Running)
            {
                Running = true;
                OnStart();
            }
        }

        /// <summary>
        /// When a manager is started, this will be called after Running has already been set to true.
        /// Custom logic can go here
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Used to stop a manager
        /// </summary>
        public void Stop()
        {
            if (Running)
            {
                Running = false;
                OnStop();
            }
        }

        /// <summary>
        /// When a manager is stopped, this will be called after Running has already been set to false.
        /// Custom logic can go here
        /// </summary>
        protected virtual void OnStop() { }

        /// <summary>
        /// Returns the service of type U that is in the game
        /// </summary>
        /// <typeparam name="U">The type of the service to get</typeparam>
        /// <returns>The instance of the service that is registered with the game</returns>
        protected U GetService<U>() where U : Service
        {
            return Game.CurrentGame.GetService<U>();
        }
    }
}
