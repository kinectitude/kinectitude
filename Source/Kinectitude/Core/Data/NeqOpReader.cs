using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class NeqOpReader : BinOpReader
    {
        internal NeqOpReader(ValueReader left, ValueReader right) : base(left, right) { }
        internal override bool GetBoolValue() { return Left != Right; }
        internal override string GetStrValue() { return (Left != Right).ToString(); }
        internal override double GetDoubleValue() { return Left != Right ? 1 : 0; }
        internal override float GetFloatValue() { return Left != Right ? 1 : 0; }
        internal override int GetIntValue() { return Left != Right ? 1 : 0; }
        internal override long GetLongValue() { return Left != Right ? 1 : 0; }
        internal override PreferedType PreferedRetType() { return PreferedType.Boolean; }
    }
}
