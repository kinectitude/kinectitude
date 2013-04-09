//-----------------------------------------------------------------------
// <copyright file="BaseChangeable.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.DataContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.Changeables
{
    internal abstract class BaseChangeable : IChangeable
    {
        private readonly BaseDataContainer container;
        private readonly Dictionary<string, ValueReader> properties;

        protected BaseChangeable(BaseDataContainer container)
        {
            this.container = container;
            this.properties = new Dictionary<string, ValueReader>();
        }

        protected void PublishComponentChange()
        {
            container.PublishComponentChange(this);
        }

        protected void PublishPropertyChange(string property)
        {
            container.PublishComponentChange(this, property);
        }

        protected abstract ValueReader CreatePropertyReader(string name);

        #region IChangeable implementation

        object IChangeable.this[string parameter]
        {
            get
            {
                ValueReader reader;
                properties.TryGetValue(parameter, out reader);

                if (null == reader)
                {
                    reader = CreatePropertyReader(parameter);
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
