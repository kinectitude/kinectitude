using System.Collections.Generic;
using MessagePassing.Public;

namespace MessagePassing.Core
{
    /// <summary>
    /// Base class for an entity, scene, or game. They can all store local variable (formerly attributes)
    /// and publish/subscribe to events. Since pub/sub means different things in different scopes, those
    /// methods are abstract and are implemented differently in each of the 3 subclasses.
    /// </summary>
    internal abstract class Actor : IUpdateable
    {
        private readonly Dictionary<string, object> localVariables;

        public object this[string key]
        {
            get
            {
                object ret;
                localVariables.TryGetValue(key, out ret);
                return ret;
            }
            set
            {
                localVariables[key] = value;
            }
        }

        protected Actor()
        {
            localVariables = new Dictionary<string, object>();
        }

        public void Update(double frameDelta) { }

        public abstract void Publish(string id, params object[] data);

        public abstract void Subscribe(string id, MessageCallback callback, params object[] parameters);
    }
}
