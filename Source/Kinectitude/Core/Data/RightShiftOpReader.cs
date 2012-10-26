using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class RightShiftOpReader : BinOpReader
    {
        internal RightShiftOpReader(ValueReader left, ValueReader right) : base(left, right) { }
        internal override bool GetBoolValue() { return ToBool(Left.GetLongValue() >> Right.GetIntValue()); }
        internal override string GetStrValue() { return (Left.GetLongValue() >> Right.GetIntValue()).ToString(); }
        internal override double GetDoubleValue() { return Left.GetLongValue() >> Right.GetIntValue(); }
        internal override float GetFloatValue() { return Left.GetLongValue() >> Right.GetIntValue(); }
        internal override int GetIntValue() { return Left.GetIntValue() >> Right.GetIntValue(); }
        internal override long GetLongValue() { return Left.GetLongValue() >> Right.GetIntValue(); }
        internal override PreferedType PreferedRetType() { return PreferedType.Number; }
    }
}
