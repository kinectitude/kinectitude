//-----------------------------------------------------------------------
// <copyright file="Component.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------


namespace Kinectitude.Core.Base
{

    public abstract class Component : Changeable,  IUpdateable
    {
        //private Entity entity;
        internal Entity Entity { get; set; }

        /// <summary>
        /// The IEntity that this component belongs to
        /// </summary>
        public IEntity IEntity
        {
            get { return Entity; }
        }

        public Component() { }

        /// <summary>
        /// Called to update the component.  Managers are expected to call this to update components when they get updated
        /// </summary>
        /// <param name="frameDelta"></param>
        public virtual void OnUpdate(float frameDelta) { }

        /// <summary>
        /// Notifies the component that all setters have been set
        /// </summary>
        public virtual void Ready() { }

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
        /// Notifies the component that the entity it is a part of was destroyed.
        /// Components are expected to clean up themselves and remove themselves from any managers
        /// </summary>
        public abstract void Destroy();
    }
}
