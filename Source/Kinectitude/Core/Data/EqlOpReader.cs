using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class EqlOpReader : BinOpReader
    {
        internal override ConstantReader NullEquals { get { return ConstantReader.TrueValue; } }
        internal EqlOpReader(ValueReader left, ValueReader right) : base(left, right) { }
        internal override bool GetBoolValue() { return Left.HasSameVal(Right); }
        internal override string GetStrValue() { return Left.HasSameVal(Right).ToString(); }
        internal override double GetDoubleValue() { return Left.HasSameVal(Right) ? 1 : 0; }
        internal override float GetFloatValue() { return Left.HasSameVal(Right) ? 1 : 0; }
        internal override int GetIntValue() { return Left.HasSameVal(Right) ? 1 : 0; }
        internal override long GetLongValue() { return Left.HasSameVal(Right) ? 1 : 0; }
        internal override PreferedType PreferedRetType() { return PreferedType.Boolean; }
    }
}
