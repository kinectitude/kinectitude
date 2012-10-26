using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    public abstract class ValueWriter : RepeatReader
    {
        internal ValueWriter(ValueReader reader) { Reader = reader; }

        public void SetValue(object value) { SetValue(new ConstantReader(value)); }
        public abstract void SetValue(ValueReader value);

        //This won't be called on a writer
        internal override void notifyOfChange(Action<ValueReader> change) { return; }

        internal override ValueWriter ConvertToWriter() { return this; }
    }
}
