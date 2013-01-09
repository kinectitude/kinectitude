using System;
using System.Collections.Generic;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    public abstract class DataContainer : IDataContainer
    {
        private readonly Dictionary<string, ValueReader> attributes = new Dictionary<string, ValueReader>();
        private readonly Dictionary<string, List<IChangeable>> callbacks = new Dictionary<string, List<IChangeable>>();

        internal readonly Dictionary<string, List<IChangeable>> CheckProperties = new Dictionary<string, List<IChangeable>>();
        internal readonly List<Tuple<DataContainer, string, IChangeable>> PropertyChanges = new List<Tuple<DataContainer, string, IChangeable>>();

        internal bool Deleted { get; set; }

        internal int Id { get; private set; }

        internal string Name { get; set; }

        public ValueReader this[string key]
        {
            get
            {
                if (Deleted) Game.CurrentGame.Die("Can't read from a deleted entity");
                if (!attributes.ContainsKey(key)) return ConstantReader.NullValue;
                return attributes[key];
            }

            set
            {
                if (Deleted) Game.CurrentGame.Die("Can't write to a deleted entity");

                ValueReader reader = value as ConstantReader ?? new ConstantReader(value.GetPreferedValue());

                attributes[key] = reader;
                if (callbacks.ContainsKey(key))
                {
                    foreach (IChangeable callable in callbacks[key]) callable.Prepare();
                    foreach (IChangeable callable in callbacks[key]) callable.Change();
                }
            }
        }

        internal DataContainer(int id) 
        {
            this.Id = id;
            Deleted = false;
        }

        public void NotifyOfChange(string key, IChangeable callback)
        {
            List<IChangeable> addTo = null;
            callbacks.TryGetValue(key, out addTo);
            if (null == addTo)
            {
                addTo = new List<IChangeable>();
                callbacks[key] = addTo;
                addTo.Add(callback);
            }
            else
            {
                addTo.Add(callback);
            }
        }

        internal void StopNotifications(string key, IChangeable callback)
        {
            List<IChangeable> removeFrom = callbacks[key];
            removeFrom.Remove(callback);
        }


        public void NotifyOfComponentChange(string what, IChangeable callback)
        {
            List<IChangeable> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                callbacks = new List<IChangeable>();
                callbacks.Add(callback);
                CheckProperties[what] = callbacks;
                string[] parts = what.Split('.');
                Changeable ch = GetChangeable(parts[0]);
                if(null != ch) ch.ShouldCheck = true;
            }
        }

        internal void UnnotifyOfComponentChange(string what, IChangeable callback)
        {
            List<IChangeable> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                callbacks.Remove(callback);
                if (callbacks.Count == 0)
                {
                    Changeable ch = GetChangeable(what.Split('.')[0]);
                    if(null != ch) ch.ShouldCheck = false;
                }
            }
        }

        internal void ChangedProperty(string what)
        {
            List<IChangeable> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                foreach (IChangeable callback in callbacks) callback.Prepare();
                foreach (IChangeable callback in callbacks) callback.Change();
            }
        }

        internal abstract Changeable GetChangeable(string name);
    }
}
