using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class PlusOpReader : BinOpReader
    {
        internal override ConstantReader NullEquals { get { return ConstantReader.ZeroValue; } }

        internal PlusOpReader(ValueReader left, ValueReader right) : base(left, right) { }

        internal override bool  GetBoolValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return ToBool(Left.GetDoubleValue() + Right.GetDoubleValue());
            }
            return ToBool(Left.GetStrValue() + Right.GetStrValue());
        }

        internal override string GetStrValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return (Left.GetDoubleValue() + Right.GetDoubleValue()).ToString();
            }
            return Left.GetStrValue() + Right.GetStrValue();
        }

        internal override PreferedType PreferedRetType()
        {
            if (Left.PreferedRetType() == PreferedType.String || Right.PreferedRetType() == PreferedType.String)
                return PreferedType.String;
            return PreferedType.Number;
        }

        internal override double GetDoubleValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return Left.GetDoubleValue() + Right.GetDoubleValue();
            }
            double num = 0;
            double.TryParse(Left.GetStrValue() + Right.GetStrValue(), out num);
            return num;
        }

        internal override float GetFloatValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return Left.GetFloatValue() + Right.GetFloatValue();
            }
            float num = 0;
            float.TryParse(Left.GetStrValue() + Right.GetStrValue(), out num);
            return num;
        }

        internal override int GetIntValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return Left.GetIntValue() + Right.GetIntValue();
            }
            int num = 0;
            int.TryParse(Left.GetStrValue() + Right.GetStrValue(), out num);
            return num;
        }

        internal override long GetLongValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return Left.GetLongValue() + Right.GetLongValue();
            }
            long num = 0;
            long.TryParse(Left.GetStrValue() + Right.GetStrValue(), out num);
            return num;
        }
    }
}
