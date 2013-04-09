//-----------------------------------------------------------------------
// <copyright file="SceneValueReader.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.DataContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.ValueReaders
{
    internal sealed class SceneValueReader : RepeatReader
    {
        private readonly SceneDataContainer container;
        private readonly string key;

        protected override ValueReader Reader
        {
            get
            {
                var owner = container.Scene;
                if (null != owner)
                {
                    var attribute = owner.GetAttribute(key);
                    if (null != attribute)
                    {
                        return attribute.Value.Reader;
                    }
                }

                return ConstantReader.NullValue;
            }
        }

        public SceneValueReader(SceneDataContainer container, string key)
        {
            this.container = container;
            this.key = key;
        }

        internal override ValueWriter ConvertToWriter()
        {
            throw new NotSupportedException();
        }
    }
}
