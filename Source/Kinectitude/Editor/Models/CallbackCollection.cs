using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal sealed class CallbackCollection
    {
        private static void PrivatePublish(IEnumerable<IChangeable> callbacks)
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

        private readonly Dictionary<string, List<IChangeable>> allCallbacks;
        private readonly Dictionary<string, Dictionary<string, List<IChangeable>>> allComponentCallbacks;

        public CallbackCollection()
        {
            allCallbacks = new Dictionary<string, List<IChangeable>>();
            allComponentCallbacks = new Dictionary<string, Dictionary<string, List<IChangeable>>>();
        }

        private List<IChangeable> GetCallbacks(string key)
        {
            List<IChangeable> callbacks;
            allCallbacks.TryGetValue(key, out callbacks);

            if (null == callbacks)
            {
                callbacks = new List<IChangeable>();
                allCallbacks[key] = callbacks;
            }

            return callbacks;
        }

        private List<IChangeable> GetPropertyCallbacks(string component, string property)
        {
            var componentCallbacks = GetComponentCallbacks(component);

            List<IChangeable> callbacks;
            componentCallbacks.TryGetValue(property, out callbacks);

            if (null == callbacks)
            {
                callbacks = new List<IChangeable>();
                componentCallbacks[property] = callbacks;
            }

            return callbacks;
        }

        private Dictionary<string, List<IChangeable>> GetComponentCallbacks(string component)
        {
            Dictionary<string, List<IChangeable>> callbacks;
            allComponentCallbacks.TryGetValue(component, out callbacks);

            if (null == callbacks)
            {
                callbacks = new Dictionary<string, List<IChangeable>>();
                allComponentCallbacks[component] = callbacks;
            }

            return callbacks;
        }

        public void SubscribeToAttributeChange(string key, IChangeable callback)
        {
            GetCallbacks(key).Add(callback);
        }

        public void SubscribeToComponentChange(string component, string property, IChangeable callback)
        {
            GetPropertyCallbacks(component, property).Add(callback);
        }

        public void Unsubscribe(string key, IChangeable callback)
        {
            GetCallbacks(key).Remove(callback);
        }

        public void Unsubscribe(string component, string property, IChangeable callback)
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
    }
}
