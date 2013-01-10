using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class ParameterValueReader : ValueReader
    {
        internal readonly Changeable Obj;
        internal readonly string Param;
        internal readonly IDataContainer Owner;

        internal static ParameterValueReader GetParameterValueReader(Changeable obj, string param, IDataContainer owner)
        {
            Func<ParameterValueReader> create = new Func<ParameterValueReader>(() => new ParameterValueReader(obj, param, owner));
            return DoubleDictionary<object, string, ParameterValueReader>.GetItem(obj, param, create);
        }

        internal static void DeleteObject(object obj)
        {
            DoubleDictionary<object, string, ParameterValueReader>.DeleteDict(obj);
        }

        private ParameterValueReader(Changeable obj, string param, IDataContainer owner)
        {
            Obj = obj;
            Param = param;
            Owner = owner;
        }

        internal override void SetupNotifications()
        {
            if (null != Owner)
                Owner.NotifyOfComponentChange(new Tuple<IChangeable, string>(Obj, Param), this);
        }

        internal override double GetDoubleValue() 
        {
            object value = Owner.GetParam(Obj, Param);
            return value as ValueReader ?? ToNumber<double>(value); 
        }
        internal override float GetFloatValue()
        {
            object value = Owner.GetParam(Obj, Param);
            return value as ValueReader ?? ToNumber<float>(value); 
        }
        internal override int GetIntValue()
        {
            object value = Owner.GetParam(Obj, Param);
            return value as ValueReader ?? ToNumber<int>(value); 
        }
        
        internal override long GetLongValue() 
        {
            object value = Owner.GetParam(Obj, Param);
            return value as ValueReader ?? ToNumber<long>(value); 
        }

        internal override bool GetBoolValue()
        {
            object value = Owner.GetParam(Obj, Param);
            return value as ValueReader ?? ToBool(value); 
        }

        internal override string GetStrValue()
        {
            object value = Owner.GetParam(Obj, Param);
            return value as ValueReader ?? value.ToString();
        }

        internal override PreferedType PreferedRetType()
        {
            object value = Owner.GetParam(Obj, Param);
            ValueReader reader = value as ValueReader;
            return reader == null? NativeReturnType(value) : reader.PreferedRetType();
        }

        internal override ValueWriter ConvertToWriter() { return new ParameterValueWriter(this); }
    }
}
