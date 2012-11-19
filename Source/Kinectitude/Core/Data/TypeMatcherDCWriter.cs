using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal class TypeMatcherDCWriter : ValueWriter
    {
        private readonly TypeMatcherDCReader reader;
        internal TypeMatcherDCWriter(TypeMatcherDCReader reader) : base(reader)
        {
            this.reader = reader;
        }

        public override void SetValue(ValueReader value)
        {
            reader.Watcher.GetTypeMatcher()[reader.Param] = value;
        }
    }
}
