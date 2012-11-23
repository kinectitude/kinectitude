using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class TypeMatcherProperyWriter : ValueWriter
    {
        private readonly TypeMatcherProperyReader reader;
        internal TypeMatcherProperyWriter(TypeMatcherProperyReader reader) : base(reader)
        {
            this.reader = reader;
        }

        public override void SetValue(ValueReader value)
        {
            reader.Watcher.GetTypeMatcher().setComponentValue(reader.Component, reader.Param, value);
        }
    }
}
