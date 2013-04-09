//-----------------------------------------------------------------------
// <copyright file="GeOpReader.cs" company="Kinectitude">
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
    internal sealed class GeOpReader : BinOpReader
    {
        internal GeOpReader(ValueReader left, ValueReader right) : base(left, right) { }
        internal override bool GetBoolValue() { return Left.GetDoubleValue() >= Right.GetDoubleValue(); }
        internal override string GetStrValue() { return (Left.GetDoubleValue() >= Right.GetDoubleValue()).ToString(); }
        internal override double GetDoubleValue() { return Left.GetDoubleValue() >= Right.GetDoubleValue() ? 1 : 0; }
        internal override float GetFloatValue() { return Left.GetDoubleValue() >= Right.GetDoubleValue() ? 1 : 0; }
        internal override int GetIntValue() { return Left.GetDoubleValue() >= Right.GetDoubleValue() ? 1 : 0; }
        internal override long GetLongValue() { return Left.GetDoubleValue() >= Right.GetDoubleValue() ? 1 : 0; }
        internal override PreferedType PreferedRetType() { return PreferedType.Boolean; }
    }
}
