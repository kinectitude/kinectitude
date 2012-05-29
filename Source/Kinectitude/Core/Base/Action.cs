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
        public T GetComponent<T>() where T : Component
        {
            return Event.Entity.GetComponent<T>();
        }
    }
}
