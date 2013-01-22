using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    internal sealed class CallbackCollection
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
        private readonly Dictionary<string, Dictionary<string, List<IChanges>>> allComponentCallbacks;

        public CallbackCollection()
        {
            allCallbacks = new Dictionary<string, List<IChanges>>();
            allComponentCallbacks = new Dictionary<string, Dictionary<string, List<IChanges>>>();
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

        private List<IChanges> GetPropertyCallbacks(string component, string property)
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

        private Dictionary<string, List<IChanges>> GetComponentCallbacks(string component)
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

        public void SubscribeToComponentChange(string component, string property, IChanges callback)
        {
            GetPropertyCallbacks(component, property).Add(callback);
        }

        public void Unsubscribe(string key, IChanges callback)
        {
            GetCallbacks(key).Remove(callback);
        }

        public void Unsubscribe(string component, string property, IChanges callback)
        {
            GetPropertyCallbacks(component, property).Remove(callback);
        }

        public void PublishAttributeChange(string key)
        {
            PrivatePublish(GetCallbacks(key));
        }

        public void PublishComponentChange(string component)
        {
            foreach (var callbacks in GetComponentCallbacks(component))
            {
                PrivatePublish(callbacks.Value);
            }
        }

        public void PublishComponentChange(string component, string property)
        {
            PrivatePublish(GetPropertyCallbacks(component, property));
        }

        public void PublishAll()
        {
            foreach (var entry in allCallbacks)
            {
                PrivatePublish(entry.Value);
            }

            foreach (var key in allComponentCallbacks.Keys)
            {
                PublishComponentChange(key);
            }
        }
    }
}
