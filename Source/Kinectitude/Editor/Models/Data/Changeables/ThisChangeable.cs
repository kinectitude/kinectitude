using Kinectitude.Core.Base;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Data.ValueReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.Changeables
{
    internal sealed class ThisChangeable : IChangeable
    {
        private readonly ThisDataContainer container;
        private readonly string type;
        private readonly Dictionary<string, ThisChangeableValueReader> properties;

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

        public ThisChangeable(ThisDataContainer container, string type)
        {
            this.container = container;
            this.type = type;
            properties = new Dictionary<string, ThisChangeableValueReader>();
        }

        #region IChangeable implementation

        public object this[string parameter]
        {
            get
            {
                ThisChangeableValueReader reader;
                properties.TryGetValue(parameter, out reader);

                if (null == reader)
                {
                    reader = new ThisChangeableValueReader(this, parameter);
                    properties[parameter] = reader;
                }

                return reader;
            }
        }

        public bool ShouldCheck
        {
            get { return true; }    // TODO: What should I return here? Should it be settable?
            set { throw new NotSupportedException(); }
        }

        #endregion
    }
}
