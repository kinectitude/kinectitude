using Kinectitude.Core.Base;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Data.ValueReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.Changeables
{
    internal sealed class ComponentChangeable : IChangeable
    {
        private readonly EntityDataContainer container;
        private readonly string type;
        private readonly Dictionary<string, ComponentValueReader> properties;

        public Component Component
        {
            get
            {
                Component component = null;

                var entity = container.Entity;
                if (null != entity)
                {
                    component = entity.GetComponentByType(type);
                }

                return component;
            }
        }

        public ComponentChangeable(EntityDataContainer container, string type)
        {
            this.container = container;
            this.type = type;
            properties = new Dictionary<string, ComponentValueReader>();
        }

        #region IChangeable implementation

        object IChangeable.this[string parameter]
        {
            get
            {
                ComponentValueReader reader;
                properties.TryGetValue(parameter, out reader);

                if (null == reader)
                {
                    reader = new ComponentValueReader(this, parameter);
                    properties[parameter] = reader;
                }

                return reader;
            }
        }

        bool IChangeable.ShouldCheck
        {
            get { return true; }
            set { throw new NotSupportedException(); }
        }

        #endregion
    }
}
