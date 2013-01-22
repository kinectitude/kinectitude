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
