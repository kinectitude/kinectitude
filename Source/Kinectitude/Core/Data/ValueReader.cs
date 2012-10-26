using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal enum PreferedType { String, Number, Boolean, NoPreference }

    public abstract class ValueReader : IAssignable
    {
        protected readonly List<Action<ValueReader>> Callbacks = new List<Action<ValueReader>>();
        protected ValueWriter Writer { get; private set; }

        /*Inconsistent (can be fixed by making dc hold objects
         * entity.x = true
         * vs entity.x = "true"
         * both give 0, but true + 1 = 2?
         */
        internal static T ToNumber<T>(object obj) where T : struct
        {
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
        internal abstract void notifyOfChange(Action<ValueReader> change);

        //Always will have same string value if they are the same.
        public static bool operator ==(ValueReader a, ValueReader b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.PreferedRetType() != b.PreferedRetType() && a.PreferedRetType() != PreferedType.NoPreference 
                && b.PreferedRetType() != PreferedType.NoPreference) return false;

            return a.GetStrValue() == b.GetStrValue();
        }
        //Making this call return the opposite of == will always hold consistancy (chagnes to == won't require change to !=)
        public static bool operator !=(ValueReader a, ValueReader b) { return !(a == b); }

        public override bool Equals(object obj)
        {
            ValueReader var = obj as ValueReader;
            return var == this;
        }

        public virtual void SetParam(object obj, string param)
        {
            ClassFactory.SetParam<ValueReader>(obj, param, this);
        }

        internal abstract ValueWriter ConvertToWriter();
        private ValueWriter toValueWriter()
        {
            if (null == Writer) Writer = ConvertToWriter();
            return Writer;
        }

        public static ValueWriter GetValueWriter(ValueReader reader) { return reader.toValueWriter(); }
    }
}
