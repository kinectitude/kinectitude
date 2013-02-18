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
        private readonly Dictionary<string, ThisComponentValueReader> properties;

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
            properties = new Dictionary<string, ThisComponentValueReader>();
        }

        #region IChangeable implementation

        object IChangeable.this[string parameter]
        {
            get
            {
                ThisComponentValueReader reader;
                properties.TryGetValue(parameter, out reader);

                if (null == reader)
                {
                    reader = new ThisComponentValueReader(this, parameter);
                    properties[parameter] = reader;
                }

                return reader;
            }
        }

        bool IChangeable.ShouldCheck
        {
            get { return true; }    // TODO: What should I return here? Should it be settable?
            set { throw new NotSupportedException(); }
        }

        #endregion
    }
}
