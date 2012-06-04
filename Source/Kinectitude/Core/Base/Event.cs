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
            OnInitialize();
        }

        public abstract void OnInitialize();

        public void AddAction(Action action)
        {
            //TODO check assembly
            action.Event = this;
            actions.Add(action);
        }

        public void DoActions()
        {
            foreach (Action a in actions)
            {
                a.Run();
            }
        }

        public T GetComponent<T>() where T : Component
        {
            return Entity.GetComponent<T>();
        }
    }
}
