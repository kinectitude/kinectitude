using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    internal sealed class DelegateDataContainer : IDataContainer
    {
        private readonly Func<string, ValueReader> getter;
        private readonly Func<string, IChangeable> changeableFunc;
        private readonly Dictionary<string, IChangeable> changeables;

        public ValueReader this[string key]
        {
            get { return getter(key); }
            set { throw new NotSupportedException(); }
        }

        public DelegateDataContainer(Func<string, ValueReader> getterFunc, Func<string, IChangeable> changeableFunc)
        {
            this.getter = getterFunc;
            this.changeableFunc = changeableFunc;

            changeables = new Dictionary<string, IChangeable>();
        }

        IChangeable IDataContainer.GetChangeable(string name)
        {
            IChangeable changeable;
            changeables.TryGetValue(name, out changeable);

            if (null == changeable)
            {
                changeable = changeableFunc(name);
                changeables[name] = changeable;
            }

            return changeable;
        }

        void IDataContainer.NotifyOfChange(string key, IChanges callback) { }
        void IDataContainer.NotifyOfComponentChange(Tuple<IChangeable, string> what, IChanges callback) { }
    }
}
