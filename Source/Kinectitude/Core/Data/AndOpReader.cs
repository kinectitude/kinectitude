using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class AndOpReader : BinOpReader
    {
        internal AndOpReader(ValueReader left, ValueReader right) : base(left, right) { }
        internal override bool GetBoolValue() { return Left.GetBoolValue() && Right.GetBoolValue(); }
        internal override string GetStrValue() { return (Left.GetBoolValue() && Right.GetBoolValue()).ToString(); }
        internal override PreferedType PreferedRetType() { return PreferedType.Boolean; }
        internal override double GetDoubleValue() { return Left.GetBoolValue() && Right.GetBoolValue() ? 1 : 0; }
        internal override float GetFloatValue() { return Left.GetBoolValue() && Right.GetBoolValue() ? 1 : 0; }
        internal override int GetIntValue() { return Left.GetBoolValue() && Right.GetBoolValue() ? 1 : 0; }
        internal override long GetLongValue() { return Left.GetBoolValue() && Right.GetBoolValue() ? 1 : 0; }
    }
}
