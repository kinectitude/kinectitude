using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.DataContainers
{
    internal sealed class EntityDataContainer : IDataContainer
    {
        private readonly Value value;
        private readonly string name;

        public EntityDataContainer(Value value, string name)
        {
            this.value = value;
            this.name = name;
        }

        ValueReader IDataContainer.this[string key]
        {
            get
            {
                // TODO: Get the value from the entity
                return ConstantReader.NullValue;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        IChangeable IDataContainer.GetChangeable(string name)
        {
            throw new NotImplementedException();
        }

        void IDataContainer.NotifyOfChange(string key, IChanges callback)
        {
            throw new NotImplementedException();
        }

        void IDataContainer.NotifyOfComponentChange(Tuple<IChangeable, string> what, IChanges callback)
        {
            throw new NotImplementedException();
        }
    }
}
