using System.Collections.Generic;
using Kinectitude.Core.Data;
using Kinectitude.Attributes;

namespace Kinectitude.Core.Base
{
    public abstract class Event
    {

        private static readonly Queue<Event> eventQueue = new Queue<Event>();

        internal readonly Dictionary<string, TypeMatcher> AvailableSelectors =
            new Dictionary<string, TypeMatcher>();

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
            if (0 == eventQueue.Count)
            {
                Run();
            }
            else
            {
                eventQueue.Enqueue(this);
            }
        }

        private void Run()
        {
            foreach (Action a in actions)
            {
                a.Run();
            }
            if (eventQueue.Count != 0)
            {
                eventQueue.Dequeue().Run();
            }
        }

        public T GetComponent<T>() where T : Component
        {
            return Entity.GetComponent<T>();
        }
    }
}
