namespace Kinectitude.Core.Base
{
    public abstract class Action
    {
        internal Event Event { get; set; }

        /// <summary>
        /// This method is called when an Event or Trigger causes an Action to happen
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// This method allows Actions to get components from the entity they belong to.
        /// </summary>
        /// <typeparam name="T">The type of the component.
        /// NOTE This must be the same as ImplementationType in the component</typeparam>
        /// <returns>The component of type T or null if none exists in the entity</returns>
        public T GetComponent<T>() where T : class
        {
            return Event.Entity.GetComponent<T>();
        }

        /// <summary>
        /// This method allows Actions to get a manager from the event
        /// </summary>
        /// <typeparam name="T">The type of the manager.
        /// NOTE This must be the same as ImplementationType in the manager</typeparam>
        /// <returns>The manager of type T. A new one is created if not found</returns>
        public T GetManager<T>() where T : class, IManager
        {
            return Event.GetManager<T>();
        }
    }
}
