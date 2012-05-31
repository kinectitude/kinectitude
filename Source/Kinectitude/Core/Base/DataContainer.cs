using System;
using System.Collections.Generic;

namespace Kinectitude.Core.Base
{
    public class DataContainer : IDataContainer
    {
        private readonly Dictionary<string, string> attributes = new Dictionary<string,string>();

        private readonly Dictionary<string, List<Action<string>>> callbacks = 
            new Dictionary<string,List<Action<string>>>();
        
        public int Id { get; private set; }
        
        public string Name { get; set; }

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
                string old = null;
                attributes.TryGetValue(value, out old);
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
