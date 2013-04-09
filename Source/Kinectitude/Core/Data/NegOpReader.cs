//-----------------------------------------------------------------------
// <copyright file="NegOpReader.cs" company="Kinectitude">
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
    internal sealed class NegOpReader : UniOpReader
    {
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
