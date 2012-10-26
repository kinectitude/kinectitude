using System;
using System.Collections.Generic;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    public abstract class DataContainer : IEntity
    {
        private readonly Dictionary<string, ValueReader> attributes =
            new Dictionary<string, ValueReader>();

        private readonly Dictionary<string, List<Action<ValueReader>>> callbacks =
            new Dictionary<string, List<Action<ValueReader>>>();

        internal readonly Dictionary<string, List<Action<ValueReader>>> CheckProperties =
            new Dictionary<string, List<Action<ValueReader>>>();
        protected readonly List<Tuple<DataContainer, string, Action<ValueReader>>> PropertyChanges =
            new List<Tuple<DataContainer, string, Action<ValueReader>>>();

        public bool Deleted { get; protected set; }

        private int id;
        public int Id {
            get { return id; }
            private set
            {
                id = value;
                attributes["Id"] = new ConstantReader(id);
            }
        }

        private string name;
        public string Name {
            get { return name; }
            set
            {
                name = value;
                attributes["Name"] = new ConstantReader(value);
            }
        }

        public ValueReader this[string key]
        {
            get
            {
                if (attributes.ContainsKey(key)) return attributes[key];
                return null;
            }

            set
            {
                if ("Name" == key || "Id" == key)
                {
                    throw new ArgumentException("Name And Id can't be changed");
                }

                ValueReader reader = new ConstantReader(value.GetPreferedValue());
                attributes[key] = reader;
                if (callbacks.ContainsKey(key))
                {
                    foreach (Action<ValueReader> action in callbacks[key]) action(reader);
                }
            }
        }

        internal DataContainer(int id) 
        {
            this.Id = id;
            Deleted = false;
        }

        internal void NotifyOfChange(string key, Action<ValueReader> callback)
        {
            List<Action<ValueReader>> addTo = null;
            callbacks.TryGetValue(key, out addTo);
            if (null == addTo)
            {
                addTo = new List<Action<ValueReader>>();
                callbacks[key] = addTo;
                addTo.Add(callback);
            }
            else
            {
                addTo.Add(callback);
            }
        }

        internal void StopNotifications(string key, Action<ValueReader> callback)
        {
            List<Action<ValueReader>> removeFrom = callbacks[key];
            removeFrom.Remove(callback);
        }


        internal void NotifyOfComponentChange(string what, Action<ValueReader> callback)
        {
            List<Action<ValueReader>> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                callbacks = new List<Action<ValueReader>>();
                callbacks.Add(callback);
                CheckProperties[what] = callbacks;
                string[] parts = what.Split('.');
                Changeable ch = GetComponentOrManager(parts[0]);
                if(null != ch) ch.ShouldCheck = true;
            }
        }

        internal void UnnotifyOfComponentChange(string what, Action<ValueReader> callback)
        {
            List<Action<ValueReader>> callbacks;
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

        internal void ChangedProperty(string what, object value)
        {
            List<Action<ValueReader>> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                foreach (Action<ValueReader> callback in callbacks) callback(new ConstantReader(value));
            }
        }

        internal abstract Changeable GetComponentOrManager(string name);
    }
}
