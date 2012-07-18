using System;
using System.Collections.Generic;
using Kinectitude.Core.Events;
using Kinectitude.Core.Loaders;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{    
    internal class Scene : DataContainer
    {
        private readonly Dictionary<Type, IManager> managersDictionary;
        private readonly List<IManager> managers;
        private readonly Dictionary<int, Entity> entityById;
        private readonly Dictionary<string, List<TriggerOccursEvent>> triggers;
        private readonly SceneLoader seceneLoader;
        private readonly Dictionary<string, Entity> entityByName;

        private readonly Dictionary<string, List<Timer>> runningTimers = new Dictionary<string, List<Timer>>();
        private readonly Dictionary<string, List<Timer>> pausedTimers = new Dictionary<string, List<Timer>>();

        private bool started = false;

        internal List<SceneStartsEvent> OnStart
        {
            get;
            private set;
        }
        
        internal Game Game
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
        internal bool Running
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

        internal Scene(SceneLoader sceneLoader, Game game) : base(-1)
        {
            managersDictionary = new Dictionary<Type, IManager>();
            managers = new List<IManager>();
            Game = game;
            OnStart = new List<SceneStartsEvent>();
            IsType = sceneLoader.IsType;
            IsExactType = sceneLoader.IsExactType;
            entityById = sceneLoader.EntityById;
            entityByName = sceneLoader.EntityByName;
            triggers = new Dictionary<string,List<TriggerOccursEvent>>();
            this.seceneLoader = sceneLoader;
        }

        internal void OnUpdate(float frameDelta)
        {
            //should not be updated if not running
            foreach (IManager m in managers)
            {
                m.OnUpdate(frameDelta);

            }
            
            foreach (KeyValuePair<string, List<Timer>> pair in runningTimers)
            {
                List<Timer> timers = pair.Value;
                List<Timer> remove = new List<Timer>();
                foreach (Timer timer in timers)
                {
                    if (timer.tick(frameDelta))
                    {
                        FireTrigger(timer.ExpressionReader.GetValue());
                        if (!timer.Recurring)
                        {
                            remove.Add(timer);
                        }
                    }
                }
                foreach (Timer timer in remove)
                {
                    timers.Remove(timer);
                }
            }
        }

        internal void CreateComponent(Entity entity, string stringType, List<Tuple<string, string>> values)
        {
            Component created = ClassFactory.Create<Component>(stringType);
            created.Entity = entity;
            entity.AddComponent(created, stringType);
            foreach (Tuple<string, string> tuple in values)
            {
                ClassFactory.SetParam(created, tuple.Item1, tuple.Item2, null, entity);
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

        internal Entity EntityByName(string name)
        {
            return entityByName[name];
        }

        internal void CreateEntity(string prototype)
        {
            seceneLoader.CreateEntity(prototype);
        }

        internal void CreateManager(string registeredName, List<Tuple<string, string>> values)
        {
            IManager manager = GetManager(registeredName);
            foreach (Tuple<string, string> value in values)
            {
                ClassFactory.SetParam(manager, value.Item1, value.Item2, null, null);
            }
        }

        internal IManager GetManager(string registeredName)
        {
            Type managerType = ClassFactory.TypesDict[registeredName];
            IManager manager;
            if (managersDictionary.ContainsKey(managerType))
            {
                manager = managersDictionary[managerType];
            }
            else
            {
                manager = ClassFactory.Create<IManager>(registeredName);
                managersDictionary.Add(managerType, manager);
                managers.Add(manager);
            }
            return manager;
        }

        internal T GetManager<T>() where T : class, IManager
        {
            if (!managersDictionary.ContainsKey(typeof(T)))
            {
                T manager = ClassFactory.Create<T>(typeof(T));
                managers.Add(manager);
                managersDictionary.Add(typeof(T), manager);
                return manager;
            }
            else
            {
                return managersDictionary[typeof(T)] as T;
            }
        }
        internal void AddTimer(string name, float time, IExpressionReader expressionReader, bool recurring)
        {
            Timer t = new Timer(expressionReader, time, recurring);
            List<Timer> timerList;
            if (!runningTimers.TryGetValue(name, out timerList))
            {
                if (!pausedTimers.TryGetValue(name, out timerList))
                {
                    timerList = new List<Timer>();
                }
                else
                {
                    pausedTimers.Remove(name);
                }
            }
            timerList.Add(t);
            runningTimers.Add(name, timerList);
        }

        internal void ResumeTimers(string name)
        {
            List<Timer> timerList;
            if (pausedTimers.TryGetValue(name, out timerList))
            {
                pausedTimers.Remove(name);
                runningTimers.Add(name, timerList);
            }
        }

        internal void PauseTimers(string name)
        {
            List<Timer> timerList;
            if (runningTimers.TryGetValue(name, out timerList))
            {
                runningTimers.Remove(name);
                pausedTimers.Add(name, timerList);
            }
        }

        internal void DeleteEntity(Entity delete)
        {
            if (delete.Name != null)
            {
                entityByName.Remove(delete.Name);
            }
            entityById.Remove(delete.Id);
            //I don't think I need to remove the entity from each type matcher, since it won't be refered to by anything.
        }


        internal override Changeable GetComponentOrManager(string name)
        {
            Type managerType = ClassFactory.TypesDict[name];
            IManager manager = null;
            managersDictionary.TryGetValue(managerType, out manager);
            return manager as Changeable;
        }
    }
}