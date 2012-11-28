using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    public abstract class ValueWriter : RepeatReader
    {
        private readonly ValueReader ReaderVal;
        protected override ValueReader Reader { get { return ReaderVal; } }

        internal ValueWriter(ValueReader reader) { ReaderVal = reader; }
        internal override void SetupNotifications() { ReaderVal.NotifyOfChange(this); }
        public void SetValue(object value) { SetValue(new ConstantReader(value)); }
        public abstract void SetValue(ValueReader value);
        internal override ValueWriter ConvertToWriter() { return this; }
    }
}
