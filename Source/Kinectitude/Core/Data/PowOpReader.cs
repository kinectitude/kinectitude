//-----------------------------------------------------------------------
// <copyright file="PowOpReader.cs" company="Kinectitude">
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
    internal sealed class PowOpReader : BinOpReader
    {
        internal PowOpReader(ValueReader left, ValueReader right) : base(left, right) { }
        internal override bool GetBoolValue() { return ToBool(Math.Pow(Left.GetDoubleValue(), Right.GetDoubleValue())); }
        internal override string GetStrValue() { return Math.Pow(Left.GetDoubleValue(), Right.GetDoubleValue()).ToString(); }
        internal override PreferedType PreferedRetType() { return PreferedType.Number; }
        internal override double GetDoubleValue() { return Math.Pow(Left.GetDoubleValue(), Right.GetDoubleValue()); }
        internal override float GetFloatValue() { return (float)Math.Pow(Left.GetFloatValue(), Right.GetFloatValue()); }
        internal override int GetIntValue() { return (int)Math.Pow(Left.GetIntValue(), Right.GetIntValue()); }
        internal override long GetLongValue() { return (long)Math.Pow(Left.GetLongValue(), Right.GetLongValue()); }
    }
}
