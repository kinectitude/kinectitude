using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal abstract class GameModel : BaseModel, IScope
    {
        private readonly List<GameModel> children;
        private readonly Dictionary<Type, List<Delegate>> allHandlers;
        private readonly Dictionary<Type, List<Tuple<Delegate, string>>> allDependencies;

        public IList<GameModel> Children
        {
            get { return children; }
        }

        public abstract IScope Parent { get; }

        protected GameModel()
        {
            children = new List<GameModel>();
            allHandlers = new Dictionary<Type, List<Delegate>>();
            allDependencies = new Dictionary<Type, List<Tuple<Delegate, string>>>();
        }

        public abstract void Accept(IGameVisitor visitor);

        public void AddHandler<T>(Action<T> handler) where T : Notification
        {
            List<Delegate> handlers;

            if (!allHandlers.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Delegate>();
                allHandlers[typeof(T)] = handlers;
            }

            handlers.Add(handler);
        }

        public void RemoveHandler<T>(Action<T> handler) where T : Notification
        {
            List<Delegate> handlers;

            if (allHandlers.TryGetValue(typeof(T), out handlers))
            {
                handlers.Remove(handler);
            }
        }

        public void AddDependency<T>(string property, Predicate<T> predicate = null) where T : Notification
        {
            List<Tuple<Delegate, string>> dependencies;

            if (!allDependencies.TryGetValue(typeof(T), out dependencies))
            {
                dependencies = new List<Tuple<Delegate, string>>();
                allDependencies[typeof(T)] = dependencies;
            }

            dependencies.Add(Tuple.Create<Delegate, string>(predicate, property));
        }

        public void Notify<T>(T notification) where T : Notification
        {
            HandleNotification(notification);

            if (notification.Mode == Notification.NotificationMode.Children)
            {
                foreach (GameModel child in Children)
                {
                    child.Notify(notification);
                }
            }
            else if (notification.Mode == Notification.NotificationMode.Parent && null != Parent)
            {
                Parent.Notify(notification);
            }
        }

        public void Broadcast<T>(T notification) where T : Notification
        {
            IScope root = this;

            while (root.Parent != null)
            {
                root = root.Parent;
            }

            root.Notify(notification);
        }

        private void HandleNotification<T>(T notification)
        {
            List<Delegate> handlers;

            if (allHandlers.TryGetValue(typeof(T), out handlers))
            {
                foreach (Delegate handler in handlers)
                {
                    var typedHandler = handler as Action<T>;

                    if (null != typedHandler)
                    {
                        typedHandler(notification);
                    }
                }
            }

            List<Tuple<Delegate, string>> dependencies;

            if (allDependencies.TryGetValue(typeof(T), out dependencies))
            {
                foreach (var pair in dependencies)
                {
                    var typedPredicate = pair.Item1 as Predicate<T>;

                    if (null == typedPredicate || typedPredicate(notification))
                    {
                        NotifyPropertyChanged(pair.Item2);
                    }
                }
            }
        }
    }

    internal abstract class GameModel<TScope> : GameModel where TScope : class, IScope
    {
        private TScope scope;

        public TScope Scope
        {
            get { return scope; }
            set
            {
                if (scope != value)
                {
                    if (null != scope)
                    {
                        OnScopeDetaching(scope);
                        scope.Children.Remove(this);
                    }

                    scope = value;

                    if (null != scope)
                    {
                        scope.Children.Add(this);
                        OnScopeAttaching(scope);
                    }

                    Notify(new ScopeChanged());
                }
            }
        }

        public override IScope Parent
        {
            get { return Scope; }
        }

        protected virtual void OnScopeDetaching(TScope scope) { }
        protected virtual void OnScopeAttaching(TScope scope) { }
    }
}
