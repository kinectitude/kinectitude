//-----------------------------------------------------------------------
// <copyright file="ConstantReader.cs" company="Kinectitude">
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
    public sealed class ConstantReader : ValueReader
    {
        //We can get the float from the double with the same persision it had before?
        private readonly double Dval;
        private readonly string Sval;
        private readonly bool Bval;
        private readonly PreferedType Type;

        //Used by DataContainer for null values and ConstantValueReader default
        public static readonly ConstantReader NullValue = new ConstantReader(null);
        //Used by boolean based value reader defaults
        public static readonly ConstantReader TrueValue = new ConstantReader(true);
        public static readonly ConstantReader FalseValue = new ConstantReader(false);

       private static readonly Dictionary<object, ConstantReader> SavedValues = new Dictionary<object,ConstantReader>(){{true, TrueValue}, {false, FalseValue}};

        public ConstantReader(object value)
        {
            Type = NativeReturnType(value);
            Dval = ToNumber<double>(value);
            if (value != null) Sval = value.ToString();
            else Sval = "";
            Bval = ToBool(value);
        }

        /*TODO decide if this should actually be public.
         * Advantage is that less are created, disadvantage it can be misused by mistake and tons of dict items could be created
         * Maybe use WeakReference and check the dict every once in a while?
         */
        public static ConstantReader CacheOrCreate(object value)
        {
            if (null == value) return NullValue;
            ConstantReader val;
            if(!SavedValues.TryGetValue(value, out val))
            {
                val = new ConstantReader(value);
                SavedValues[val] = val;
            }
            return val;
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
