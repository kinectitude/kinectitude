using System;
using System.Collections.Generic;
using System.Reflection;
using Kinectitude.Core.Events;
using Kinectitude.Core.Loaders;

namespace Kinectitude.Core.Base
{    
    public class Scene : DataContainer
    {
        private readonly Dictionary<Type, IManager> managersDictionary;
        private readonly List<IManager> managers;
        private readonly Dictionary<int, Entity> entityById;
        private readonly Dictionary<string, List<TriggerOccursEvent>> triggers;
        private readonly SceneLoader seceneLoader;
        private bool started = false;

        public List<SceneStartsEvent> OnStart
        {
            get;
            private set;
        }
        
        public Game Game
        {
            get;
            private set;
        }
        
        internal Dictionary<string, Entity> EntityByName
        {
            get;
            private set;
        }
        
        internal Dictionary<string, HashSet<int>> IsType
        {
            get;
            set;
        }
        
        internal Dictionary<string, HashSet<int>> IsExactType
        {
            get;
            set;
        }

        private bool running = false;
        public bool Running
        {
            get { return running; }
            set
            {
                if (true == value)
                {
                    if (!started)
                    {
                        started = true;
                        foreach (SceneStartsEvent e in OnStart)
                        {
                            e.DoActions();
                        }
                    }
                    foreach (IManager m in managers)
                    {
                        m.Start();
                    }
                }
                else
                {
                    foreach (IManager m in managers)
                    {
                        m.Stop();
                    }
                }
                running = value;
            }
        }

        internal Scene(SceneLoader sceneLoader, Game game) : base(-2)
        {
            managersDictionary = new Dictionary<Type, IManager>();
            managers = new List<IManager>();
            Game = game;
            OnStart = new List<SceneStartsEvent>();
            IsType = sceneLoader.IsType;
            IsExactType = sceneLoader.IsExactType;
            entityById = sceneLoader.EntityById;
            EntityByName = sceneLoader.EntityByName;
            triggers = new Dictionary<string,List<TriggerOccursEvent>>();
            this.seceneLoader = sceneLoader;
        }

        public T GetManager<T>() where T : class
        {
            return managersDictionary[typeof(T)] as T;  // TODO: This might be a good place to move the manager construction code
        }

        internal void OnUpdate(float frameDelta)
        {
            //should not be updated if not running
            foreach (IManager m in managers)
            {
                m.OnUpdate(frameDelta);
            }
        }

        internal void CreateComponent(Entity entity, string stringType, List<Tuple<string, string>> values)
        {
            Component created = ClassFactory.Create<Component>(stringType);
            created.Entity = entity;
            entity.AddComponent(created);
            Type manager = created.ManagerType();
            IManager parent = null;
            if (!managersDictionary.ContainsKey(manager))
            {
                parent = ClassFactory.Create<IManager>(manager, Game);
                managers.Add(parent);
                managersDictionary.Add(manager, parent);
            }
            else
            {
                parent = managersDictionary[manager];
            }
                
            foreach (Tuple<string, string> tuple in values)
            {
                ClassFactory.SetParam(created, tuple.Item1, tuple.Item2, this, null, entity);
            }
            //TODO make this better?
            MethodInfo mi = manager.GetMethod("Add");
            object[] argVal = { created };
            mi.Invoke(parent, argVal);
        }

        internal void CreateAction(Event evt, string type, List<Tuple<string, string>> attribs, Condition cond = null)
        {
            Action action = ClassFactory.Create<Action>(type);
            action.Event = evt;
            foreach (Tuple<string, string> attrib in attribs)
            {
                ClassFactory.SetParam(action, attrib.Item1, attrib.Item2, this, evt, evt.Entity);
            }
            if (null == cond)
            {
                evt.AddAction(action);
            }
            else
            {
                cond.AddAction(action);
            }
        }

        internal void FireTrigger(string triggerName)
        {
            if (!triggers.ContainsKey(triggerName))
            {
                return;
            }
            List<TriggerOccursEvent> triggerList = triggers[triggerName];
            foreach (TriggerOccursEvent trigger in triggerList)
            {
                trigger.DoActions();
            }
        }

        internal void RegisterTrigger(string name, TriggerOccursEvent evt)
        {
            if (triggers.ContainsKey(name))
            {
                triggers[name].Add(evt);
                return;
            }
            List<TriggerOccursEvent> tlist = new List<TriggerOccursEvent>();
            tlist.Add(evt);
            triggers.Add(name, tlist);
        }

    }
}