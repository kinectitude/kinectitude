using System.Collections.Generic;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    public abstract class Event
    {
        private readonly List<Action> actions;

        public Game Game { get; private set; }

        public Entity Entity { get; private set; }
        
        internal Dictionary<string, ReadableData> AvailableSelectors { get; private set; }
        
        internal Scene Scene { get; private set; }

        protected Event()
        {
            actions = new List<Action>();
        }

        internal void Initialize(Scene scene, Entity entity)
        {
            Scene = scene;
            Game = scene.Game;
            AvailableSelectors = new Dictionary<string, ReadableData>();
            Entity = entity;
            OnInitialize();
        }

        public abstract void OnInitialize();

        internal void AddAction(Action action)
        {
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
    }
}
