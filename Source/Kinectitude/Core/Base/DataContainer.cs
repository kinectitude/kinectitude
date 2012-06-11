using System;
using System.Collections.Generic;

namespace Kinectitude.Core.Base
{
    public class DataContainer : IEntity
    {
        private readonly Dictionary<string, string> attributes = new Dictionary<string,string>();

        private readonly Dictionary<string, List<Action<string>>> callbacks = 
            new Dictionary<string,List<Action<string>>>();

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
    }
}
