using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysAction = System.Action;
using Action = Kinectitude.Core.Base.Action;

namespace Kinectitude.Core.Data
{
    internal class TypeMatcherWatcher
    {
        private readonly object Obj;
        private readonly string Param;
        private readonly DataContainer Owner;

        private readonly List<SysAction> Callbacks = new List<SysAction>();

        internal static TypeMatcherWatcher getTypeMatcherWatcher(object obj, string param, Entity owner)
        {
            Func<TypeMatcherWatcher> create = new Func<TypeMatcherWatcher>(() => new TypeMatcherWatcher(obj, param, owner));
            return DoubleDictionary<object, string, TypeMatcherWatcher>.getItem(obj, param, create);

        }

        private TypeMatcherWatcher(object obj, string param, Entity owner)
        {
            Obj = obj;
            Param = param;
            Type objType = obj.GetType();
            Owner = owner;
            Owner.NotifyOfComponentChange(ClassFactory.GetReferedName(Obj.GetType()) + '.' + Param, change);
        }

        internal TypeMatcher GetTypeMatcher() { return ClassFactory.GetParam(Obj, Param) as TypeMatcher; }

        internal void NotifyOfChange(SysAction callback) { Callbacks.Add(callback); }

        //If it makes it easier, make the callback with TypeMatcher or change dc to callback empty
        private void change() { foreach (SysAction callback in Callbacks) callback(); }
    }
}
