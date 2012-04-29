using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Reflection;
using System.Windows;

namespace Kinectitude.Core
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

        internal void OnUpdate(double frameDelta)
        {
            //should not be updated if not running
            foreach (IManager m in managers)
            {
                m.OnUpdate(frameDelta);
            }
        }

        internal void CreateComponent(Entity entity, string stringType, List<Tuple<string, string>> values)
        {
            Type componentType = Game.GetType(stringType);
            Component created = entity.GetComponent(componentType);

            if (null == created)
            {

                Type[] constructors = { typeof(Entity) };
                object[] argVals = { entity };
                created = (Component)Game.CreateFromReflection(stringType, constructors, argVals);
                entity.AddComponent(created);
                Type manager = created.ManagerType();
                IManager parent = null;
                if (!managersDictionary.ContainsKey(manager))
                {
                    constructors = new Type[] { typeof(Game) };
                    argVals = new object[] { Game };
                    parent = (IManager)Game.CreateFromReflection(manager.Name, constructors, argVals);
                    managers.Add(parent);
                    managersDictionary.Add(manager, parent);
                }
                else
                {
                    parent = managersDictionary[manager];
                }

                foreach (Tuple<string, string> tuple in values)
                {
                    Game.SetParam(created, tuple.Item1, tuple.Item2);
                }
                MethodInfo mi = manager.GetMethod("Add");
                object[] argVal = { created };
                mi.Invoke(parent, argVal);
            }
            created.Ready();
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