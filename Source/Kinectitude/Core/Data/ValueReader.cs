using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal enum PreferedType { String, Number, Boolean, Null }

    public abstract class ValueReader
    {
        protected readonly List<Action<ValueReader>> Callbacks = new List<Action<ValueReader>>();
        //TODO change to a null reader when I make one
        private ConstantReader oldValue = null;

        protected ValueWriter Writer { get; private set; }

        /*Inconsistent (can be fixed by making dc hold objects
         * entity.x = true
         * vs entity.x = "true"
         * both give 0, but true + 1 = 2?
         */
        internal static T ToNumber<T>(object obj) where T : struct
        {
            if (null == obj) return (T)Convert.ChangeType(0, typeof(T));
            if (typeof(string) == obj.GetType())
            {
                int i = 0;
                int.TryParse(obj as string, out i);
                return (T)Convert.ChangeType(i, typeof(T));
            }
            if (typeof(bool) == obj.GetType())
            {
                int i = (bool)obj ? 1 : 0;
                return (T)Convert.ChangeType(i, typeof(T));
            }
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        internal static bool ToBool(object obj)
        {
            if (null == obj) return false;
            Type type = obj.GetType();
            if (typeof(bool) == type)
            {
                return (bool)obj;
            }
            if (typeof(string) == type)
            {
                string strResult = obj as string;
                return null != strResult && strResult.Length != 0 && strResult.ToLower() != "false";
            }
            return 0 != (double)Convert.ChangeType(obj, typeof(double));
        }


        internal static PreferedType NativeReturnType(object value)
        {
            if (null == value) return PreferedType.Null;
            if (value.GetType() == typeof(string)) return PreferedType.String;
            else if (value.GetType() == typeof(bool)) return PreferedType.Boolean;
            return PreferedType.Number;
        }

        internal object GetPreferedValue()
        {
            switch (PreferedRetType())
            {
                case PreferedType.Boolean: return GetBoolValue();
                case PreferedType.Number: return GetDoubleValue();
                default: return GetStrValue();
            }
        }

        internal abstract double GetDoubleValue();
        public static implicit operator double(ValueReader reader) { return reader.GetDoubleValue(); }
        internal abstract float GetFloatValue();
        public static implicit operator float(ValueReader reader) { return reader.GetFloatValue(); }
        internal abstract int GetIntValue();
        public static implicit operator int(ValueReader reader) { return reader.GetIntValue(); }
        internal abstract long GetLongValue();
        public static implicit operator long(ValueReader reader) { return reader.GetLongValue(); }
        internal abstract bool GetBoolValue();
        public static implicit operator bool(ValueReader reader) { return reader.GetBoolValue(); }
        internal abstract string GetStrValue();
        public static implicit operator string(ValueReader reader) { return reader.GetStrValue(); }
        internal abstract PreferedType PreferedRetType();

        internal void notifyOfChange(Action<ValueReader> change) { Callbacks.Add(change); }

        internal bool hasSameVal(ValueReader other)
        {
            if (other == null) return false;
            if (PreferedRetType() != other.PreferedRetType()) return false;
            switch (PreferedRetType())
            {
                case PreferedType.Boolean: return GetBoolValue() == other.GetBoolValue();
                case PreferedType.Number: return GetDoubleValue() == other.GetDoubleValue();
                default: return GetStrValue() == other.GetStrValue();
            }
        }

        internal abstract ValueWriter ConvertToWriter();
        private ValueWriter toValueWriter()
        {
            if (null == Writer) Writer = ConvertToWriter();
            return Writer;
        }

        public static ValueWriter GetValueWriter(ValueReader reader) { return reader.toValueWriter(); }

        protected void Change()
        {
            if (!hasSameVal(oldValue))
            {
                foreach (Action<ValueReader> callback in Callbacks) callback(this);
                oldValue = new ConstantReader(this.GetPreferedValue());
            }
        }

        //many value readers notify of change from another vr so use this function instead of creating delegates
        protected void Change(ValueReader vr) { Change(); }
    }
}
