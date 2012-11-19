﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class MinusOpReader : BinOpReader
    {
        internal MinusOpReader(ValueReader left, ValueReader right) : base(left, right) { }
        internal override bool GetBoolValue() { return ToBool(Left.GetDoubleValue() - Right.GetDoubleValue()); }
        internal override string GetStrValue() { return (Left.GetDoubleValue() - Right.GetDoubleValue()).ToString(); }
        internal override PreferedType PreferedRetType() { return PreferedType.Number; }
        internal override double GetDoubleValue() { return Left.GetDoubleValue() - Right.GetDoubleValue(); }
        internal override float GetFloatValue() { return Left.GetFloatValue() - Right.GetFloatValue(); }
        internal override int GetIntValue() { return Left.GetIntValue() - Right.GetIntValue(); }
        internal override long GetLongValue() { return Left.GetLongValue() - Right.GetLongValue(); }
    }
}
