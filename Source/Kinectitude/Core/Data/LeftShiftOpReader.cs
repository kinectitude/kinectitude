//-----------------------------------------------------------------------
// <copyright file="LeftShiftOpReader.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class LeftShiftOpReader : BinOpReader
    {
        internal LeftShiftOpReader(ValueReader left, ValueReader right) : base(left, right) { }
        internal override bool GetBoolValue() { return ToBool(Left.GetLongValue() << Right.GetIntValue()); }
        internal override string GetStrValue() { return (Left.GetLongValue() << Right.GetIntValue()).ToString(); }
        internal override double GetDoubleValue() { return Left.GetLongValue() << Right.GetIntValue(); }
        internal override float GetFloatValue() { return Left.GetLongValue() << Right.GetIntValue(); }
        internal override int GetIntValue() { return Left.GetIntValue() << Right.GetIntValue(); }
        internal override long GetLongValue() { return Left.GetLongValue() << Right.GetIntValue(); }
        internal override PreferedType PreferedRetType() { return PreferedType.Number; }
    }
}
