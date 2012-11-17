using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class NegOpReader : UniOpReader
    {
        internal override ConstantReader NullEquals { get { return ConstantReader.ZeroValue; } }
        internal NegOpReader(ValueReader reader) : base(reader) { }
        internal override double GetDoubleValue() { return -Reader.GetDoubleValue(); }
        internal override float GetFloatValue() { return -Reader.GetFloatValue(); }
        internal override int GetIntValue() { return -Reader.GetIntValue(); }
        internal override long GetLongValue() { return -Reader.GetLongValue(); }
        internal override bool GetBoolValue() { return Reader.GetBoolValue(); }
        internal override string GetStrValue() { return (-Reader.GetDoubleValue()).ToString(); }
        internal override PreferedType PreferedRetType() { return PreferedType.Number; }
    }
}
