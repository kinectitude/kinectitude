using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.DataContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.ValueReaders
{
    internal sealed class AmbiguousValueReader : RepeatReader
    {
        private readonly AmbiguousDataContainer container;
        private readonly string attributeOrPropertyName;

        protected override ValueReader Reader
        {
            get
            {
                var entity = container.NamedEntity;
                if (null != entity)
                {
                    var attribute = entity.GetAttribute(attributeOrPropertyName);
                    if (null != attribute)
                    {
                        return attribute.Value.Reader;
                    }
                }
                else
                {
                    var component = container.ThisComponent;
                    if (null != component)
                    {
                        var property = component.GetProperty(attributeOrPropertyName);
                        if (null != property)
                        {
                            return property.Value.Reader;
                        }
                    }
                }

                return ConstantReader.NullValue;
            }
        }

        public AmbiguousValueReader(AmbiguousDataContainer container, string attributeOrPropertyName)
        {
            this.container = container;
            this.attributeOrPropertyName = attributeOrPropertyName;
        }

        internal override ValueWriter ConvertToWriter()
        {
            throw new NotSupportedException();
        }
    }
}
