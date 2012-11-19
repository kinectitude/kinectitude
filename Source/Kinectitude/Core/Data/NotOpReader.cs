using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class NotOpReader : UniOpReader
    {
        internal NotOpReader(ValueReader reader) : base(reader) { }
        internal override double GetDoubleValue() { return Reader.GetBoolValue() ? 0 : 1; }
        internal override float GetFloatValue() { return Reader.GetBoolValue() ? 0 : 1; }
        internal override int GetIntValue() { return Reader.GetBoolValue() ? 0 : 1; }
        internal override long GetLongValue() { return Reader.GetBoolValue() ? 0 : 1; }
        internal override bool GetBoolValue() { return !Reader.GetBoolValue(); }
        internal override string GetStrValue() { return (!Reader.GetBoolValue()).ToString(); }
        internal override PreferedType PreferedRetType() { return PreferedType.Boolean; }
    }
}
