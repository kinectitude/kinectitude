using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models.Values
{
    internal sealed class BooleanValue : Value
    {
        private static readonly bool[] values = { true, false };

        private bool currentValue;

        public IEnumerable<bool> PossibleValues
        {
            get { return values; }
        }

        public BooleanValue() { }
    }
}
