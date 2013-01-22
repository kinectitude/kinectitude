using Kinectitude.Core.Base;
using Kinectitude.Editor.Models.Data.DataContainers;
using Kinectitude.Editor.Models.Data.ValueReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.Changeables
{
    internal sealed class ServiceChangeable : IChangeable
    {
        private readonly GameDataContainer container;
        private readonly string type;
        private readonly Dictionary<string, ServiceValueReader> properties;

        public Service Service
        {
            get
            {
                Service service = null;

                var game = container.Game;
                if (null != game)
                {
                    service = game.GetServiceByDefinedName(type);
                }

                return service;
            }
        }

        public ServiceChangeable(GameDataContainer container, string type)
        {
            this.container = container;
            this.type = type;
            properties = new Dictionary<string, ServiceValueReader>();
        }

        #region IChangeable implementation

        object IChangeable.this[string parameter]
        {
            get
            {
                ServiceValueReader reader;
                properties.TryGetValue(parameter, out reader);

                if (null == reader)
                {
                    reader = new ServiceValueReader(this, parameter);
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
