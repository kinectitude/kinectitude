using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class OrOpReader : BinOpReader
    {
        internal override ConstantReader NullEquals { get { return ConstantReader.FalseValue; } }
        internal OrOpReader(ValueReader left, ValueReader right) : base(left, right) { }
        internal override bool GetBoolValue() { return Left.GetBoolValue() || Right.GetBoolValue(); }
        internal override string GetStrValue() { return (Left.GetBoolValue() || Right.GetBoolValue()).ToString(); }
        internal override double GetDoubleValue() { return Left.GetBoolValue() || Right.GetBoolValue() ? 1 : 0; }
        internal override float GetFloatValue() { return Left.GetBoolValue() || Right.GetBoolValue() ? 1 : 0; }
        internal override int GetIntValue() { return Left.GetBoolValue() || Right.GetBoolValue() ? 1 : 0; }
        internal override long GetLongValue() { return Left.GetBoolValue() || Right.GetBoolValue() ? 1 : 0; }
        internal override PreferedType PreferedRetType() { return PreferedType.Boolean; }
    }
}
