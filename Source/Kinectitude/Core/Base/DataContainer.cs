using System;
using System.Collections.Generic;

namespace Kinectitude.Core.Base
{
    public abstract class DataContainer : IEntity
    {
        private readonly Dictionary<string, string> attributes = new Dictionary<string,string>();

        private readonly Dictionary<string, List<Action<string>>> callbacks = 
            new Dictionary<string,List<Action<string>>>();

        internal readonly Dictionary<string, List<Action<string>>> CheckProperties =
            new Dictionary<string, List<Action<string>>>();
        protected readonly List<Tuple<DataContainer, string, Action<string>>> PropertyChanges =
            new List<Tuple<DataContainer, string, Action<string>>>();
        protected readonly Dictionary<string, string> oldValues = new Dictionary<string, string>();

        public bool deleted { get; protected set; }

        private int id;
        public int Id {
            get { return id; }
            private set
            {
                id = value;
                attributes["Id"] = id.ToString();
            }
        }

        private string name;
        public string Name {
            get { return name; }
            set
            {
                name = value;
                attributes["Name"] = value;
            }
        }

        public string this[string key]
        {
            get
            {
                if (attributes.ContainsKey(key))
                {
                    return attributes[key];
                }
                return null;
            }

            set
            {
                if ("Name" == key || "Id" == key)
                {
                    throw new ArgumentException("Name and Id can't be changed");
                }
                attributes[key] = value;
                if (callbacks.ContainsKey(key))
                {
                    foreach (Action<string> action in callbacks[key])
                    {
                        action(value);
                    }
                }
            }
        }

        internal DataContainer(int id) 
        {
            this.Id = id;
            deleted = false;
        }

        internal void NotifyOfChange(string key, Action<string> callback)
        {
            List<Action<string>> addTo = null;
            callbacks.TryGetValue(key, out addTo);
            if (null == addTo)
            {
                addTo = new List<Action<string>>();
                callbacks[key] = addTo;
                addTo.Add(callback);
            }
            else
            {
                addTo.Add(callback);
            }
        }

        internal void StopNotifications(string key, Action<string> callback)
        {
            List<Action<string>> removeFrom = callbacks[key];
            removeFrom.Remove(callback);
        }


        internal void NotifyOfComponentChange(string what, Action<string> callback)
        {
            List<Action<string>> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                callbacks = new List<Action<string>>();
                callbacks.Add(callback);
                CheckProperties[what] = callbacks;
                string [] parts = what.Split('.');
                object obj = GetComponentOrManager(parts[0]);
                oldValues[what] = ClassFactory.GetStringParam(obj, parts[1]);
            }
        }

        internal void UnnotifyOfComponentChange(string what, Action<string> callback)
        {
            List<Action<string>> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                callbacks.Remove(callback);
                if (callbacks.Count == 0) CheckProperties.Remove(what);
            }
        }

        internal void CheckForChanges()
        {
            foreach (KeyValuePair<string, List<Action<string>>> keyValue in CheckProperties)
            {
                string [] parts = keyValue.Key.Split('.');
                object obj = GetComponentOrManager(parts[0]);
                if (null == obj) continue;
                string newVal = ClassFactory.GetStringParam(obj, parts[1]);
                if (oldValues[keyValue.Key] != newVal)
                {
                    foreach (Action<string> action in keyValue.Value) action(newVal);
                    oldValues[keyValue.Key] = newVal;
                }
            }
        }

        internal abstract object GetComponentOrManager(string name);

    }
}
