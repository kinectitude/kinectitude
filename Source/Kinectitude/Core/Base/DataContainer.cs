using System;
using System.Collections.Generic;
using Kinectitude.Core.Data;

namespace Kinectitude.Core.Base
{
    public abstract class DataContainer : IDataContainer
    {
        private readonly Dictionary<string, ValueReader> attributes = new Dictionary<string, ValueReader>();
        private readonly Dictionary<string, List<IChanges>> callbacks = new Dictionary<string, List<IChanges>>();

        internal readonly Dictionary<Tuple<IChangeable, string>, List<IChanges>> CheckProperties =
            new Dictionary<Tuple<IChangeable, string>, List<IChanges>>();

        internal readonly List<Tuple<DataContainer, Tuple<IChangeable, string>, IChanges>> PropertyChanges =
            new List<Tuple<DataContainer, Tuple<IChangeable, string>, IChanges>>();

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
                    foreach (IChanges callable in callbacks[key]) callable.Prepare();
                    foreach (IChanges callable in callbacks[key]) callable.Change();
                }
            }
        }

        internal DataContainer(int id) 
        {
            this.Id = id;
            Deleted = false;
        }

        public void NotifyOfChange(string key, IChanges callback)
        {
            List<IChanges> addTo = null;
            callbacks.TryGetValue(key, out addTo);
            if (null == addTo)
            {
                addTo = new List<IChanges>();
                callbacks[key] = addTo;
                addTo.Add(callback);
            }
            else
            {
                addTo.Add(callback);
            }
        }

        internal void StopNotifications(string key, IChanges callback)
        {
            List<IChanges> removeFrom = callbacks[key];
            removeFrom.Remove(callback);
        }

        void IDataContainer.NotifyOfComponentChange(Tuple<IChangeable, string> what, IChanges callback)
        {
            List<IChanges> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                callbacks = new List<IChanges>();
                callbacks.Add(callback);
                CheckProperties[what] = callbacks;

                if(null != what.Item1) what.Item1.ShouldCheck = true;
            }
        }

        internal void UnnotifyOfComponentChange(Tuple<IChangeable, string> what, IChanges callback)
        {
            List<IChanges> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                callbacks.Remove(callback);
                if (callbacks.Count == 0 && null != what.Item1) what.Item1.ShouldCheck = false;
            }
        }

        internal void ChangedProperty(Tuple<IChangeable, string> what)
        {
            List<IChanges> callbacks;
            if (CheckProperties.TryGetValue(what, out callbacks))
            {
                foreach (IChanges callback in callbacks) callback.Prepare();
                foreach (IChanges callback in callbacks) callback.Change();
            }
        }

        internal abstract Changeable GetChangeable(string name);

        IChangeable IDataContainer.GetChangeable(string name)
        {
            return GetChangeable(name);
        }
    }
}
