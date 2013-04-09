//-----------------------------------------------------------------------
// <copyright file="TypeMatcherWatcher.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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

        internal static TypeMatcherWatcher GetTypeMatcherWatcher(object obj, string param, IDataContainer owner)
        {
            Func<TypeMatcherWatcher> create = new Func<TypeMatcherWatcher>(() => new TypeMatcherWatcher(obj, param, owner));
            return DoubleDictionary<object, string, TypeMatcherWatcher>.GetItem(obj, param, create);

        }

        private TypeMatcherWatcher(object obj, string param, IDataContainer owner)
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
