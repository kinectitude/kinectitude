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

        internal ConstantReader(object value)
        {

            Dval = ToNumber<double>(value);
            Sval = value.ToString();
            Type = NativeReturnType(value);
            Bval = ToBool(value);
        }

        internal override double GetDoubleValue() { return Dval; }
        internal override float GetFloatValue() { return (float)Dval; }
        internal override int GetIntValue() { return (int)Dval; }
        internal override long GetLongValue() { return (long)Dval; }
        internal override bool GetBoolValue() { return Bval; }
        internal override string GetStrValue() { return Sval; }
        internal override PreferedType PreferedRetType() { return Type; }
        internal override void notifyOfChange(Action<ValueReader> change) { return; }
        internal override ValueWriter ConvertToWriter() { return new NullWriter(this); }
    }
}
