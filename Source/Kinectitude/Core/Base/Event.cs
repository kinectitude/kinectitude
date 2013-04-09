//-----------------------------------------------------------------------
// <copyright file="Event.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Kinectitude.Core.Base
{
    public abstract class Event
    {

        private readonly List<Action> actions = new List<Action>();

        internal Entity Entity;

        protected Event() { }

        internal void Initialize()
        {
            Entity.addEvent(this);
            OnInitialize();
        }

        public abstract void OnInitialize();

        public void AddAction(Action action)
        {
            actions.Add(action);
        }

        public void DoActions()
        {
            //don't run events if the entity is destroyed
            if (Entity.Deleted) return;
            foreach (Action a in actions) a.Run();
        }

        /// <summary>
        /// Allows a Component to get another Component form the entity it belongs to
        /// </summary>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>The component in the entity, or null if none of type T exists</returns>
        public T GetComponent<T>() where T : class
        {
            return Entity.GetComponent<T>();
        }

        /// <summary>
        /// Allows a Component to get a manager of type T.
        /// Only one manager of Type T will be created in a scene.
        /// If none exists, one will be created and returned
        /// </summary>
        /// <typeparam name="T">The type of manager to get</typeparam>
        /// <returns>The scene's manager of type T</returns>
        public T GetManager<T>() where T : class, IManager
        {
            return Entity.Scene.GetManager<T>();
        }

        /// <summary>
        /// Called when the entity that an event belongs to is destroyed.
        /// This is only for cleanup, since DoActions will no longer do anything if the entity is deleted
        /// </summary>
        public virtual void Destroy() { }

    }
}
