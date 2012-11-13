using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class TypeMatcherDCReader : RepeatReader
    {
        internal readonly TypeMatcherWatcher Watcher;
        internal readonly string Param;
        Entity lastEntity = null;

        protected override ValueReader Reader
        {
            get { return Watcher.GetTypeMatcher()[Param]; }
        }

        internal static TypeMatcherDCReader getTypeMatcherDCValueReader(object obj, string objParam, string param,  Entity owner)
        {
            TypeMatcherWatcher watcher = TypeMatcherWatcher.getTypeMatcherWatcher(obj, objParam, owner);
            Func<TypeMatcherDCReader> create = new Func<TypeMatcherDCReader>(() => new TypeMatcherDCReader(watcher, param));
            return DoubleDictionary<TypeMatcherWatcher, string, TypeMatcherDCReader>.getItem(watcher, param, create);
        }

        private TypeMatcherDCReader(TypeMatcherWatcher watcher, string param)
        {
            Param = param;
            Watcher = watcher;
            TypeMatcher matcher = Watcher.GetTypeMatcher();
            Watcher.NotifyOfChange(Change);
        }

        private void typeMatcherChange()
        {
            if (lastEntity != null) lastEntity.NotifyOfChange(Param, Change);
            Entity entity = Watcher.GetTypeMatcher().Entity;
            lastEntity = entity;
            entity.StopNotifications(Param, Change);
            Change();
        }
        //TODO make this
        internal override ValueWriter ConvertToWriter() { return new TypeMatchreDCWriter(this); }
    }
}
