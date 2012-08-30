using System;
using System.Collections.Generic;
using Kinectitude.Core.Events;
using Kinectitude.Core.Loaders;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{    
    internal class Scene : DataContainer
    {
        private readonly Dictionary<string, List<TriggerOccursEvent>> triggers;
        private readonly SceneLoader seceneLoader;
        private readonly Dictionary<string, List<Timer>> runningTimers = new Dictionary<string, List<Timer>>();
        private readonly Dictionary<string, List<Timer>> pausedTimers = new Dictionary<string, List<Timer>>();

        internal readonly Dictionary<Type, IManager> ManagersDictionary;
        internal readonly List<IManager> Managers;

        private bool started = false;
        private bool running = false;

        internal List<SceneStartsEvent> OnStart { get; private set; }
        internal Game Game { get; private set; }
        internal Dictionary<string, HashSet<int>> IsType { get; private set; }
        internal Dictionary<string, HashSet<int>> IsExactType { get; private set; }
        internal Dictionary<int, Entity> EntityById { get; private set; }
        internal Dictionary<string, Entity> EntityByName { get; private set; }

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
                    foreach (IManager m in Managers)
                    {
                        m.Start();
                    }
                }
                else
                {
                    foreach (IManager m in Managers)
                    {
                        m.Stop();
                    }
                }
                running = value;
            }
        }

        internal Scene(SceneLoader sceneLoader, Game game) : base(-1)
        {
            ManagersDictionary = new Dictionary<Type, IManager>();
            Managers = new List<IManager>();
            Game = game;
            OnStart = new List<SceneStartsEvent>();
            IsType = new Dictionary<string, HashSet<int>>();
            IsExactType = new Dictionary<string, HashSet<int>>();
            EntityById = new Dictionary<int, Entity>();
            EntityByName = new Dictionary<string, Entity>();
            triggers = new Dictionary<string,List<TriggerOccursEvent>>();
            this.seceneLoader = sceneLoader;
        }

        internal void OnUpdate(float frameDelta)
        {
            //should not be updated if not running
            foreach (IManager m in Managers)
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

        internal Entity GetEntity(string name)
        {
            return EntityByName[name];
        }

        internal void CreateEntity(string prototype)
        {
            seceneLoader.CreateEntity(prototype, this);
        }

        internal IManager GetManager(string registeredName)
        {
            Type managerType = ClassFactory.TypesDict[registeredName];
            IManager manager;
            if (ManagersDictionary.ContainsKey(managerType))
            {
                manager = ManagersDictionary[managerType];
            }
            else
            {
                manager = ClassFactory.Create<IManager>(registeredName);
                ManagersDictionary.Add(managerType, manager);
                Managers.Add(manager);
            }
            return manager;
        }

        internal T GetManager<T>() where T : class, IManager
        {
            if (!ManagersDictionary.ContainsKey(typeof(T)))
            {
                T manager = ClassFactory.Create<T>(typeof(T));
                Managers.Add(manager);
                ManagersDictionary.Add(typeof(T), manager);
                return manager;
            }
            else
            {
                return ManagersDictionary[typeof(T)] as T;
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
                EntityByName.Remove(delete.Name);
            }
            EntityById.Remove(delete.Id);
            //I don't think I need to remove the entity from each type matcher, since it won't be refered to by anything.
        }


        internal override Changeable GetComponentOrManager(string name)
        {
            Type managerType = ClassFactory.TypesDict[name];
            IManager manager = null;
            ManagersDictionary.TryGetValue(managerType, out manager);
            return manager as Changeable;
        }
    }
}