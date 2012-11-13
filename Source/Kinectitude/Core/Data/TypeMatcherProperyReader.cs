using Kinectitude.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class TypeMatcherProperyReader : RepeatReader
    {
        internal readonly TypeMatcherWatcher Watcher;
        internal readonly string What;
        internal readonly string Component;
        internal readonly string Param;

        private Entity lastEntity = null;

        protected override ValueReader Reader
        {
            get { return Watcher.GetTypeMatcher().getComponentValue(Component, Param); }
        }

        internal static TypeMatcherProperyReader getTypeMatcherProperyReader
            (object obj, string objParam, string component, string param,  Entity owner)
        {
            TypeMatcherWatcher watcher = TypeMatcherWatcher.getTypeMatcherWatcher(obj, objParam, owner);
            Func<TypeMatcherProperyReader> create = 
                new Func<TypeMatcherProperyReader>(() => new TypeMatcherProperyReader(watcher, component, param));
            string fullParam = component + "." + param;
            return DoubleDictionary<TypeMatcherWatcher, string, TypeMatcherProperyReader>.getItem(watcher, fullParam, create);
        }

        private TypeMatcherProperyReader(TypeMatcherWatcher watcher, string component, string param)
        {
            Component = component;
            Param = param;
            What = component + "." + param;
            Watcher = watcher;
            TypeMatcher matcher = Watcher.GetTypeMatcher();
            Watcher.NotifyOfChange(typeMatcherChange);
        }

        private void typeMatcherChange()
        {
            if (lastEntity != null) lastEntity.UnnotifyOfComponentChange(What, Change);
            Entity entity = Watcher.GetTypeMatcher().Entity;
            lastEntity = entity;
            entity.NotifyOfComponentChange(What, Change);
            Change();
        }
        //TODO
        internal override ValueWriter ConvertToWriter() { return new TypeMatcherProperyWriter(this); }
    }
}
