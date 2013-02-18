using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;

namespace Kinectitude.Editor.Models
{
    internal abstract class GameModel : BaseModel, IScope
    {
        private readonly List<GameModel> children;
        private readonly Dictionary<Type, List<Delegate>> allHandlers;
        private readonly Dictionary<Type, List<Tuple<Delegate, string>>> allDependencies;

        public abstract IScope Parent { get; }

        public IList<GameModel> Children
        {
            get { return children; }
        }

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

        protected void Notify<T>(T e) where T : Notification
        {
            Notify(this, e);
        }

        protected void Broadcast<T>(T e) where T : Notification
        {
            Broadcast(this, e);
        }

        public void Notify<T>(GameModel source, T e) where T : Notification
        {
            HandleNotification(source, e);

            if (!e.Handled)
            {
                if (e.Mode == Notification.NotificationMode.Parent && null != Parent)
                {
                    Parent.Notify(source, e);
                }
                else if (e.Mode == Notification.NotificationMode.Children)
                {
                    foreach (GameModel child in Children)
                    {
                        child.Notify(source, e);
                    }
                }
            }
        }

        private T GetAncestor<T>(GameModel model) where T : class, IScope
        {
            IScope current = model;
            T ancestor = current as T;

            while (null == ancestor && current.Parent != null)
            {
                current = current.Parent;
                ancestor = current as T;
            }

            return ancestor;
        }

        protected T GetAncestor<T>() where T : class, IScope
        {
            return GetAncestor<T>(this);
        }

        public void Broadcast<T>(GameModel source, T e) where T : Notification
        {
            Game game = GetAncestor<Game>(source);

            if (null != game)
            {
                game.Notify(source, e);
            }
        }

        private void HandleNotification<T>(GameModel source, T e)
        {
            //if (this != source)
            //{
                List<Delegate> handlers;

                if (allHandlers.TryGetValue(typeof(T), out handlers))
                {
                    foreach (Delegate handler in handlers)
                    {
                        var typedHandler = handler as Action<T>;

                        if (null != typedHandler)
                        {
                            typedHandler(e);
                        }
                    }
                }

                List<Tuple<Delegate, string>> dependencies;

                if (allDependencies.TryGetValue(typeof(T), out dependencies))
                {
                    foreach (var pair in dependencies)
                    {
                        var typedPredicate = pair.Item1 as Predicate<T>;

                        if (null == typedPredicate || typedPredicate(e))
                        {
                            NotifyPropertyChanged(pair.Item2);
                        }
                    }
                }
            //}
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
