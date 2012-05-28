using System;
using System.Collections.Generic;
using MessagePassing.Public;

namespace MessagePassing.Core
{
    internal sealed class Scene : Actor
    {
        private readonly List<Entity> entities;
        private readonly Dictionary<string, MessageCallback> callbacks;
        private readonly Dictionary<Type, IManager> managers;

        public Scene()
        {
            entities = new List<Entity>();
            callbacks = new Dictionary<string, MessageCallback>();
            managers = new Dictionary<Type, IManager>();
        }

        public void AddEntity(Entity entity)
        {
            entity.Scene = this;
            entities.Add(entity);
        }

        public override void Publish(string id, object[] data)
        {
            MessageCallback registeredCallback;
            callbacks.TryGetValue(id, out registeredCallback);

            if (null != registeredCallback)
            {
                registeredCallback(data);
            }
        }

        public override void Subscribe(string id, MessageCallback callback, object[] parameters)
        {
            MessageCallback registeredCallback;
            callbacks.TryGetValue(id, out registeredCallback);

            if (null == registeredCallback)
            {
                registeredCallback = callback;
            }
            else
            {
                registeredCallback += callback;
            }

            callbacks[id] = registeredCallback;
        }

        public T GetManager<T>() where T : class, IManager, new()
        {
            IManager ret;
            managers.TryGetValue(typeof(T), out ret);

            if (null == ret)
            {
                ret = new T();
                managers[typeof(T)] = ret;
            }

            return ret as T;
        }
    }
}
