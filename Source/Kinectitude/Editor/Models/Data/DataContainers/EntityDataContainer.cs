using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.ValueReaders;
using Kinectitude.Editor.Models.Values;
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
        private readonly Dictionary<string, EntityValueReader> attributes;

        private Scene Scene
        {
            get { return value.Scene; }
        }

        public Entity Entity
        {
            get
            {
                Entity entity = null;

                var scene = Scene;
                if (null != scene)
                {
                    entity = scene.GetEntityByName(name);
                }

                return entity;
            }
        }

        public EntityDataContainer(Value value, string name)
        {
            this.value = value;
            this.name = name;
            attributes = new Dictionary<string, EntityValueReader>();
        }

        ValueReader IDataContainer.this[string key]
        {
            get
            {
                EntityValueReader reader;
                attributes.TryGetValue(key, out reader);

                if (null == reader)
                {
                    reader = new EntityValueReader(this, key);
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
