using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.Changeables;
using Kinectitude.Editor.Models.Data.ValueReaders;
using Kinectitude.Editor.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    /// <summary>
    /// This is a wrapper class to serve as the entity IDataContainer for a ValueReader in the editor.
    /// A ThisDataContainer is associated with a Value as soon as it is created. If that Value is in
    /// an entity's scope, the ThisDataContainer will be able to get data from that entity. If the Value
    /// is not in any entity's scope, attempts to access data will return ConstantReader.NullValue.
    /// 
    /// If the scope of the Value changes, this class will publish the appropriate notifications.
    /// 
    /// If the owning entity changes:
    ///     - Publish changes for all attributes and 
    /// </summary>
    internal sealed class ThisDataContainer : IDataContainer
    {
        private readonly Value value;
        private readonly Dictionary<string, ThisEntityValueReader> attributes;
        private readonly Dictionary<string, ThisChangeable> changeables;

        public Entity Entity
        {
            get { return value.Entity; }
        }

        public ThisDataContainer(Value value)
        {
            this.value = value;

            attributes = new Dictionary<string, ThisEntityValueReader>();
            changeables = new Dictionary<string, ThisChangeable>();
        }

        #region IDataContainer implementation

        ValueReader IDataContainer.this[string key]
        {
            get
            {
                ThisEntityValueReader reader;
                attributes.TryGetValue(key, out reader);

                if (null == reader)
                {
                    reader = new ThisEntityValueReader(this, key);
                    attributes[key] = reader;
                }

                return reader;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        IChangeable IDataContainer.GetChangeable(string name)
        {
            ThisChangeable changeable;
            changeables.TryGetValue(name, out changeable);

            if (null == changeable)
            {
                changeable = new ThisChangeable(this, name);
                changeables[name] = changeable;
            }

            return changeable;
        }

        void IDataContainer.NotifyOfChange(string key, IChanges callback)
        {
            throw new NotImplementedException();
        }

        void IDataContainer.NotifyOfComponentChange(Tuple<IChangeable, string> what, IChanges callback)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
