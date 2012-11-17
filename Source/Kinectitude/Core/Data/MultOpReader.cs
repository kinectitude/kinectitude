using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal sealed class MultOpReader : BinOpReader
    {
        internal override ConstantReader NullEquals { get { return ConstantReader.ZeroValue; } }
        internal MultOpReader(ValueReader left, ValueReader right) : base(left, right) { }

        private static string repeateString(string str, double numTimes)
        {
            long times = (long)numTimes;
            int numChars = (int)((numTimes - times) * str.Length);
            if (null == str || "" == str) return str;
            StringBuilder sb = new StringBuilder(str);
            for (long x = 0; x < times - 1; x++) sb.Append(str);
            sb.Append(str.Substring(0, numChars));
            return sb.ToString();
        }

        internal override bool  GetBoolValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return ToBool(Left.GetDoubleValue() * Right.GetDoubleValue());
            }

            bool convertLeft = true;

            if (Left.PreferedRetType() == PreferedType.String && Right.PreferedRetType() == PreferedType.String)
            {
                double numTimes = 0;
                convertLeft = Double.TryParse(Left.GetStrValue(), out numTimes);
                if(!convertLeft && !Double.TryParse(Right.GetStrValue(), out numTimes)) return false;
            }

            if (convertLeft)
            {
                if (Right.GetLongValue() != 1) return false;
                return ToBool(Right.GetStrValue());
            }

            if (Left.GetLongValue() != 1) return false;
            return ToBool(Left.GetStrValue());
        }

        internal override string GetStrValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return (Left.GetDoubleValue() * Right.GetDoubleValue()).ToString();
            }

            bool convertLeft = true;

            if (Left.PreferedRetType() == PreferedType.String && Right.PreferedRetType() == PreferedType.String)
            {
                double numTimes = 0;
                convertLeft = Double.TryParse(Left.GetStrValue(), out numTimes);
                if (!convertLeft && !Double.TryParse(Right.GetStrValue(), out numTimes)) return "";
            }

            if (convertLeft)
            {
                return repeateString(Right.GetStrValue(), Left.GetDoubleValue());
            }

            return repeateString(Left.GetStrValue(), Right.GetDoubleValue());
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
                return Left.GetDoubleValue() * Right.GetDoubleValue();
            }

            bool convertLeft = true;

            if (Left.PreferedRetType() == PreferedType.String && Right.PreferedRetType() == PreferedType.String)
            {
                double numTimes = 0;
                convertLeft = Double.TryParse(Left.GetStrValue(), out numTimes);
                if (!convertLeft && !Double.TryParse(Right.GetStrValue(), out numTimes)) return 0;
            }

            if (convertLeft)
            {
                return ToNumber<double>(repeateString(Right.ToString(), Left.GetDoubleValue()));
            }

            return ToNumber<double>(repeateString(Left.ToString(), Right.GetDoubleValue()));
        }

        internal override float GetFloatValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return Left.GetFloatValue() * Right.GetFloatValue();
            }

            bool convertLeft = true;

            if (Left.PreferedRetType() == PreferedType.String && Right.PreferedRetType() == PreferedType.String)
            {
                double numTimes = 0;
                convertLeft = Double.TryParse(Left.GetStrValue(), out numTimes);
                if (!convertLeft && !Double.TryParse(Right.GetStrValue(), out numTimes)) return 0;
            }

            if (convertLeft)
            {
                return ToNumber<float>(repeateString(Right.ToString(), Left.GetDoubleValue()));
            }

            return ToNumber<float>(repeateString(Left.ToString(), Right.GetDoubleValue()));
        }

        internal override int GetIntValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return Left.GetIntValue() * Right.GetIntValue();
            }

            bool convertLeft = true;

            if (Left.PreferedRetType() == PreferedType.String && Right.PreferedRetType() == PreferedType.String)
            {
                double numTimes = 0;
                convertLeft = Double.TryParse(Left.GetStrValue(), out numTimes);
                if (!convertLeft && !Double.TryParse(Right.GetStrValue(), out numTimes)) return 0;
            }

            if (convertLeft)
            {
                return ToNumber<int>(repeateString(Right.ToString(), Left.GetDoubleValue()));
            }

            return ToNumber<int>(repeateString(Left.ToString(), Right.GetDoubleValue()));
        }

        internal override long GetLongValue()
        {
            if (PreferedRetType() != PreferedType.String)
            {
                return Left.GetLongValue() * Right.GetLongValue();
            }

            bool convertLeft = true;

            if (Left.PreferedRetType() == PreferedType.String && Right.PreferedRetType() == PreferedType.String)
            {
                double numTimes = 0;
                convertLeft = Double.TryParse(Left.GetStrValue(), out numTimes);
                if (!convertLeft && !Double.TryParse(Right.GetStrValue(), out numTimes)) return 0;
            }

            if (convertLeft)
            {
                return ToNumber<long>(repeateString(Right.ToString(), Left.GetDoubleValue()));
            }

            return ToNumber<long>(repeateString(Left.ToString(), Right.GetDoubleValue()));
        }
    }
}
