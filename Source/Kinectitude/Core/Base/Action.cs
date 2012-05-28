namespace Kinectitude.Core.Base
{
    public abstract class Action
    {
        internal Event Event { get; set; }
        public abstract void Run();
        public T GetComponent<T>() where T : Component
        {
            return Event.Entity.GetComponent<T>();
        }
    }
}
