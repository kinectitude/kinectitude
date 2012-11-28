using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Data
{
    internal sealed class TypeMatcherWatcher : IChangeable
    {
        private readonly object Obj;
        private readonly string Param;
        private readonly DataContainer Owner;

        private readonly List<IChangeable> Callbacks = new List<IChangeable>();

        internal static TypeMatcherWatcher GetTypeMatcherWatcher(object obj, string param, Entity owner)
        {
            Func<TypeMatcherWatcher> create = new Func<TypeMatcherWatcher>(() => new TypeMatcherWatcher(obj, param, owner));
            return DoubleDictionary<object, string, TypeMatcherWatcher>.GetItem(obj, param, create);

        }

        private TypeMatcherWatcher(object obj, string param, Entity owner)
        {
            Obj = obj;
            Param = param;
            Type objType = obj.GetType();
            Owner = owner;
            if(typeof(Changeable).IsAssignableFrom(obj.GetType()))
                Owner.NotifyOfComponentChange(ClassFactory.GetReferedName(Obj.GetType()) + '.' + Param, this);
        }

        internal TypeMatcher GetTypeMatcher() { return ClassFactory.GetParam(Obj, Param) as TypeMatcher; }

        internal void NotifyOfChange(IChangeable callback) { Callbacks.Add(callback); }

        void IChangeable.Change() { foreach (IChangeable callback in Callbacks) callback.Change(); }
        void IChangeable.Prepare() { foreach (IChangeable callback in Callbacks) callback.Prepare(); }
    }
}
