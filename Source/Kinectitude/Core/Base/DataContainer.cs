using System;
using System.Collections.Generic;
using Kinectitude.Core.Data;
using SysAction = System.Action;

namespace Kinectitude.Core.Base
{
    public abstract class DataContainer
    {
        private readonly Dictionary<string, ValueReader> attributes = new Dictionary<string, ValueReader>();
        private readonly Dictionary<string, List<SysAction>> callbacks = new Dictionary<string, List<SysAction>>();

        internal readonly Dictionary<string, List<SysAction>> CheckProperties = new Dictionary<string, List<SysAction>>();
        protected readonly List<Tuple<DataContainer, string, SysAction>> PropertyChanges = new List<Tuple<DataContainer, string, SysAction>>();

        public bool Deleted { get; protected set; }

        public int Id { get; private set; }

        public string Name { get; set; }

        public ValueReader this[string key]
        {
            get
            {
                if (Deleted || !attributes.ContainsKey(key)) return ConstantReader.NullValue;
                return attributes[key];
            }

            set
            {
                if (Deleted) return;

                ValueReader reader;
                if (typeof(ConstantReader) == value.GetType()) reader = value;
                else reader = new ConstantReader(value.GetPreferedValue());

                attributes[key] = reader;
                if (callbacks.ContainsKey(key))
                {
                    foreach (SysAction Callable in callbacks[key]) Callable();
                }
            }
        }

        internal DataContainer(int id) 
        {
            this.Id = id;
            Deleted = false;
        }

        internal void NotifyOfChange(string key, SysAction callback)
        {
            List<SysAction> addTo = null;
            callbacks.TryGetValue(key, out addTo);
            if (null == addTo)
            {
                addTo = new List<SysAction>();
                callbacks[key] = addTo;
                addTo.Add(callback);
            }
            else
            {
                addTo.Add(callback);
            }
        }

        internal void StopNotifications(string key, SysAction callback)
        {
            List<SysAction> removeFrom = callbacks[key];
            removeFrom.Remove(callback);
        }


        internal void NotifyOfComponentChange(string what, SysAction callback)
        {
            List<SysAction> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                callbacks = new List<SysAction>();
                callbacks.Add(callback);
                CheckProperties[what] = callbacks;
                string[] parts = what.Split('.');
                Changeable ch = GetComponentOrManager(parts[0]);
                if(null != ch) ch.ShouldCheck = true;
            }
        }

        internal void UnnotifyOfComponentChange(string what, SysAction callback)
        {
            List<SysAction> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                callbacks.Remove(callback);
                if (callbacks.Count == 0)
                {
                    Changeable ch = GetComponentOrManager(what.Split('.')[0]);
                    if(null != ch) ch.ShouldCheck = false;
                }
            }
        }

        internal void ChangedProperty(string what)
        {
            List<SysAction> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                foreach (SysAction callback in callbacks) callback();
            }
        }

        internal abstract Changeable GetComponentOrManager(string name);
    }
}
