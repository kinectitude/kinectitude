using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.DataContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.ValueReaders
{
    internal sealed class EntityValueReader : RepeatReader
    {
        private readonly EntityDataContainer container;
        private readonly string key;

        protected override ValueReader Reader
        {
            get
            {
                var entity = container.Entity;
                if (null != entity)
                {
                    var attribute = entity.GetAttribute(key);
                    if (null != attribute)
                    {
                        return attribute.Value.Reader;
                    }
                }

                return ConstantReader.NullValue;
            }
        }

        public EntityValueReader(EntityDataContainer container, string key)
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
