using Kinectitude.Core.Base;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Data.ValueReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.Changeables
{
    internal sealed class ManagerChangeable : IChangeable
    {
        private readonly SceneDataContainer container;
        private readonly string type;
        private readonly Dictionary<string, ManagerValueReader> properties;

        public Manager Manager
        {
            get
            {
                Manager manager = null;

                var scene = container.Scene;
                if (null != scene)
                {
                    manager = scene.GetManagerByDefinedName(type);
                }

                return manager;
            }
        }

        public ManagerChangeable(SceneDataContainer container, string type)
        {
            this.container = container;
            this.type = type;
            properties = new Dictionary<string, ManagerValueReader>();
        }

        #region IChangeable implementation

        object IChangeable.this[string parameter]
        {
            get
            {
                ManagerValueReader reader;
                properties.TryGetValue(parameter, out reader);

                if (null == reader)
                {
                    reader = new ManagerValueReader(this, parameter);
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
