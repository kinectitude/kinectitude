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
        internal readonly string ComponentName;
        internal readonly string Param;

        private Component component = null; 
        private Entity lastEntity = null;

        protected override ValueReader Reader
        {
            get { return Watcher.GetTypeMatcher().GetComponentValue(ComponentName, Param); }
        }

        internal static TypeMatcherProperyReader GetTypeMatcherProperyReader
            (object obj, string objParam, string component, string param, IDataContainer owner)
        {
            TypeMatcherWatcher watcher = TypeMatcherWatcher.GetTypeMatcherWatcher(obj, objParam, owner);
            Func<TypeMatcherProperyReader> create = 
                new Func<TypeMatcherProperyReader>(() => new TypeMatcherProperyReader(watcher, component, param));
            string fullParam = component + "." + param;
            return DoubleDictionary<TypeMatcherWatcher, string, TypeMatcherProperyReader>.GetItem(watcher, fullParam, create);
        }

        private TypeMatcherProperyReader(TypeMatcherWatcher watcher, string component, string param)
        {
            ComponentName = component;
            Param = param;
            Watcher = watcher;
            TypeMatcher matcher = Watcher.GetTypeMatcher();
            if(matcher.Entity != null) this.component = matcher.Entity.GetComponent(component);
            Watcher.NotifyOfChange(this);
        }

        private void typeMatcherChange()
        {
            if (lastEntity != null) lastEntity.UnnotifyOfComponentChange(new Tuple<IChangeable,string>(component, Param), this);
            Entity entity = Watcher.GetTypeMatcher().Entity;
            if (entity != null)
            {
                component = entity.GetComponent(ComponentName);
                lastEntity = entity;
                ((IDataContainer)entity).NotifyOfComponentChange(new Tuple<IChangeable, string>(component, Param), this);
            }
            ((IChanges)this).Change();
        }

        internal override ValueWriter ConvertToWriter() { return new TypeMatcherProperyWriter(this); }
    }
}
