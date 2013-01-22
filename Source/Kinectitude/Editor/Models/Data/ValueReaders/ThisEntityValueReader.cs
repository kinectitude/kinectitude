using Kinectitude.Core.Data;
using Kinectitude.Editor.Models.Data.DataContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Data.ValueReaders
{
    /// <summary>
    /// A ValueReader for a local attribute that may or may not exist. A ThisValueReader is not
    /// tied to any entity at one time. It gets whatever Entity owns the associated Value object
    /// at the time it is used.
    /// </summary>
    internal sealed class ThisEntityValueReader : RepeatReader
    {
        private readonly ThisDataContainer container;
        private readonly string key;

        protected override ValueReader Reader
        {
            get
            {
                var owner = container.Entity;
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

        public ThisEntityValueReader(ThisDataContainer container, string key)
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
