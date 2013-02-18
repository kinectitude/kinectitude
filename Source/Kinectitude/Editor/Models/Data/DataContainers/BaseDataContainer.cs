using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    internal abstract class BaseDataContainer : IDataContainer
    {
        private static void PrivatePublish(IEnumerable<IChanges> callbacks)
        {
            foreach (var callback in callbacks)
            {
                callback.Prepare();
            }

            foreach (var callback in callbacks)
            {
                callback.Change();
            }
        }

        private readonly Dictionary<string, List<IChanges>> allCallbacks;
        private readonly Dictionary<IChangeable, Dictionary<string, List<IChanges>>> allComponentCallbacks;
        private readonly Dictionary<string, ValueReader> attributes;
        private readonly Dictionary<string, IChangeable> changeables;

        protected BaseDataContainer()
        {
            allCallbacks = new Dictionary<string, List<IChanges>>();
            allComponentCallbacks = new Dictionary<IChangeable, Dictionary<string, List<IChanges>>>();
            attributes = new Dictionary<string, ValueReader>();
            changeables = new Dictionary<string, IChangeable>();
        }

        private List<IChanges> GetCallbacks(string key)
        {
            List<IChanges> callbacks;
            allCallbacks.TryGetValue(key, out callbacks);

            if (null == callbacks)
            {
                callbacks = new List<IChanges>();
                allCallbacks[key] = callbacks;
            }

            return callbacks;
        }

        private List<IChanges> GetPropertyCallbacks(IChangeable component, string property)
        {
            var componentCallbacks = GetComponentCallbacks(component);

            List<IChanges> callbacks;
            componentCallbacks.TryGetValue(property, out callbacks);

            if (null == callbacks)
            {
                callbacks = new List<IChanges>();
                componentCallbacks[property] = callbacks;
            }

            return callbacks;
        }

        private Dictionary<string, List<IChanges>> GetComponentCallbacks(IChangeable component)
        {
            Dictionary<string, List<IChanges>> callbacks;
            allComponentCallbacks.TryGetValue(component, out callbacks);

            if (null == callbacks)
            {
                callbacks = new Dictionary<string, List<IChanges>>();
                allComponentCallbacks[component] = callbacks;
            }

            return callbacks;
        }

        public void SubscribeToAttributeChange(string key, IChanges callback)
        {
            GetCallbacks(key).Add(callback);
        }

        public void SubscribeToComponentChange(IChangeable component, string property, IChanges callback)
        {
            GetPropertyCallbacks(component, property).Add(callback);
        }

        public void UnsubscribeFromAttributeChange(string key, IChanges callback)
        {
            GetCallbacks(key).Remove(callback);
        }

        public void UnsubscribeFromComponentChange(IChangeable component, string property, IChanges callback)
        {
            GetPropertyCallbacks(component, property).Remove(callback);
        }

        public void PublishAttributeChange(string key)
        {
            PrivatePublish(GetCallbacks(key));
        }

        public void PublishAllAttributeChanges()
        {
            foreach (var entry in allCallbacks)
            {
                PrivatePublish(entry.Value);
            }
        }

        public void PublishComponentChange(IChangeable component)
        {
            foreach (var callbacks in GetComponentCallbacks(component))
            {
                PrivatePublish(callbacks.Value);
            }
        }

        public void PublishComponentChange(IChangeable component, string property)
        {
            PrivatePublish(GetPropertyCallbacks(component, property));
        }

        public void PublishAllComponentChanges()
        {
            foreach (var key in allComponentCallbacks.Keys)
            {
                PublishComponentChange(key);
            }
        }

        public void PublishAll()
        {
            PublishAllAttributeChanges();
            PublishAllComponentChanges();
        }

        protected abstract ValueReader CreateAttributeReader(string key);
        protected abstract IChangeable CreateChangeable(string name);

        #region IDataContainer implementation

        ValueReader IDataContainer.this[string key]
        {
            get
            {
                ValueReader reader;
                attributes.TryGetValue(key, out reader);

                if (null == reader)
                {
                    reader = CreateAttributeReader(key);
                    attributes[key] = reader;
                }

                return reader;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        IChangeable IDataContainer.GetChangeable(string name)
        {
            IChangeable changeable;
            changeables.TryGetValue(name, out changeable);

            if (null == changeable)
            {
                changeable = CreateChangeable(name);
                changeables[name] = changeable;
            }

            return changeable;
        }

        void IDataContainer.NotifyOfChange(string key, IChanges callback)
        {
            SubscribeToAttributeChange(key, callback);
        }

        void IDataContainer.NotifyOfComponentChange(Tuple<IChangeable, string> what, IChanges callback)
        {
            var changeable = changeables.Values.FirstOrDefault(x => x == what.Item1);
            if (null != changeable)
            {
                SubscribeToComponentChange(changeable, what.Item2, callback);
            }
        }

        #endregion
    }
}
