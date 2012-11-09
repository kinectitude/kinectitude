using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal abstract class UniOpReader : ValueReader
    {
        protected readonly ValueReader Reader;
        private object oldVal;

        protected UniOpReader(ValueReader reader)
        { 
            Reader = reader;
            oldVal = GetPreferedValue();
            reader.notifyOfChange(Change);
        }

        internal override ValueWriter ConvertToWriter() { return new NullWriter(this); }
    }
}
