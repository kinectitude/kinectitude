using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Data
{
    internal sealed class TypeMatcherWatcher : IChanges
    {
        private readonly object Obj;
        private readonly string Param;
        private readonly IDataContainer Owner;

        private readonly List<IChanges> Callbacks = new List<IChanges>();

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
            if(typeof(IChangeable).IsAssignableFrom(obj.GetType()))
                Owner.NotifyOfComponentChange(new Tuple<IChangeable,string>((IChangeable)obj, param), this);
        }

        internal TypeMatcher GetTypeMatcher() { return ClassFactory.GetParam(Obj, Param) as TypeMatcher; }

        internal void NotifyOfChange(IChanges callback) { Callbacks.Add(callback); }

        void IChanges.Change() { foreach (IChanges callback in Callbacks) callback.Change(); }
        void IChanges.Prepare() { foreach (IChanges callback in Callbacks) callback.Prepare(); }
    }
}
