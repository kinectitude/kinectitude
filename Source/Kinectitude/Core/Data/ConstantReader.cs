using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    public sealed class ConstantReader : ValueReader
    {
        //We can get the float from the double with the same persision it had before?
        private readonly double Dval;
        private readonly string Sval;
        private readonly bool Bval;
        private readonly PreferedType Type;

        internal static readonly ConstantReader NullValue = new ConstantReader(null);

        internal ConstantReader(object value)
        {
            Type = NativeReturnType(value);
            Dval = ToNumber<double>(value);
            if (value != null) Sval = value.ToString();
            else Sval = "";
            Bval = ToBool(value);
        }

        internal override double GetDoubleValue() { return Dval; }
        internal override float GetFloatValue() { return (float)Dval; }
        internal override int GetIntValue() { return (int)Dval; }
        internal override long GetLongValue() { return (long)Dval; }
        internal override bool GetBoolValue() { return Bval; }
        internal override string GetStrValue() { return Sval; }
        internal override PreferedType PreferedRetType() { return Type; }
        internal override ValueWriter ConvertToWriter() { return new NullWriter(this); }
    }
}
