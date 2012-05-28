using System.Collections.Generic;

namespace MessagePassing.Public
{
    public interface IManager : IUpdateable
    {
        void Start();
        void Stop();
    }

    public class Manager<T> : IManager
    {
        private readonly List<T> children;

        protected IEnumerable<T> Children
        {
            get { return children; }
        }

        public Manager()
        {
            children = new List<T>();
        }

        public void Add(T component)
        {
            children.Add(component);
            OnAdd(component);
        }

        protected void OnAdd(T component) { }

        public void Update(double frameDelta) { }

        public void Start() { }

        public void Stop() { }
    }
}
