using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class ParameterValueReader : ValueReader
    {
        internal readonly object Obj;
        internal readonly string Param;
        internal readonly DataContainer Owner;

        internal static ParameterValueReader GetParameterValueReader(object obj, string param, Scene scene)
        {
            Func<ParameterValueReader> create = new Func<ParameterValueReader>(() => new ParameterValueReader(obj, param, scene));
            return DoubleDictionary<object, string, ParameterValueReader>.GetItem(obj, param, create);
        }

        internal static void DeleteObject(object obj)
        {
            DoubleDictionary<object, string, ParameterValueReader>.DeleteDict(obj);
        }

        private ParameterValueReader(object obj, string param, Scene scene)
        {
            Obj = obj;
            Param = param;
            Type objType = obj.GetType();
            if (typeof(Component).IsAssignableFrom(objType)) Owner = ((Component)obj).Entity;
            else if (typeof(IManager).IsAssignableFrom(objType)) Owner = scene;
            else if (typeof(Service).IsAssignableFrom(objType)) Owner = scene.Game;
            else Owner = null;
        }

        internal override void SetupNotifications()
        {
            if (null != Owner)
                Owner.NotifyOfComponentChange(ClassFactory.GetReferedName(Obj.GetType()) + '.' + Param, this);
        }

        internal override double GetDoubleValue() 
        {
            object value = ClassFactory.GetParam(Obj, Param);
            return value as ValueReader ?? ToNumber<double>(value); 
        }
        internal override float GetFloatValue()
        {
            object value = ClassFactory.GetParam(Obj, Param);
            return value as ValueReader ?? ToNumber<float>(value); 
        }
        internal override int GetIntValue()
        {
            object value = ClassFactory.GetParam(Obj, Param);
            return value as ValueReader ?? ToNumber<int>(value); 
        }
        
        internal override long GetLongValue() 
        {
            object value = ClassFactory.GetParam(Obj, Param);
            return value as ValueReader ?? ToNumber<long>(value); 
        }

        internal override bool GetBoolValue()
        {
            object value = ClassFactory.GetParam(Obj, Param);
            return value as ValueReader ?? ToBool(value); 
        }

        internal override string GetStrValue()
        {
            object value = ClassFactory.GetParam(Obj, Param);
            return value as ValueReader ?? value.ToString();
        }

        internal override PreferedType PreferedRetType()
        {
            object value = ClassFactory.GetParam(Obj, Param);
            ValueReader reader = value as ValueReader;
            return reader == null? NativeReturnType(value) : reader.PreferedRetType();
        }

        internal override ValueWriter ConvertToWriter() { return new ParameterValueWriter(this); }
    }
}
