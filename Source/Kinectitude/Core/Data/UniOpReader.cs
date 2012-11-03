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
        }

        internal override void notifyOfChange(Action<ValueReader> change)
        {
            Callbacks.Add(change);
        }

        private void valChange()
        {
            object value = GetPreferedValue();
            if (oldVal != value)
            {
                foreach (Action<ValueReader> callback in Callbacks) callback(this);
            }
        }
        internal override ValueWriter ConvertToWriter() { return new NullWriter(this); }
    }
}
