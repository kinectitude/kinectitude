using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal abstract class BinOpReader : ValueReader
    {
        protected readonly ValueReader Left;
        protected readonly ValueReader Right;

        object oldVal;

        protected BinOpReader(ValueReader left, ValueReader right)
        {
            Left = left;
            Right = right;
            oldVal = GetPreferedValue();
        }

        internal override void notifyOfChange(Action<ValueReader> change)
        {
            Left.notifyOfChange(new Action<ValueReader>(vr => SideChange()));
            Right.notifyOfChange(new Action<ValueReader>(vr => SideChange()));
            Callbacks.Add(change);
        }

        private void SideChange()
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