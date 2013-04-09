//-----------------------------------------------------------------------
// <copyright file="RepeatReader.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
namespace Kinectitude.Core.Data
{
    public abstract class RepeatReader : ValueReader
    {
        protected abstract ValueReader Reader { get; }
        internal override double GetDoubleValue() { return Reader.GetDoubleValue(); }
        internal override float GetFloatValue() { return Reader.GetFloatValue(); }
        internal override int GetIntValue() { return Reader.GetIntValue(); }
        internal override long GetLongValue() { return Reader.GetLongValue(); }
        internal override bool GetBoolValue() { return Reader.GetBoolValue(); }
        internal override string GetStrValue() { return Reader.GetStrValue(); }
        internal override PreferedType PreferedRetType() { return Reader.PreferedRetType(); }
    }
}
