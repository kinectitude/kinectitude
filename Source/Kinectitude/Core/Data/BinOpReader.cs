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

        protected BinOpReader(ValueReader left, ValueReader right)
        {
            Left = left;
            Right = right;
            Left.notifyOfChange(Change);
            Right.notifyOfChange(Change);
        }

        internal override ValueWriter ConvertToWriter() { return new NullWriter(this); }
    }
}